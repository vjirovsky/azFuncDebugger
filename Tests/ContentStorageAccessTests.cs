using Azure.Storage.Files.Shares;
using DnsClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AzFappDebugger.Tests
{
    internal class ContentStorageAccessTests
    {
        private static IDictionary<string, string> _environmentVariablesDictionary = null;


        string _contentFileConnectionString;
        string _contentFileShare;
        string _contentOverVnet;
        string _runFromPackage;

        internal ContentStorageAccessTests(IDictionary<string, string> environmentVariablesDictionary)
        {

            _environmentVariablesDictionary = environmentVariablesDictionary;

            _contentFileConnectionString = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_CONTENTAZUREFILECONNECTIONSTRING_VARIABLE);
            _contentFileShare = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_CONTENTSHARE_VARIABLE);
            _runFromPackage = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_RUN_FROM_PACKAGE_VARIABLE);
            _contentOverVnet = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_CONTENTOVERVNET);
        }

        public static bool IsRunningFromLocalPackage(string runFromPackage, string contentFileConnectionString, string contentFileShare)
        {
            // RUN_FROM_PACKAGE is 1 (the highest priority)
            if (!string.IsNullOrEmpty(runFromPackage) && runFromPackage.Equals("1"))
            {
                return true;
            }

            //RUN_FROM_PACKAGE is 0 or remote URL
            if (!string.IsNullOrEmpty(runFromPackage))
            {
                return false;
            }

            //there is defined connection string and file share name for content storage
            if (!string.IsNullOrEmpty(contentFileConnectionString) && !string.IsNullOrEmpty(contentFileShare))
            {
                return false;
            }

            // default behavior
            return true;
        }

        public static bool IsRunningFromRemotePackage(string runFromPackage, string contentFileConnectionString, string contentFileShare)
        {
            if (IsRunningFromLocalPackage(runFromPackage, contentFileConnectionString, contentFileShare))
            {
                return false;
            }
            else if (!string.IsNullOrEmpty(runFromPackage))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsContentFileConnectionStringDefined(string contentFileConnectionStringEnv)
        {
            if (string.IsNullOrEmpty(contentFileConnectionStringEnv))
            {
                return false;
            }
            return true;
        }

        public static bool IsContentFileShareDefined(string contentFileShareEnv)
        {
            if (string.IsNullOrEmpty(contentFileShareEnv))
            {
                return false;
            }

            return true;
        }

        public static bool IsContentOverVnet(string contentOverVnetEnv)
        {
            if (!string.IsNullOrEmpty(contentOverVnetEnv) && contentOverVnetEnv.Equals("1"))
            {
                return true;
            }

            return false;
        }


        internal string RunAllTestsAsHtmlOutput()
        {
            string output = "", tooltipText = "", configItemValue = "";

            output += "<h2>Content Storage Access</h2>";

            output += "<h3>Active configuration</h3>";

            if (IsRunningFromLocalPackage(_runFromPackage, _contentFileConnectionString, _contentFileShare) || IsRunningFromRemotePackage(_runFromPackage, _contentFileConnectionString, _contentFileShare))
            {


                if (IsContentFileConnectionStringDefined(_contentFileConnectionString) && IsContentFileShareDefined(_contentFileShare))
                {
                    output += $"<div class='callout callout-warning'>" +
                        $"<strong>Warning</strong> - there are defined variables <code>{Constants.APPSERVICE_WEBSITE_CONTENTAZUREFILECONNECTIONSTRING_VARIABLE}</code>" +
                        $" and <code>{Constants.APPSERVICE_WEBSITE_CONTENTSHARE_VARIABLE}</code>. These variables will be ignored, " +
                        $"because <code>{Constants.APPSERVICE_WEBSITE_RUN_FROM_PACKAGE_VARIABLE}</code> has higher priority.</div>";
                }
                else if (IsContentFileConnectionStringDefined(_contentFileConnectionString) && !IsContentFileShareDefined(_contentFileShare))
                {
                    output += $"<div class='callout callout-warning'>" +
                        $"<strong>Warning</strong> - there is defined variable <code>{Constants.APPSERVICE_WEBSITE_CONTENTAZUREFILECONNECTIONSTRING_VARIABLE}</code>, " +
                        $"but missing required <code>{Constants.APPSERVICE_WEBSITE_CONTENTSHARE_VARIABLE}</code> variable. " +
                        $"This variable will be ignored.</div>";
                }
                else if (!IsContentFileConnectionStringDefined(_contentFileConnectionString) && IsContentFileShareDefined(_contentFileShare))
                {
                    output += $"<div class='callout callout-warning'>" +
                        $"<strong>Warning</strong> - there is defined variable <code>{Constants.APPSERVICE_WEBSITE_CONTENTSHARE_VARIABLE}</code>, " +
                        $"but missing required <code>{Constants.APPSERVICE_WEBSITE_CONTENTAZUREFILECONNECTIONSTRING_VARIABLE}</code> variable. " +
                        $"This variable will be ignored.</div>";
                }
                else
                {
                    // correct behavior
                }
            }
            output += "<table class=\"table\">";

            if (ContentStorageAccessTests.IsRunningFromLocalPackage(_runFromPackage, _contentFileConnectionString, _contentFileShare))
            {
                // RUN_FROM_PACKAGE is 1 or nothing is defined
                tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("casCodeRunning", TextProvider.GetTooltipTextLocalPackage(), false);
                configItemValue = "Running from local package";
            }
            else if (ContentStorageAccessTests.IsRunningFromRemotePackage(_runFromPackage, _contentFileConnectionString, _contentFileShare))
            {
                // RUN_FROM_PACKAGE is remote URL 
                tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("casCodeRunning", TextProvider.GetTooltipTextRemotePackage(configItemValue), false);
                configItemValue = "Running from package, remote URL";
            }
            else
            {
                // no RUN_FROM_PACKAGE is set, but file connection string is set
                tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("casCodeRunning", TextProvider.GetTooltipTextFromContentStorage(_contentFileShare), false);
                configItemValue = $"Running from Azure Files";
            }

            output += HtmlBrandingHelper.GetStandardTableRow("Code location", $"{configItemValue} {tooltipText}");


            if (IsContentFileConnectionStringDefined(_contentFileConnectionString))
            {
                string pattern = @"DefaultEndpointsProtocol=https;AccountName=(?<accountname>.*);AccountKey=(?<accountkey>.*);EndpointSuffix=core.windows.net";
                var parseConnectionStringRegex = new Regex(pattern).Match(_contentFileConnectionString);

                if (!parseConnectionStringRegex.Success)
                {
                    output += HtmlBrandingHelper.GetStandardTableRow("Content storage connection string", $"---- SANITATION OF SECRET FAILED, CAN'T DISPLAY BECAUSE OF SECURITY -----");
                }
                else
                {


                    string sanitizedConnectionString = _contentFileConnectionString.Replace(parseConnectionStringRegex.Groups["accountkey"].Value, "***************************************");

                    output += HtmlBrandingHelper.GetStandardTableRow("Storage account name", $"<code>{parseConnectionStringRegex.Groups["accountname"].Value}</code>");
                    output += HtmlBrandingHelper.GetStandardTableRow("Content storage connection string", $"<code>{sanitizedConnectionString}</code>");
                }
            }

            if (IsContentFileShareDefined(_contentFileShare))
            {
                output += HtmlBrandingHelper.GetStandardTableRow("Content file share", $"<code>{_contentFileShare}</code>");
            }

            //CONTENT_OVER_VNET
            if (IsContentFileConnectionStringDefined(_contentFileConnectionString) && IsContentFileShareDefined(_contentFileShare))
            {
                if (IsContentOverVnet(_contentOverVnet))
                {
                    tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("casContentOverVnet",
                    "<p>The content storage is accessed via integrated vNET with application.<br>" +
                    "This setup enables support for accessing content storage published via PrivateLink/vNET injection " +
                    "and supports increasing security of storage account by disabling public endpoint.<br>" +
                    "Please make sure the storage account is resolvable from the vNET and there is no NSG blocking access to the storage.<br><br>" +
                    $"You can turn off loading content via vNET with <code>{Constants.APPSERVICE_WEBSITE_CONTENTOVERVNET}</code> configuration variable.<br>" +
                    "<a href='https://docs.microsoft.com/en-us/azure/app-service/overview-vnet-integration#content-storage' target='_blank'>more info</a></p>", false);

                    output += HtmlBrandingHelper.GetStandardTableRow("Network",
                                            $"Content storage will be loaded via attached vNET " +
                                            $"<span class='badge text-bg-success'>supports private connection</span>" +
                                            $"{tooltipText}");
                }
                else
                {
                    tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("casContentOverVnet",
                    "<p>The content storage will be accessed via public outbound connection and public endpoint of storage account.<br><br>" +
                    "This setup doesn't support any private access to content storage published via PrivateLink or vNET injection, " +
                    "so storage account with content storage requires a public endpoint to be enabled.<br><br>" +
                    $"You can turn on loading content via vNET with <code>{Constants.APPSERVICE_WEBSITE_CONTENTOVERVNET}</code> configuration variable.<br>" +
                    "<a href='https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-vnet' target='_blank'>more info</a></p>", false);

                    output += HtmlBrandingHelper.GetStandardTableRow("Network",
                                            $"Content storage is loaded via internet " +
                                            $"<span class='badge text-bg-danger'>no private connection supported</span>" +
                                            $"{tooltipText}");
                }
            }


            output += "</table><br>";

            output += $"<h3>Content reachability tests</h3>";

            if (IsRunningFromLocalPackage(_runFromPackage, _contentFileConnectionString, _contentFileShare) || IsRunningFromRemotePackage(_runFromPackage, _contentFileConnectionString, _contentFileShare))
            {
                if (IsRunningFromLocalPackage(_runFromPackage, _contentFileConnectionString, _contentFileShare))
                {
                    output += $"<div class='callout callout-warning'>The application is running from local package, test has been skipped.</div>";
                }
                else
                {
                    output += $"<div class='callout callout-warning'>The application is running from package from remote URL, test has been skipped.</div>";
                }

            }
            else
            {

                try
                {
                    ShareClient fileShareClient = new ShareClient(_contentFileConnectionString, _contentFileShare);
                    var wwwrootDirClient = fileShareClient.GetDirectoryClient(Constants.FUNCTIONS_CONTENT_FUNCTION_DIR);
                    var filesAndDirs = wwwrootDirClient.GetFilesAndDirectories();


                    output += "<table class=\"table\">";


                    string tempOutput = "<ul>";
                    foreach (var item in filesAndDirs.Where(i => i.IsDirectory == true))
                    {
                        var subdir = wwwrootDirClient.GetSubdirectoryClient(item.Name);


                        //if file function.json exists, it's a registered function
                        if (subdir.GetFileClient(Constants.FUNCTIONS_FUNCTION_FILENAME).Exists())
                        {
                            tempOutput += $"<li><code>{item.Name}</code></li>";
                        }
                    }
                    tempOutput += $"</ul>";

                    output += HtmlBrandingHelper.GetStandardTableRow("Detected Functions in content storage", tempOutput);

                    output += "</table>";
                }
                catch (Exception e)
                {
                    output += "ERROR: Accessing CAS failed!";
                }


            }

            output += "<p><br></p>";

            return output;
        }
    }
}
