using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace AP.AzureADAuth.Xaml
{
    [ContentProperty(nameof(FileName))]
    [AcceptEmptyServiceProvider]
    internal class LocalResourceExtension : IMarkupExtension<ImageSource>
    {
        public string FileName { get; set; }

        public ImageSource ProvideValue(IServiceProvider serviceProvider)
        {
            var app = Application.Current;

            if(app.Resources.ContainsKey("LoginLogo"))
            {
                switch(app.Resources["LoginLogo"])
                {
                    case ImageSource imageSource:
                        return imageSource;
                    case string resourceName:
                        return ImageSource.FromFile(resourceName);
                }
            }

            (var assembly, var resourceId) = GetResource(app);
            return ImageSource.FromResource(resourceId, assembly);
        }

        private (Assembly, string) GetResource(Application app)
        {
            var assembly = app.GetType().Assembly;
            var fileName = Path.HasExtension(FileName) ? FileName : $"{FileName}.png";
            var resourceId = assembly.GetManifestResourceNames()
                .FirstOrDefault(r => r.EndsWith(fileName, StringComparison.InvariantCultureIgnoreCase));
            if(string.IsNullOrEmpty(resourceId))
            {
                assembly = GetType().Assembly;
                resourceId = "AP.AzureADAuth.Images.Logo.png";
            }

            return (assembly, resourceId);
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
            ProvideValue(serviceProvider);
    }
}
