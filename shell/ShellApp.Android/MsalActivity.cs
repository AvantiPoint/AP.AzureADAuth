
using Android.App;
using Microsoft.Identity.Client;
using Android.Content;
using ShellApp.Helpers;

namespace ShellApp.Droid
{
    [Activity]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataHost = "auth",
        DataScheme = Secrets.DataScheme)]
    public class MsalActivity : BrowserTabActivity
    {

    }
}