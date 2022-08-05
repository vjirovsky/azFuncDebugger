﻿using DnsClient;
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
    internal class HostDataAccessTests
    {
        private static IDictionary<string, string> _environmentVariablesDictionary = null;

        internal HostDataAccessTests(IDictionary<string, string> environmentVariablesDictionary)
        {

            _environmentVariablesDictionary = environmentVariablesDictionary;
        }


        internal string RunAllTestsAsHtmlOutput()
        {
            string output = "", tooltipText = "";

            output += "<h2>Host Data Access</h2>";


            output += "<br><p>TODO: in future releases, there will be tests for host blob storage reachability.</p>";


            return output;
        }
    }
}
