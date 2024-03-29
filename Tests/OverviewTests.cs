﻿using DnsClient;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
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

            // site rg
            configItemValue = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_RESOURCE_GROUP_VARIABLE);
            if (!string.IsNullOrEmpty(configItemValue))
            {
                output += HtmlBrandingHelper.GetStandardTableRow("Resource group", $"<code>{configItemValue}</code>");
            }

            // site region
            configItemValue = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_REGION_NAME_VARIABLE);
            if (!string.IsNullOrEmpty(configItemValue))
            {
                output += HtmlBrandingHelper.GetStandardTableRow("Region", $"<code>{configItemValue}</code>");
            }


            // runtime
            configItemValue = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_FUNCTIONS_WORKER_RUNTIME_VARIABLE);
            if (!string.IsNullOrEmpty(configItemValue))
            {
                output += HtmlBrandingHelper.GetStandardTableRow("Runtime", $"<code>{configItemValue}</code>");
            }

            // OS
            configItemValue = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_OS_VARIABLE);
            var os = configItemValue;
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


            if (ContentStorageAccessTests.IsRunningFromLocalPackage(configItemValue, contentFileConnectionString, contentFileShare))
            {
                // RUN_FROM_PACKAGE is 1 or nothing is defined
                tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("overviewCodeRunning", TextProvider.GetTooltipTextLocalPackage(), false);
                configItemValue = "Running from local package";
            }
            else if (ContentStorageAccessTests.IsRunningFromRemotePackage(configItemValue, contentFileConnectionString, contentFileShare))
            {
                // RUN_FROM_PACKAGE is remote URL 
                tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("overviewCodeRunning", TextProvider.GetTooltipTextRemotePackage(configItemValue), false);
                configItemValue = "Running from package, remote URL";
            }
            else
            {
                // no RUN_FROM_PACKAGE is set, but file connection string is set
                tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("overviewCodeRunning", TextProvider.GetTooltipTextFromContentStorage(contentFileShare), false);
                configItemValue = "Running from Azure Files <button class='btn btn-sm btn-outline-info' onclick=\"const csaTabEl = document.querySelector('#main-tab4');const csaTab = new bootstrap.Tab(csaTabEl);csaTab.show()\" style=\"--bs-btn-padding-y: .25rem; --bs-btn-padding-x: .5rem; --bs-btn-font-size: .75rem;\">details</button><br>";
            }

            output += HtmlBrandingHelper.GetStandardTableRow("Code location", $"{configItemValue} {tooltipText}");

            // timezone
            output += HtmlBrandingHelper.GetStandardTableRow("Current time", $"<code>{DateTime.Now.ToShortTimeString()}</code>");
            
            configItemValue = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_TIME_ZONE_VARIABLE);

            bool isMatch;
            tooltipText = String.Empty;

            if (!string.IsNullOrEmpty(configItemValue))
            {
                if (os.ToLower().Equals("linux"))
                {
                    // guessing if timezone format is the Unix-expected
                    isMatch = Regex.IsMatch(configItemValue, "\\S+/\\S+", RegexOptions.IgnoreCase);
                    if (isMatch)
                    {
                        output += HtmlBrandingHelper.GetStandardTableRow("Timezone", "<code>" + configItemValue + $"</code>");
                    }
                    else
                    {
                        // is it valid Unix-timezone? seems like Windows-timezone
                        tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("timezoneProblem", TextProvider.GetTooltipTextForTimezoneWrongOnLinux(configItemValue), false);
                        output += HtmlBrandingHelper.GetStandardTableRow("Timezone",
                        $"<code>" + configItemValue + $"</code> <span class=\"badge text-bg-danger\">Probably wrong format</span><br>" +
                        $"{tooltipText}");
                    }
                    
                    
                }
                else if (os.ToLower().Equals("windows"))
                {
                    var correspondingTimeZone = TimeZoneInfo.GetSystemTimeZones().SingleOrDefault(tz => tz.Id.Equals(configItemValue));
                    if (correspondingTimeZone == null)
                    {
                        //timezone doesn't exist in current runtime
                        tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("timezoneProblem", TextProvider.GetTooltipTextForTimezoneWrongOnWindows(configItemValue), false);
                        output += HtmlBrandingHelper.GetStandardTableRow("Timezone",
                        $"<code>" + configItemValue + $"</code> <span class=\"badge text-bg-danger\">Unknown timezone</span><br>" +
                        $"{tooltipText}");
                    }
                    else {
                        tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("timezoneOk", TextProvider.GetTooltipTextForTimezoneRightOnWindows(configItemValue), false);
                        output += HtmlBrandingHelper.GetStandardTableRow("Timezone",
                        $"<code>" + configItemValue + $"</code>&nbsp; <span class=\"badge text-bg-success\">Valid timezone</span><br>" +
                        $"{tooltipText}");
                    }
                }
                else {
                    // unknown OS, skipping test
                    output += HtmlBrandingHelper.GetStandardTableRow("Timezone", $"<code>{configItemValue}</code>");
                }


            }


            output += "</table>";


            return output;
        }
    }
}
