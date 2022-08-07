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
    internal class ContentStorageAccessSimulatorTests : ContentStorageAccessTests
    {
        private static IDictionary<string, string> _environmentVariablesDictionary = null;


        public ContentStorageAccessSimulatorTests(IDictionary<string, string> environmentVariablesDictionary): base(environmentVariablesDictionary)
        {

            _environmentVariablesDictionary = environmentVariablesDictionary;

            _contentFileConnectionString = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.TEST_WEBSITE_CONTENTAZUREFILECONNECTIONSTRING_VARIABLE);
            _contentFileShare = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.TEST_WEBSITE_CONTENTSHARE_VARIABLE);
            _runFromPackage = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_RUN_FROM_PACKAGE_VARIABLE);
            _contentOverVnet = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_CONTENTOVERVNET);
        }

        internal new string RunAllTestsAsHtmlOutput()
        {
            string output = "";

            output += "<h2>Content Storage Access (simulator)</h2>";

            output += $"<p>Debugging the access to content storage could be complicated - when the application doesn't have correct configuration, it has long response times and 5xx errors with no error details.<br>" +
                $"This debugger enables easier debugging experience, when you can run the debugger from the package and simulate content storage configuration from real environment before it's in place. <br>" +
                $"Simulator can be set by configuration variables <code>{Constants.TEST_WEBSITE_CONTENTAZUREFILECONNECTIONSTRING_VARIABLE}</code> and <code>{Constants.TEST_WEBSITE_CONTENTSHARE_VARIABLE}</code>.<br>" +
                $"</p><br>";


            output += "<h3>Simulated configuration</h3>";

            output += TestsHtmlOutput(Constants.TEST_WEBSITE_CONTENTAZUREFILECONNECTIONSTRING_VARIABLE, Constants.TEST_WEBSITE_CONTENTSHARE_VARIABLE, false);

            return output;
        }
    }
}
