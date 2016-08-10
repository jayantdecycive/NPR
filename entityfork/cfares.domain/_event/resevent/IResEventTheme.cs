using System;
namespace cfares.domain._event.resevent
{
    interface IResEventTheme
    {
        
        string Name { get; set; }
        
        ResTemplate Template { get; set; }
        string TemplateName { get; set; }
        string ResEventThemeId { get; set; }
    }
}
