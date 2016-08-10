using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cfacom.domain.media;
using cfacore.domain.user;

namespace cfacom.domain.user
{
    public class ComUser:User
    {
        public virtual ComMedia Image { get; set; }
        public int? ImageId { get; set; }
    }
}
