using DnsClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AzFappDebugger.Tests
{
    internal class OverviewTests
    {
        private static IDictionary<string, string> _environmentVariablesDictionary = null;


        internal OverviewTests(IDictionary<string, string> environmentVariablesDictionary)
        {

            _environmentVariablesDictionary = environmentVariablesDictionary;

        }


        internal string RunAllTestsAsHtmlOutput()
        {
            string output = "", tooltipText = "";

            output += "<h2>Overview</h2>";


            output += "<h3>Active configuration</h3>";

            output += "<table class=\"table\">";

            string configItemValue = "";

            // Site name
            configItemValue = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_SITE_NAME_VARIABLE);
            if (!string.IsNullOrEmpty(configItemValue))
            {
                output += HtmlBrandingHelper.GetStandardTableRow("Site name", $"<code>{configItemValue}</code>");
            }

            // site hostname
            configItemValue = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_HOSTNAME_VARIABLE);
            if (!string.IsNullOrEmpty(configItemValue))
            {
                output += HtmlBrandingHelper.GetStandardTableRow("Website hostname", $"<code>{configItemValue}</code>");
            }

            // site hostname
            configItemValue = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_RESOURCE_GROUP_VARIABLE);
            if (!string.IsNullOrEmpty(configItemValue))
            {
                output += HtmlBrandingHelper.GetStandardTableRow("Resource group", $"<code>{configItemValue}</code>");
            }
            

            // runtime
            configItemValue = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_FUNCTIONS_WORKER_RUNTIME_VARIABLE);
            if (!string.IsNullOrEmpty(configItemValue))
            {
                output += HtmlBrandingHelper.GetStandardTableRow("Runtime", $"<code>{configItemValue}</code>");
            }

            // OS
            configItemValue = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_OS_VARIABLE);
            if (!string.IsNullOrEmpty(configItemValue))
            {
                output += HtmlBrandingHelper.GetStandardTableRow("Website OS", $"<code>{configItemValue}</code>");
            }

            // SKU
            configItemValue = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_SKU_VARIABLE);
            if (!string.IsNullOrEmpty(configItemValue))
            {
                output += HtmlBrandingHelper.GetStandardTableRow("SKU", $"<code>{configItemValue}</code>");
            }

            // Run from package
            string contentFileConnectionString = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_CONTENTAZUREFILECONNECTIONSTRING_VARIABLE);
            string contentFileShare = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_CONTENTSHARE_VARIABLE);
            configItemValue = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_RUN_FROM_PACKAGE_VARIABLE);
            if ((!string.IsNullOrEmpty(configItemValue) && configItemValue.Equals("1")) || (string.IsNullOrEmpty(configItemValue) && (string.IsNullOrEmpty(contentFileConnectionString))))
            {
                // RUN_FROM_PACKAGE is 1 or nothing is defined
                configItemValue = "Running from local package";
                tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("overviewCodeRunning",
                               $"<p>There is no other variable for code run set or the variable <code>{Constants.APPSERVICE_WEBSITE_RUN_FROM_PACKAGE_VARIABLE}</code> is set to value <code>1</code>, " +
                               $"so the runtime runs code located in <code>d:\\home\\data\\SitePackages</code> (Windows) " +
                               $"or <code>/home/data/SitePackages</code> (Linux).<br></p>" +
                               $"<p>Alternative options are: <code>&lt;&lt;url&gt;&gt;</code>, or running from Azure Files with variables <code>{Constants.APPSERVICE_WEBSITE_CONTENTAZUREFILECONNECTIONSTRING_VARIABLE}</code> and <code>{Constants.APPSERVICE_WEBSITE_CONTENTSHARE_VARIABLE}</code></p>" +
                               "<a href='https://docs.microsoft.com/en-us/azure/azure-functions/run-functions-from-deployment-package#enable-functions-to-run-from-a-package' target='_blank'>more about local package</a>, " +
                               "<a href='https://docs.microsoft.com/en-us/azure/azure-functions/run-functions-from-deployment-package#using-website_run_from_package--url' target='_blank'>more about URL package</a>, " +
                               "and <a href='https://docs.microsoft.com/en-us/azure/azure-functions/functions-infrastructure-as-code?tabs=windows#create-a-function-app' target='_blank'>more about Azure Files deployment</a></p>", false);
            }
            else if (!string.IsNullOrEmpty(configItemValue))
            {
                // RUN_FROM_PACKAGE is remote URL 
                tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("overviewCodeRunning",
                               $"<p>The variable <code>{Constants.APPSERVICE_WEBSITE_RUN_FROM_PACKAGE_VARIABLE}</code> is set to another value than <code>1</code>, so it runs a code from this URL <code>{configItemValue}</code>.<br></p>" +
                               $"<p>Alternative options are: <code>1</code> (run from local package), or running from Azure Files with variables <code>{Constants.APPSERVICE_WEBSITE_CONTENTAZUREFILECONNECTIONSTRING_VARIABLE}</code> and <code>{Constants.APPSERVICE_WEBSITE_CONTENTSHARE_VARIABLE}</code></p>" +
                               "<a href='https://docs.microsoft.com/en-us/azure/azure-functions/run-functions-from-deployment-package#using-website_run_from_package--url' target='_blank'>more about URL package</a>, " +
                               "<a href='https://docs.microsoft.com/en-us/azure/azure-functions/run-functions-from-deployment-package#enable-functions-to-run-from-a-package' target='_blank'>more about local package</a> " +
                               "and <a href='https://docs.microsoft.com/en-us/azure/azure-functions/storage-considerations#create-an-app-without-azure-files' target='_blank'>more about Azure Files deployment</a></p>", false);
                configItemValue = "Running from package, remote URL";
            }
            else {
                // no RUN_FROM_PACKAGE is set, but file connection string is set
                tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("overviewCodeRunning",
                               $"<p>The variable <code>{Constants.APPSERVICE_WEBSITE_CONTENTAZUREFILECONNECTIONSTRING_VARIABLE}</code> is set, so the code should be loaded from Azure Files storage <code>{contentFileShare}</code>." +
                               "<a href='https://docs.microsoft.com/en-us/azure/azure-functions/storage-considerations#create-an-app-without-azure-files' target='_blank'>more about Azure Files deployment</a>, " +
                               "<a href='https://docs.microsoft.com/en-us/azure/azure-functions/run-functions-from-deployment-package#using-website_run_from_package--url' target='_blank'>more about URL package</a> " +
                               "and <a href='https://docs.microsoft.com/en-us/azure/azure-functions/run-functions-from-deployment-package#enable-functions-to-run-from-a-package' target='_blank'>more about local package</a>", false);
                configItemValue = "Running from Azure Files";
            }

            output += HtmlBrandingHelper.GetStandardTableRow("Code location", $"{configItemValue} {tooltipText}");
            

            

            /*            tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("overview",
                                "<p>You have a custom DNS servers, " +
                                "that means all DNS requests from your application will be forwarded via vNET to selected DNS server(s).</p>" +
                                "<p>Out-of-box, this setup bypass any DNS records stored in Azure Private DNS Zones, so these records can be irresolvable from your application.</p>" +
                                "<p>To enable integration with Azure Private DNS Zones, you need to set up your DNS servers with a service Azure DNS Private Resolver to forward requests.<br> " +
                                "<a href='https://docs.microsoft.com/en-us/azure/dns/dns-private-resolver-overview' target='_blank'>more info</a></p>", false);

                        output += HtmlBrandingHelper.GetStandardTableRow("DNS servers",
                            $"<strong>Custom DNS servers</strong> <span class='badge text-bg-warning'>can affect Azure Private DNS Zones resolution</span><br>" +
                            $"<ul>" +
                            $"<li>Primary server: <code>{_enforcedPrimaryDnsServer}</code></li>" +
                            $"<li>Secondary server: <code>{_enforcedAltDnsServer}</code></li>" +
                            $"</ul> {tooltipText}");
            */


            output += "</table>";


            return output;
        }
    }
}
