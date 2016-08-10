using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfacom.domain.story;
using cfacore.domain._base;

namespace cfacom.domain.tag
{
    public class Tag:DomainObject
    {
        [MaxLength(200)]
        [Required]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Slug { get; set; }
        //public string Slug2 { get; set; }
        //public string Slug3 { get; set; }

        [MaxLength(500)]
        public string KeyWords { get; set; }

        public int? TagId { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual ICollection<Story> Stories { get; set; }

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
