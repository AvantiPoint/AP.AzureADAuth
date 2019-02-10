using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AP.AzureADAuth.Xaml
{
    [ContentProperty(nameof(ResourceName))]
    [AcceptEmptyServiceProvider]
    internal class ColorExtension : IMarkupExtension
    {
        public string ResourceName { get; set; }

        public string HexString { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Application.Current.Resources.ContainsKey(ResourceName) &&
                Application.Current.Resources[ResourceName] is Color defaultColorName)
            {
                return defaultColorName;
            }

            if(Application.Current.Resources.ContainsKey($"{ResourceName}Color") &&
                Application.Current.Resources[$"{ResourceName}Color"] is Color qualifiedColorName)
            {
                return qualifiedColorName;
            }

            return Color.FromHex(HexString);
        }
    }
}
