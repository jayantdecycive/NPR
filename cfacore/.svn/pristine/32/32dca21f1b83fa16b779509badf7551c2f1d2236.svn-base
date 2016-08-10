using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using cfacore.domain._base;
using cfacore.domain.user;
using System.Data.Linq.Mapping;
using core.synchronization.Automation;
using System.ComponentModel.DataAnnotations;

namespace cfacore.shared.domain.media
{
    
    public interface IMedia:IDomainObject
    {
        
        Uri MediaUri { get; set; }
        
        User Owner { get; set; }
        int? OwnerId { get; set; }

        bool IsSystem { get; set; }
       
        int Width { get; set; }
        
        int Height { get; set; }
        
        int Length { get; set; }
        
        long FileSize { get; set; }
        
        string Name { get; set; }
       
        string Description { get; set; }

        MediaType MediaType { get; set; }
        
        
        
        DateTime CreatedDate { get; set; }
        float Resolution { get; }

        int MediaId { get; set; }

        string MediaUriStr { get; set; }

        string ExternalUriStr { get; set; }

        System.Uri ExternalUri();

        int CropX { get; set; }
        int CropY { get; set; }
        int CropWidth { get; set; }
        int CropHeight { get; set; }

        Rectangle Crop { get; set; }

        Uri ThumbUri(bool cropped, int? width, int? height);

        Uri ThumbPrefix();
    }

    public enum MediaType { 
        Image,
        Vector,
        Audio,
        Video
    }
}