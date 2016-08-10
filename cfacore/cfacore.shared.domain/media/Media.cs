using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using cfacore.domain._base;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using cfacore.domain.user;
using cfacore.shared.domain.attributes;
using System.ComponentModel.DataAnnotations;

namespace cfacore.shared.domain.media
{
    [DataContract]
    [System.Data.Linq.Mapping.Table]
    [SyncUrl("/Api/Media")]
    [ModelId("MediaId")]
    public class Media:DomainObject,IMedia
    {
       

        public override string UriBase()            {
                return "http://data.core.chick-fil-a.com/media/base/";            
        }

		/// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Tuesday, March 06, 2012
		/// </created>
        
		
        public override System.Uri Uri()
        {
            return this.MediaUri;            
        }
        public override void Uri(System.Uri value) { 
            this.MediaUriStr = value.ToString();
        }

        [System.Data.Linq.Mapping.Column]
        [IgnoreDataMember]
        [XmlIgnore()]
        public System.Uri MediaUri {
            get { return new Uri(MediaUriStr); }
            set { this.MediaUriStr = value.ToString(); }
        }

        [DataMember]        
        public string MediaUriStr
        {
            get; set; }

        [XmlIgnore(), DataType("AutoComplete/_User"), UIHint("AdminLinkLoadable")]
        public virtual User Owner { get; set; }

        [XmlIgnore(), DataType("AutoComplete/_UserId"), UIHint("AdminLinkLoadable")]
        public int? OwnerId { get; set; }


        [DataMember, System.Data.Linq.Mapping.Column]
        public bool IsSystem { get; set; }


        [System.Data.Linq.Mapping.Column, DataMember]
        public int Width { get; set; }


        [DataMember, System.Data.Linq.Mapping.Column]
        public int Height { get; set; }


        [DataMember, System.Data.Linq.Mapping.Column]
        public int Length { get; set; }


        [DataMember]
        public int Size { get; set; }


        [System.Data.Linq.Mapping.Column]
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }


        /// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Tuesday, March 06, 2012
		/// </created>
        public float Resolution
        {
            get{ return 0.0f;}

        }


        [DataMember, System.Data.Linq.Mapping.Column]
        public long FileSize { get; set; }


        [DataMember, System.Data.Linq.Mapping.Column]
        public string Name { get; set; }


        public Media()
        {
            CreatedDate = DateTime.Now;
            Description = null;
            Name = null;
            FileSize = 0;
            Size = 0;
            Owner = null;
            IsSystem = false;
            Width = 0;
            Height = 0;
            Length = 0;
        }

        [DataMember, System.Data.Linq.Mapping.Column]
        public string Description { get; set; }


        /// <summary>
		/// Generated code from the Core Model Builder Add-on.
		/// </summary>
		/// <created>
		/// Generated Wednesday, March 14, 2012
		/// </created>        
        [System.Data.Linq.Mapping.Column]
        public MediaType MediaType
        {
            get;

            set;
        }

       

        public override string ToChecksum()
        {
            return Owner.UserId + Name+Id(); 
        }
        public string ExternalUriStr { get; set; }
        public Uri ExternalUri()
        {
            return new Uri(this.ExternalUriStr);
        }


        [DataMember]
        [Key]
        public int MediaId
        {
            get {return int.Parse(this.Id()??"0"); }
            set { this.Id(value.ToString()); }
        }

        public override string ToString()
        {
            return this.Name;
        }


        public int CropX
        {
            get; set; 
        }

        public int CropY
        {
            get; set; 
        }

        public int CropWidth
        {
            get; set; 
        }

        public int CropHeight
        {
            get; set; 
        }

        public string CropJson()
        {
            return new JavaScriptSerializer().Serialize(Crop);
        }

        public System.Drawing.Rectangle Crop
        {
            get
            {
                return new Rectangle(CropX, CropY, CropWidth, CropHeight);
            }
            set { this.CropX = value.X;
            this.CropWidth = value.Width;
            this.CropY = value.Y;
            this.CropHeight = value.Height;
            }
        }

        public Uri ThumbPrefix()
        {
            var builder = new UriBuilder(MediaUri);
            string filename = Path.GetFileName(builder.ToString());
            string extension = Path.GetExtension(builder.ToString());
            filename = Regex.Replace(filename, extension + "$", "");
            builder.Path = Regex.Replace(builder.Path,filename+extension+"$","")+string.Format("_{0}/",filename);
            return builder.Uri;
        }

        public Uri ThumbUri(bool cropped, int? width, int? height)
        {
            var builder = new UriBuilder(ThumbPrefix());
            string extension = Path.GetExtension(MediaUri.ToString());
            builder.Path = builder.Path + string.Format("{1}-{2}-{0}", cropped ? "c" : "nc", width == null ? "o" : width.Value.ToString(), height == null ? "o" : height.Value.ToString()) + extension;
            return builder.Uri;
        }
        [NotMapped]
        public DateTime CreationDate {
            get { return CreatedDate; }
            set { CreatedDate = value; }
        }
    }
}
