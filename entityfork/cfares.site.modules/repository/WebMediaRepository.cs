using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using ImageResizer;
using cfacore.shared.domain.media;
using cfares.entity.dbcontext.res_event;
using cfares.repository.store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using cfares.site.modules.com.application;

namespace cfacore.shared.modules.repository
{
    public class WebMediaRepository : MediaRepository
    {
        private string _bucketName=null;
        public override string BucketName
        {
            get
            {
                if(string.IsNullOrEmpty(_bucketName))
                    return ReservationConfig.GetS3Bucket();
                return _bucketName;
            }
            set { this._bucketName = value; }
        }

        public const int MAX_QR_DIMENSIONS = 300000;

        public static Uri QrCodeUrl(string content, int size = 300)
        {
            if (size*size > MAX_QR_DIMENSIONS)
            {
                size = (int)Math.Floor(Math.Sqrt(MAX_QR_DIMENSIONS));
            }

            string uriString =
                string.Format("http://chart.apis.google.com/chart?cht=qr&chs={1}x{2}&chld=L&choe=UTF-8&chl={0}",
                              HttpUtility.UrlEncode(content), size, size);
            return new Uri(uriString);
        }

        public static Uri QrCodeUrl(Uri content, int size = 300)
        {
            return QrCodeUrl(content.ToString(),size);
        }

        private string _bucketRoot = null;
        public override string BucketRoot
        {
            get
            {
                if (string.IsNullOrEmpty(_bucketRoot))
                    return ReservationConfig.GetS3BucketRoot();
                return _bucketRoot;
            }
            set { this._bucketRoot = value; }
        }


        
        public HttpControllerContext HttpContext;
        private const int MAX_IMAGE_WIDTH = 3200;
        private const int MAX_IMAGE_HEIGHT = 3200;

        public WebMediaRepository(IResContext context) : base(context, null, null)
        {
            
        }


        public WebMediaRepository(IResContext context, HttpControllerContext httpContext)
            : this(context)
        {

            this.HttpContext = httpContext;
        }

        public IMedia DownloadFromUri(string name, int? ownerId, string uri, string s3Uri)
        {
            return DownloadFromUri(new Media()
                {
                    ExternalUriStr = uri,
                    Name = name,
                    CreatedDate = DateTime.Now,
                    Description = name,
                    OwnerId = ownerId,
                    MediaUriStr = s3Uri
                });
        }


        public IMedia DownloadFromUri(IMedia media)
        {
            //throw new NotImplementedException("This does not work as expected");
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(media.ExternalUri());
                request.MaximumAutomaticRedirections = 4;
                request.MaximumResponseHeadersLength = 4;
                
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                using (var stream = response.GetResponseStream()) // accept the file
                {
                    byte[] imageBytes= new byte[]{};
                    byte[] chunk;
                    int CHUNK_SIZE = 500000;
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        do
                        {
                            chunk = br.ReadBytes(CHUNK_SIZE);
                            imageBytes = imageBytes.Concat(chunk).ToArray();
                        } while (chunk.Length == CHUNK_SIZE);
                        br.Close();
                    }
                    MemoryStream ms = new MemoryStream(imageBytes);
                    SyncToImage(media,ms);
                    UploadImage(media.Uri().PathAndQuery, ms);
                }
                return media;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }
        }

        protected void SyncToImage(IMedia media, Stream stream)
        {
            var img = Image.FromStream(stream, true, true);
            media.Width = img.Width;
            media.Height = img.Height;
        }

        public IMedia UploadFromInput(Media media, HttpPostedFileBase media_upload)
        {

            try
            {
                if (media_upload.ContentLength > 0) // accept the file
                {
                    SyncToImage(media,media_upload.InputStream);
                    
                    UploadImage(media.Uri().PathAndQuery, media_upload.InputStream);
                }
                return media;

            }catch(Exception ex){
                Console.Write(ex.Message);
                return null;
            }
        }

        

        protected S3Response UploadImage(string path, Stream stream)
        {
            AmazonS3 client;
            S3Response response = null;
            path = NormalizeUrl(path);

            using (client = Amazon.AWSClientFactory.CreateAmazonS3Client())
            {
                PutObjectRequest request = new PutObjectRequest();
                request.WithBucketName(BucketName)
               .WithCannedACL(S3CannedACL.PublicRead)
               .WithKey(path).InputStream = stream;
                response = client.PutObject(request);
            }
            return response;
        }

        public override void OnSave(object sender, cfares.repository._base.SavedEventArgs<Media> savedEventArgs)
        {
            // Special logic for res tickets:
            // RE: http://stackoverflow.com/questions/3686688/how-to-translate-objectstateentry-originalvalues-into-entity
            // .. Check object state to determine if we are "just now" activating this ticket
            // .. Only then do we want to send email messages

            if (!savedEventArgs.Created)
            {
                string modifiedProperty = savedEventArgs.ObjectState.GetModifiedProperties()
                                                            .FirstOrDefault(o => o == "MediaUriStr") ?? string.Empty;
                if (!string.IsNullOrEmpty(modifiedProperty))
                {
                    string origMediaUri = (string) savedEventArgs.ObjectState.OriginalValues[modifiedProperty];
                    string newMediaUri = (string) savedEventArgs.ObjectState.CurrentValues[modifiedProperty];
                    if (origMediaUri != newMediaUri)
                    {
                        DeleteImage(origMediaUri);
                    }
                }
            }


            string[] cropProps = new string[] { "CropX", "CropY", "CropWidth", "CropHeight" };
            bool reCrop = false;
            if (!savedEventArgs.Created)
            {
                foreach (string prop in cropProps)
                {
                    string modifiedProperty = savedEventArgs.ObjectState.GetModifiedProperties()
                                                            .FirstOrDefault(o => o == prop) ?? string.Empty;
                    if (string.IsNullOrEmpty(modifiedProperty))
                    {
                        continue;
                    }
                    int origCrop = (int) savedEventArgs.ObjectState.OriginalValues[modifiedProperty];
                    int currCrop = (int) savedEventArgs.ObjectState.CurrentValues[modifiedProperty];
                    if (origCrop != currCrop)
                    {
                        reCrop = true;
                    }
                }
            }
         


            // If status not modified, pull rip cord
            if (reCrop) DeleteThumbs(savedEventArgs.Instance);


           

            
            base.OnSave(sender, savedEventArgs);
        }

        protected S3Response GetImageS3(string path)
        {
            AmazonS3 client;
            S3Response response = null;
            path = NormalizeUrl(path);
            
            using (client = Amazon.AWSClientFactory.CreateAmazonS3Client())
            {
                GetObjectRequest request = new GetObjectRequest();
                request.WithBucketName(BucketName)
               .WithKey(path);
                response = client.GetObject(request);
            }
            return response;
        }

        protected Image GetImage(string path)
        {
            S3Response response = GetImageS3(path);
            return Image.FromStream(response.ResponseStream);
        }

        protected Image GetImage(IMedia model)
        {
            return GetImage(model.MediaUriStr);
        }

        protected Image GetImage(IMedia model, bool cropped)
        {
            var image = GetImage(model.MediaUriStr);
            if (image.Width > MAX_IMAGE_WIDTH)
            {
                image = ImageBuilder.Current.Build(image, new ResizeSettings(string.Format("width={0}&mode=crop&scale=both",
                                                                                          MAX_IMAGE_WIDTH)));
            }

            if (image.Height > MAX_IMAGE_HEIGHT)
            {
                image = ImageBuilder.Current.Build(image, new ResizeSettings(string.Format("height={0}&mode=crop&scale=both",
                                                                                          MAX_IMAGE_HEIGHT)));
            }
            if (cropped)
            {
                return ImageBuilder.Current.Build(image, new ResizeSettings(string.Format("crop=({0},{1},{2},{3})",
                                                                                          model.Crop.X, model.Crop.Y,
                                                                                          model.Crop.Right,
                                                                                          model.Crop.Bottom)));
            }
            else
            {
                return image;
            }
        }

        protected Image GetImageResized(IMedia model, bool cropped, int? width = null, int? height = null)
        {
            var image = GetImage(model,cropped);
            string fmt = "";
            if (width != null && height != null)
            {
                 fmt = string.Format("width={0}&height={1}",width.Value, height.Value);
            }
            else if(width!=null)
            {
                fmt = string.Format("width={0}", width.Value);
            }else if (height != null)
            {
                fmt = string.Format("height={0}", height.Value);
            }
            else
            {
                return image;
            }

            fmt = fmt + "&mode=crop&scale=both";
            //return image;
            return ImageBuilder.Current.Build(image, new ResizeSettings(fmt));
        }



        public string GetS3Thumb(IMedia model, bool cropped, int? width = null, int? height = null,bool force=false)
        {
			if( model == null ) return string.Empty; // no image selections currently supported

            string newUrl = model.ThumbUri(cropped,width,height).ToString();
            GetObjectMetadataResponse response;
            if (!force && ImageExists(newUrl, out response))
            {
                return newUrl+(newUrl.Contains("?")?"&":"?")+"ts="+response.LastModified.Ticks;
            }

            var image = GetImageResized(model, cropped, width, height);
            var stream = new System.IO.MemoryStream();
            var fmt = image.GetImageFormat();
            if (fmt.Equals(System.Drawing.Imaging.ImageFormat.Png) || fmt.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
            {
                EncoderParameter ratio = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 10L);
                EncoderParameters codecParams = new EncoderParameters(1);
                codecParams.Param[0] = ratio;
                image.Save(stream, GetEncoderInfo("image/png"), codecParams);
            }
            else
            {
                image.Save(stream, fmt);
            }


            stream.Position = 0;

            UploadImage(newUrl, stream);
            return newUrl;
        }

        public bool ThumbExists(IMedia model, bool cropped, int? width = null, int? height = null)
        {
            return ImageExists(model.ThumbUri(cropped,width,height).ToString());
        }

        public bool ImageExists(IMedia model)
        {
            return ImageExists(model.MediaUriStr);
        }

        public bool ImageExists(IMedia model, out GetObjectMetadataResponse response)
        {
            return ImageExists(model.MediaUriStr, out response);
        }

        public bool ImageExists(string path)
        {
            var r = new GetObjectMetadataResponse();
            return ImageExists(path, out r);
        }

        public bool ImageExists(string path, out GetObjectMetadataResponse response)
        {
            path = NormalizeUrl(path);
            try
            {
                using (AmazonS3 client = Amazon.AWSClientFactory.CreateAmazonS3Client())
                {
                    GetObjectMetadataRequest request = new GetObjectMetadataRequest();
                    request.WithBucketName(BucketName);
                    request.WithKey(path);
                    response = client.GetObjectMetadata(request);
                }
                return true;
            }
            catch (AmazonS3Exception ex)
            {
                response = null;
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;

                //status wasn't not found, so throw the exception
                throw;
            }
        }

        public S3Response DeleteImage(string path)
        {
            AmazonS3 client;
            S3Response response = null;
            path = NormalizeUrl(path);
            using (client = Amazon.AWSClientFactory.CreateAmazonS3Client())
            {
                DeleteObjectRequest request = new DeleteObjectRequest();
                request.WithBucketName(BucketName);
                request.WithKey(path);
                response = client.DeleteObject(request);
            }
            return response;
        }

        public S3Response DeleteThumbs(IMedia media)
        {
            return DeletePrefix(media.ThumbPrefix().ToString());
        }

        private string NormalizeUrl(string path)
        {
            if (path.StartsWith("http"))
            {
                Uri p = new Uri(path);
                path = p.PathAndQuery;
            }
            return path.TrimStart('/');
        }

        protected S3Response DeletePrefix(string prefix)
        {
            AmazonS3 client;
            S3Response response = null;
            prefix = NormalizeUrl(prefix);

            using (client = Amazon.AWSClientFactory.CreateAmazonS3Client())
            {
                var listrequest = new ListObjectsRequest()
                .WithBucketName(BucketName)
                .WithPrefix(prefix)
                .WithDelimiter(@"/");


                using (var listResponse = client.ListObjects(listrequest))
                {
                    var keys = listResponse.S3Objects.Select(x => new KeyVersion(x.Key)).ToArray();
                    if (keys != null && keys.Length>0)
                    {
                        DeleteObjectsRequest request = new DeleteObjectsRequest();
                        request.WithBucketName(BucketName);
                        request.WithKeys(keys);
                        response = client.DeleteObjects(request);
                    }
                }

                
            }
            return response;
        }

        public override void Delete(IMedia entity)
        {
            base.Delete(entity);
            DeleteImage(entity.MediaUri.PathAndQuery);
            DeleteThumbs(entity);
        }



        public IMedia FindOrCreateByExternalUri(string imgUrl,int? ownerId)
        {
            var media = Find(x=>x.ExternalUriStr==imgUrl);
            if (media != null)
            {
                return media;
            }
            var uri = new Uri(imgUrl);
            var mediaName = "Img from " + imgUrl;
            var uriStr = GenUri(mediaName, "pulled", uri.PathAndQuery).ToString();
            media = DownloadFromUri(mediaName, ownerId, imgUrl,uriStr);
            Add(media);
            Commit();
            return media;
        }

        public void RefreshPulledImage(Media media)
        {
            DeleteThumbs(media);
            DownloadFromUri(media);
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }
    public static class ImageExtentions
    {
        

        public static System.Drawing.Imaging.ImageFormat GetImageFormat(this System.Drawing.Image img)
        {
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
                return System.Drawing.Imaging.ImageFormat.Jpeg;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp))
                return System.Drawing.Imaging.ImageFormat.Bmp;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png))
                return System.Drawing.Imaging.ImageFormat.Png;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Emf))
                return System.Drawing.Imaging.ImageFormat.Emf;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Exif))
                return System.Drawing.Imaging.ImageFormat.Exif;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif))
                return System.Drawing.Imaging.ImageFormat.Gif;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Icon))
                return System.Drawing.Imaging.ImageFormat.Icon;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.MemoryBmp))
                return System.Drawing.Imaging.ImageFormat.Bmp;
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Tiff))
                return System.Drawing.Imaging.ImageFormat.Tiff;
            else
                return System.Drawing.Imaging.ImageFormat.Wmf;
        }
    }
}
