using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfacom.domain.media;
using cfacom.domain.story;
using cfacom.domain.tag;
using cfacore.domain._base;

namespace cfacom.domain.campaign
{
    public class Campaign:DomainObject,ICampaign
    {
        [MaxLength(100)]
        public string Name { get; set; }

        public int CampaignId { get; set; }

        [MaxLength(200)]
        public string Slug { get; set; }

        public virtual ICollection<ComMedia> Images { get; set; }

        public virtual ICollection<Story> Stories { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        public override string UriBase()
        {
            throw new NotImplementedException();
        }

        public override string ToChecksum()
        {
            throw new NotImplementedException();
        }
    }
}
