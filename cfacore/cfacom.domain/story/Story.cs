using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfacom.domain._enum;
using cfacom.domain.campaign;
using cfacom.domain.media;
using cfacom.domain.tag;
using cfacore.domain._base;

namespace cfacom.domain.story
{
    public class Story:DomainObject,IStory
    {
        public override string UriBase()
        {
            throw new NotImplementedException();
        }

        public override string ToChecksum()
        {
            throw new NotImplementedException();
        }

        [MaxLength(100)]
        public string Slug { get; set; }

        [MaxLength(200)]
        public string TagLine { get; set; }

        
        [MaxLength(200)]
        [Required]
        public string Title { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        public int StoryId { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual Campaign Campaign { get; set; }
        public int? CampaignId { get; set; }

        public virtual ICollection<ComMedia> Images { get; set; }

        public PublishStatus Status { get; set; }

        public long? OrderedBy { get; set; }
        
        #region should_be_changed
        [MaxLength(200)]
        public string FacebookTagLine { get; set; }

        public string FacebookBody { get; set; }

        [MaxLength(200)]
        public string TwitterTagLine { get; set; }

        [MaxLength(200)]
        public string LinkedInTagLine { get; set; }

        [MaxLength(200)]
        public string GooglePlusTagLine { get; set; }
        #endregion
    }
}
