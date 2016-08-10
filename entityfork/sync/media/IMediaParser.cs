using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cfacore.shared.domain.media;
using sync._base;

namespace sync.media
{
    public interface IMediaParser 
    {

        
        Uri[] QueryLocalDirectory(Uri uri);
        Media[] QueryAndParseUris(Uri[] uris,string filebase , string webBase);

    }
}
