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

        private List<string> interestingEnvVariables = new List<string>() {
            Constants.APPSERVICE_VNET_ROUTEALL_VARIABLE,
            Constants.APPSERVICE_DNS_SERVER_VARIABLE,
            Constants.APPSERVICE_DNS_ALT_SERVER_VARIABLE
        };


        internal DnsTests(IDictionary<string, string> environmentVariablesDictionary)
        {

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

        public bool IsEnforcedPrimaryDns()
        {
            if (string.IsNullOrEmpty(_enforcedPrimaryDnsServer) && string.IsNullOrEmpty(_enforcedAltDnsServer))
            {
                return false;
            }
            return true;
        }

        public bool IsEnforcedPrimaryDnsReservedPrivateDnsServer()
        {
            if (!string.IsNullOrEmpty(_enforcedPrimaryDnsServer) && _enforcedPrimaryDnsServer.Equals(Constants.APPSERVICE_DNS_SERVER_RESERVED_PRIVATE_DNS))
            {
                return true;
            }
            return false;
        }

        public bool IsEnforcedAltDns()
        {
            if (string.IsNullOrEmpty(_enforcedAltDnsServer))
            {
                return false;
            }
            return true;
        }

        public bool IsEnforcedAltDnsReservedPrivateDnsServer()
        {
            if (!string.IsNullOrEmpty(_enforcedAltDnsServer) && _enforcedAltDnsServer.Equals(Constants.APPSERVICE_DNS_SERVER_RESERVED_PRIVATE_DNS))
            {
                return true;
            }
            return false;
        }

        public bool IsCustomDns()
        {
            return !IsEnforcedPrimaryDns() && !IsEnforcedAltDns();
        }


        public static string RunNameresolverExeTest(string domain)
        {

            string testResult = "";
            try
            {

                Process p = new Process();
                p.StartInfo.UseShellExecute = false;

                var enviromentPath = Environment.GetEnvironmentVariable("PATH");

                var paths = enviromentPath.Split(';');
                var exePath = paths.Select(x => Path.Combine(x, Constants.NAMERESOLVER_TOOL_EXECUTABLE))
                                   .Where(x => File.Exists(x))
                                   .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(exePath))
                {
                    throw new Exception("The tool " + Constants.NAMERESOLVER_TOOL_EXECUTABLE + " could not be found.");
                }

                p.StartInfo.FileName = exePath;
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

            return testResult;
        }


        internal string RunAllTestsAsHtmlOutput()
        {
            string output = "", tooltipText = "";

            output += "<h2>DNS</h2>";


            output += "<h3>Active configuration</h3>";
            output += "<table class=\"table\">";

            if (!IsEnforcedPrimaryDns())
            {
                output += HtmlBrandingHelper.GetStandardTableRow("DNS servers",
                    "<strong>Microsoft's managed public DNS servers</strong>" +
                    "<span class='badge text-bg-warning'>problematic Azure Private DNS Zones support</span>" +

                    HtmlBrandingHelper.GetBootstrapWhatItMeans("dnsServer",
                    "<p>The application is using default Azure public DNS servers.</p>" +
                    "<p>During access to private resources, this setup could introduce random issues with resolving records hosted in Azure Private DNS Zones within vNET." +
                    "Sometimes private resources (PrivateLink) could be resolved with their public IP addresses instead of the desired private IP address.</p>" +
                    $"<p>To avoid such problems, set the configuration variable <i>{Constants.APPSERVICE_DNS_SERVER_VARIABLE}</i> to value <code>{Constants.APPSERVICE_DNS_SERVER_RESERVED_PRIVATE_DNS}</code><br>" +
                    $"<a href='https://docs.microsoft.com/en-us/azure/private-link/private-endpoint-dns#virtual-network-and-on-premises-workloads-using-a-dns-forwarder' target='_blank'>more info</a></p>", false)
                    );
            }
            else if (IsEnforcedPrimaryDnsReservedPrivateDnsServer())
            {
                output += HtmlBrandingHelper.GetStandardTableRow("DNS servers",
                    "<strong>Microsoft's managed public DNS servers</strong> " +
                    "<span class='badge text-bg-success'>with Azure Private DNS Zones support</span>" +

                    HtmlBrandingHelper.GetBootstrapWhatItMeans("dnsServer",
                    "<p>This setup enables resolving records hosted in Azure Private DNS Zones within vNET.<br>" +
                    "All private resources should be resolvable from the application.</p>", false));
            }
            else if (!IsEnforcedPrimaryDnsReservedPrivateDnsServer() && IsEnforcedAltDnsReservedPrivateDnsServer())
            {
                output += HtmlBrandingHelper.GetStandardTableRow("DNS servers",
                    "<strong>Microsoft's managed public DNS servers</strong> " +
                    "<span class='badge text-bg-success'>with Azure Private DNS Zones support</span><span class='badge text-bg-warning'>can cause Azure Private DNS Zones issues</span>" +

                    HtmlBrandingHelper.GetBootstrapWhatItMeans("dnsServer",
                    "<p>This setup enables resolving records hosted in Azure Private DNS Zones within vNET, but it does not follow best practice, " +
                    "because it uses the recommended server for Azure Private DNS Zones resolution as alternative DNS server instead of primary one.<br><br>" +
                    "All private resources should be resolvable from the application, but it could bring some strange random issues during resolution.</p>" +
                    $"<p>To avoid such problems, set the configuration variable <i>{Constants.APPSERVICE_DNS_SERVER_VARIABLE}</i> to value <code>{Constants.APPSERVICE_DNS_SERVER_RESERVED_PRIVATE_DNS}</code></p>", false));
            }
            else
            {
                //I do have custom DNS only

                tooltipText = HtmlBrandingHelper.GetBootstrapWhatItMeans("dnsServer",
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

            }


            output += "</table>";


            output += "<h3>DNS tests</h3>";
            output += $"<p>You can perform test to resolve domain(s) from your application by setting up a configuration variable <code>{Constants.TEST_DNS_RESOLVE_DOMAIN_VARIABLE}</code>.<br>" +
                $"<small><i>Example configuration:</i> <code>{Constants.TEST_DNS_RESOLVE_DOMAIN_VARIABLE}=microsoft.com,azure.com,mystorageaccount.blob.privatelink.core.windows.net</code></small></p>";

            var dnsDomainsToResolveList = new List<string>();

            string dnsDomainsToResolveToParse;
            _environmentVariablesDictionary.TryGetValue(Constants.TEST_DNS_RESOLVE_DOMAIN_VARIABLE, out dnsDomainsToResolveToParse);

            if (dnsDomainsToResolveToParse != null)
            {
                // we do support multiple tests, delimited with ','
                foreach (var sub in dnsDomainsToResolveToParse.Split(','))
                {
                    dnsDomainsToResolveList.Add(sub);
                }
            }

            output += "<h4>AppService resolution test</h4> <small>";
            output += HtmlBrandingHelper.GetBootstrapWhatItMeans("TestResolutionNameresolver",
                "This test is performed via <code>nameresolver.exe</code> tool, which provides exactly same results as query resolution in your application.<br><br>" +
                "This tool is available just in Azure AppService environment, so the test will fail on the local computer.", false, false, "Test description");
            output += "</small>";

#if DEBUG
            output += $"<div class='callout callout-danger'>Nameresolver.exe is not available on local machines, just in KUDU engine.</div>";

#else


            if (dnsDomainsToResolveList.Count == 0)
            {

                output += $"<div class='callout callout-warning'>No valid <code>{Constants.TEST_DNS_RESOLVE_DOMAIN_VARIABLE}</code> variable defined, test has been skipped.</div>";

            }
            else
            {

                output += "<table class=\"table\">";

                foreach (var domain in dnsDomainsToResolveList)
                {
                    string testResult = RunNameresolverExeTest(domain);
                    output += HtmlBrandingHelper.GetStandardTableRow($"{domain}", $"<pre>{testResult}</pre>");

                }

                output += "</table>";
            }
#endif

            //primary DNS requested by WEBSITE_DNS_SERVER

            if (IsEnforcedPrimaryDns() && (dnsDomainsToResolveList.Count > 0) && _dnsLookupEnforcedPrimaryClient != null)
            {
                output += $"<h4>Primary DNS server <code>{_enforcedPrimaryDnsServer}</code></h4> <small>";
                output += HtmlBrandingHelper.GetBootstrapWhatItMeans("TestResolutionPrimaryDnsServer",
                    $"<p>This test performs DNS resolution via DNS client included in this debugger, directly against server <code>{_enforcedPrimaryDnsServer}</code> specified in <i>{Constants.APPSERVICE_DNS_SERVER_VARIABLE}</i> configuration variable.</p>", false, false, "Test description");
                output += "</small>";
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

            if (IsEnforcedAltDns() && (dnsDomainsToResolveList.Count > 0) && _dnsLookupEnforcedAltClient != null)
            {
                output += $"<h4>Alternate DNS server <code>{_enforcedAltDnsServer}</code></h4> <small>";
                output += HtmlBrandingHelper.GetBootstrapWhatItMeans("TestResolutionAltDnsServer",
                    $"<p>This test performs DNS resolution via DNS client included in this debugger, directly against server <code>{_enforcedAltDnsServer}</code> specified in <code>{Constants.APPSERVICE_DNS_ALT_SERVER_VARIABLE}</code> configuration variable.</p>", false, false, "Test description");
                output += "</small>";

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
