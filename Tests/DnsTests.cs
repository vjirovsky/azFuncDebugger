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
    internal class DnsTests
    {
        private LookupClient _dnsLookupEnforcedPrimaryClient = null;
        private LookupClient _dnsLookupEnforcedAltClient = null;
        private static IDictionary<string, string> _environmentVariablesDictionary = null;

        private string _enforcedPrimaryDnsServer = string.Empty;
        private string _enforcedAltDnsServer = string.Empty;


        internal DnsTests(IDictionary<string, string> environmentVariablesDictionary) {

            _environmentVariablesDictionary = environmentVariablesDictionary;


            
            _environmentVariablesDictionary.TryGetValue(Constants.APPSERVICE_DNS_SERVER_VARIABLE, out _enforcedPrimaryDnsServer);
            if (!string.IsNullOrEmpty(_enforcedPrimaryDnsServer))
            {
                var dnsLookupEnforcedPrimaryClientOptions = new LookupClientOptions(new NameServer(IPAddress.Parse(_enforcedPrimaryDnsServer)))
                {
                    UseCache = false,
                    Timeout = new TimeSpan(0, 0, 2),
                };
                _dnsLookupEnforcedPrimaryClient = new LookupClient(dnsLookupEnforcedPrimaryClientOptions);
            }


            
            _environmentVariablesDictionary.TryGetValue(Constants.APPSERVICE_DNS_ALT_SERVER_VARIABLE, out _enforcedAltDnsServer);
            if (!string.IsNullOrEmpty(_enforcedAltDnsServer))
            {
                var dnsLookupEnforcedAltClientOptions = new LookupClientOptions(new NameServer(IPAddress.Parse(_enforcedAltDnsServer)))
                {
                    UseCache = false,
                    Timeout = new TimeSpan(0, 0, 2),
                };
                _dnsLookupEnforcedAltClient = new LookupClient(dnsLookupEnforcedAltClientOptions);
            }

        }

        internal string RunAllTestsAsHtmlOutput()
        {
            string output = "";
            output += "<h2>DNS</h2>";


            output += "<h3>Active configuration</h3>";
            output += "<table class=\"table\">";

            if (string.IsNullOrEmpty(_enforcedPrimaryDnsServer) && string.IsNullOrEmpty(_enforcedAltDnsServer))
            {
                output += HtmlBrandingHelper.GetStandardTableRow("DNS servers", "<strong>Microsoft's managed public DNS servers</strong>");
            }
            else {
                output += HtmlBrandingHelper.GetStandardTableRow("DNS servers", $"<strong>Custom DNS servers</strong><br><ul><li>Primary server: {_enforcedPrimaryDnsServer}</li><li>Secondary server: {_enforcedAltDnsServer}</li></ul>");
            }


            output += "</table>";
            

            output += "<table class=\"table\">";

            //responseMessage += HtmlBrandingHelper.GetStandardTableRow("Status", "<span class=\"badge text-bg-success\">OK</span>/<span class=\"badge text-bg-danger\">Problem detected</span>");
            //responseMessage += HtmlBrandingHelper.GetStandardTableRow("Type", "Azure Files");
            output += "</table>";


            output += "<h2>DNS tests</h2>";

            var dnsDomainsToResolveList = new List<string>();

            string dnsDomainsToResolveToParse;
            _environmentVariablesDictionary.TryGetValue(Constants.TEST_DNS_RESOLVE_DOMAIN_VARIABLE, out dnsDomainsToResolveToParse);
            
            output += "<h3>DNS queries <small>(AppService nameresolver.exe)</small></h3>";
            output += "<table class=\"table\">";
#if DEBUG
    output += HtmlBrandingHelper.GetColspanTableRow($"Nameresolver.exe is not available on local machines, just in KUDU.");

#else
            if (dnsDomainsToResolveToParse != null)
            {
                // we do support multiple tests, delimited with ','
                foreach (var sub in dnsDomainsToResolveToParse.Split(','))
                {
                    dnsDomainsToResolveList.Add(sub);
                }
            }


            if (dnsDomainsToResolveList.Count == 0)
            {

                output += HtmlBrandingHelper.GetColspanTableRow($"No valid {Constants.TEST_DNS_RESOLVE_DOMAIN_VARIABLE} variable defined, test has been skipped.");

            }
            else
            {
                foreach (var domain in dnsDomainsToResolveList)
                {
                    string testResult = "";
                    try
                    {

                        Process p = new Process();
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.FileName = Path.Combine("C:\\windows\\system32", "nameresolver.exe");
                        p.StartInfo.Arguments = domain;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.Start();
                        p.WaitForExit();
                        

                        testResult = p.StandardOutput.ReadToEnd();
                    }
                    catch (Exception e)
                    {
                        testResult = "<span class=\"badge text-bg-danger\">QUERY FAILED</span> " + e.Message;
                    }

                    output += HtmlBrandingHelper.GetStandardTableRow($"{domain}", $"<pre>{testResult}</pre>");

                }
            }
            output += "</table>";
#endif

            //primary DNS requested by WEBSITE_DNS_SERVER

            string enforcedPrimaryDnsServer = string.Empty;
            _environmentVariablesDictionary.TryGetValue(Constants.APPSERVICE_DNS_SERVER_VARIABLE, out enforcedPrimaryDnsServer);

            if (!string.IsNullOrWhiteSpace(enforcedPrimaryDnsServer) && (dnsDomainsToResolveList.Count > 0) && _dnsLookupEnforcedPrimaryClient != null)
            {
                output += $"<h3>DNS queries <small>(Primary DNS server requested by {Constants.APPSERVICE_DNS_SERVER_VARIABLE}=<strong>{enforcedPrimaryDnsServer}</strong>)</small></h3>";
                output += "<table class=\"table\">";
                foreach (var domain in dnsDomainsToResolveList)
                {
                    string testResult = "";
                    try
                    {
                        var result = _dnsLookupEnforcedPrimaryClient.Query(domain, QueryType.A);

                        testResult = "<ul>";
                        foreach (var record in result.Answers)
                        {
                            testResult += "<li>" + record.ToString() + "</li>";
                        }
                        testResult += "</ul>";
                    }
                    catch (Exception e)
                    {
                        testResult = "<span class=\"badge text-bg-danger\">QUERY FAILED</span> " + e.Message;
                    }

                    output += HtmlBrandingHelper.GetStandardTableRow($"{domain}", testResult);

                }

                output += "</table>";
            }


            //secondary DNS requested by WEBSITE_DNS_ALT_SERVER

            string enforcedAltDnsServer = string.Empty;
            _environmentVariablesDictionary.TryGetValue(Constants.APPSERVICE_DNS_ALT_SERVER_VARIABLE, out enforcedAltDnsServer);

            if (!string.IsNullOrWhiteSpace(enforcedAltDnsServer) && (dnsDomainsToResolveList.Count > 0) && _dnsLookupEnforcedAltClient != null)
            {
                output += $"<h3>DNS queries <small>(Alt DNS server requested by {Constants.APPSERVICE_DNS_ALT_SERVER_VARIABLE}=<strong>{enforcedAltDnsServer}</strong>)</small></h3>";
                output += "<table class=\"table\">";
                foreach (var domain in dnsDomainsToResolveList)
                {

                    string testResult = "";
                    try
                    {
                        var result = _dnsLookupEnforcedAltClient.Query(domain, QueryType.A);

                        testResult = "<ul>";
                        foreach (var record in result.Answers)
                        {
                            testResult += "<li>" + record.ToString() + "</li>";
                        }
                        testResult += "</ul>";

                    }
                    catch (Exception e)
                    {
                        testResult = "<span class=\"badge text-bg-danger\">QUERY FAILED</span> " + e.Message;
                    }

                    output += HtmlBrandingHelper.GetStandardTableRow($"{domain}", testResult);

                }

                output += "</table>";
            }



            return output;
        }
    }
}
