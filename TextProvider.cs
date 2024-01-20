using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace AzFappDebugger
{
    internal static class TextProvider
    {

        internal static string GetTooltipTextLocalPackage()
        {
            return
                $"<p>There is no other variable for code run set or the variable <code>{Constants.APPSERVICE_WEBSITE_RUN_FROM_PACKAGE_VARIABLE}</code> is set to value <code>1</code>, " +
                $"so the runtime runs code located in <code>d:\\home\\data\\SitePackages</code> (Windows) " +
                $"or <code>/home/data/SitePackages</code> (Linux).<br></p>" +
                $"<p>Alternative options are: <code>&lt;&lt;url&gt;&gt;</code>, or running from Azure Files with variables <code>{Constants.APPSERVICE_WEBSITE_CONTENTAZUREFILECONNECTIONSTRING_VARIABLE}</code> and <code>{Constants.APPSERVICE_WEBSITE_CONTENTSHARE_VARIABLE}</code></p>" +
                "<a href='https://docs.microsoft.com/en-us/azure/azure-functions/run-functions-from-deployment-package#enable-functions-to-run-from-a-package' target='_blank'>more about local package</a>, " +
                "<a href='https://docs.microsoft.com/en-us/azure/azure-functions/run-functions-from-deployment-package#using-website_run_from_package--url' target='_blank'>more about URL package</a>, " +
                "and <a href='https://docs.microsoft.com/en-us/azure/azure-functions/functions-infrastructure-as-code?tabs=windows#create-a-function-app' target='_blank'>more about Azure Files deployment</a></p>";
        }


        internal static string GetTooltipTextRemotePackage(string remoteUrl)
        {
            return
                $"<p>The variable <code>{Constants.APPSERVICE_WEBSITE_RUN_FROM_PACKAGE_VARIABLE}</code> is set to another value than <code>1</code>, so it runs a code from URL <code>{remoteUrl}</code>.<br></p>" +
                $"<p>Alternative options are: <code>1</code> (run from local package), or running from Azure Files with variables <code>{Constants.APPSERVICE_WEBSITE_CONTENTAZUREFILECONNECTIONSTRING_VARIABLE}</code> and <code>{Constants.APPSERVICE_WEBSITE_CONTENTSHARE_VARIABLE}</code></p>" +
                "<a href='https://docs.microsoft.com/en-us/azure/azure-functions/run-functions-from-deployment-package#using-website_run_from_package--url' target='_blank'>more about URL package</a>, " +
                "<a href='https://docs.microsoft.com/en-us/azure/azure-functions/run-functions-from-deployment-package#enable-functions-to-run-from-a-package' target='_blank'>more about local package</a> " +
                "and <a href='https://docs.microsoft.com/en-us/azure/azure-functions/storage-considerations#create-an-app-without-azure-files' target='_blank'>more about Azure Files deployment</a></p>";
        }

        internal static string GetTooltipTextFromContentStorage(string contentFileShare)
        {
            return
                $"<p>The variable <code>{Constants.APPSERVICE_WEBSITE_CONTENTAZUREFILECONNECTIONSTRING_VARIABLE}</code> is set, so the code should be loaded from Azure Files storage <code>{contentFileShare}</code>.<br>" +
                "<a href='https://docs.microsoft.com/en-us/azure/azure-functions/storage-considerations#create-an-app-without-azure-files' target='_blank'>more about Azure Files deployment</a>, " +
                "<a href='https://docs.microsoft.com/en-us/azure/azure-functions/run-functions-from-deployment-package#using-website_run_from_package--url' target='_blank'>more about URL package</a> " +
                "and <a href='https://docs.microsoft.com/en-us/azure/azure-functions/run-functions-from-deployment-package#enable-functions-to-run-from-a-package' target='_blank'>more about local package</a>";
        }

        internal static string GetTooltipTextForTimezoneWrongOnLinux(string timeZone)
        {
            return
                $"<p>The runtime OS is Linux and the variable <code>{Constants.APPSERVICE_WEBSITE_TIME_ZONE_VARIABLE}</code> is set, but the value <code>{timeZone}</code> seems to be in non-Unix format for timezones.<br><br>" +
                "<a href='https://en.wikipedia.org/wiki/List_of_tz_database_time_zones' target='_blank'>List of supported Unix time zones</a> (<i>TZ identifier</i> column).";
        }

        internal static string GetTooltipTextForTimezoneWrongOnWindows(string timeZone)
        {
            return
                $"<p>The runtime OS is Windows and the variable <code>{Constants.APPSERVICE_WEBSITE_TIME_ZONE_VARIABLE}</code> is set, but the value <code>{timeZone}</code> is not supported in current runtime.<br><br>" +
                "<a href='https://learn.microsoft.com/en-us/previous-versions/windows/it-pro/windows-vista/cc749073(v=ws.10)?redirectedfrom=MSDN#time-zones' target='_blank'>List of supported time zones on Windows</a>";
        }

        internal static string GetTooltipTextForTimezoneRightOnWindows(string timeZone)
        {
            return
                $"<p>The variable <code>{Constants.APPSERVICE_WEBSITE_TIME_ZONE_VARIABLE}</code> is set to supported value in current runtime.<br><br>" +
                "<a href='https://learn.microsoft.com/en-us/previous-versions/windows/it-pro/windows-vista/cc749073(v=ws.10)?redirectedfrom=MSDN#time-zones' target='_blank'>List of supported time zones on Windows</a>";
        }

    }
}
