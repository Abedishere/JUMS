using System.Globalization;

namespace JUMS.Infrastructure.Localization
{
    public interface ISharedLocalizer
    {
        string this[string key] { get; }
        string this[string key, params object[] args] { get; }
        CultureInfo CurrentCulture { get; }
    }
}