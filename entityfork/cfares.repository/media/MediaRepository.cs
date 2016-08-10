using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfacore.shared.domain.store;
using cfares.entity.dbcontext.res_event;
using cfares.repository._base;
using cfacore.shared.domain.media;
using System.IO;

namespace cfares.repository.store
{
    public class MediaRepository : GenericRepository<IResContext, Media, int,IMedia>
    {
        public MediaRepository(IResContext context, string bucket, string bucketRoot) : base(context)
        {
            if(bucket!=null)
                BucketName = bucket;
            if (bucket != null)
                BucketRoot = bucketRoot;

        }

        public virtual string BucketName { get; set; }


        public virtual string BucketRoot { get; set; }

        public override IQueryable<IMedia> PublicQuerySet()
        {
            return base.PublicQuerySet().Where(x=>!x.IsSystem);
        }

        public Uri GenUri(string name,string path, string file) {
            string slug = Slugify(name);
            path = Slugify(path);
            string extention = Path.GetExtension(file);
            Uri outUri = null;
            int i=0;

            IMedia existing;

            do{
                outUri = new Uri(string.Format("{0}/uploads/img/{1}/{2}{3}", BucketRoot, path, slug, extention));

                
                i++;
                slug=slug+i;
                existing = FindByUri(outUri);
                if(existing!=null)
                Detatch(existing);
            } while (existing != null);
            
            return outUri;
        }

        

        public IMedia FindByUri(Uri uri){
            string uriStr = uri.ToString();
            return GetAll().FirstOrDefault(x=>x.MediaUriStr==uriStr);
        }

        public override IMedia FindBySlug(string slug)
        {
            throw new NotImplementedException();
        }
    }
}
