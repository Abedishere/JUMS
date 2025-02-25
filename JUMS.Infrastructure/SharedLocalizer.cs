using Microsoft.Extensions.Localization;
using System.Globalization;

namespace JUMS.Infrastructure.Localization
{
    public class SharedLocalizer : ISharedLocalizer
    {
        private readonly IStringLocalizer _localizer;
        
        public SharedLocalizer(IStringLocalizerFactory factory)
        {
            // Load a generic resource file for the application
            _localizer = factory.Create("SharedResources", typeof(SharedResources).Assembly.FullName);
        }
        
        public CultureInfo CurrentCulture => CultureInfo.CurrentCulture;
        
        public string this[string key] => _localizer[key];
        
        public string this[string key, params object[] args] => _localizer[key, args];
    }
    
    // Marker class for the localization resources
    public class SharedResources
    {
    }
}