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
    internal class ContentStorageAccessTests
    {
        private static IDictionary<string, string> _environmentVariablesDictionary = null;


        internal ContentStorageAccessTests(IDictionary<string, string> environmentVariablesDictionary)
        {

            _environmentVariablesDictionary = environmentVariablesDictionary;

        }


        internal string RunAllTestsAsHtmlOutput()
        {
            string output = "", tooltipText = "";

            output += "<h2>Content Storage Access</h2>";


            output += "<br><p>TODO: in future releases, there will be tests for content storage reachability.</p>";


            return output;
        }
    }
}
