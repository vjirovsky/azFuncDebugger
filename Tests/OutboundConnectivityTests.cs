using DnsClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AzFappDebugger.Tests
{
    internal class OutboundConnectivityTests
    {
        private static IDictionary<string, string> _environmentVariablesDictionary = null;

        private HttpClient _httpClient = null;

        internal OutboundConnectivityTests(IDictionary<string, string> environmentVariablesDictionary, HttpClient httpClient)
        {

            _environmentVariablesDictionary = environmentVariablesDictionary;
            _httpClient = httpClient;
        }



        internal async Task<string> RunAllTestsAsHtmlOutputAsync()
        {
            string output = "";
            output += "<h2>Overview</h2>";


            output += "<h3>Active configuration</h3>";

            output += "<table class=\"table\">";

            string configItemValue = "";

            // vNET name
            configItemValue = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_VNETNAME_VARIABLE);
            if (!string.IsNullOrEmpty(configItemValue))
            {
                output += HtmlBrandingHelper.GetStandardTableRow("Integrated vNET", $"<code>{configItemValue}</code>");
            }
            else { 
                output += HtmlBrandingHelper.GetStandardTableRow("Integrated vNET", $"<span class=\"badge text-bg-danger\">No vNET integration</span><br>" +
                    $"The application is not integrated with any vNET."); 
            }


            // Private IP
            configItemValue = Constants.GetEnvironmentVariableIfSet(_environmentVariablesDictionary, Constants.APPSERVICE_WEBSITE_PRIVATE_IP_VARIABLE);
            if (!string.IsNullOrEmpty(configItemValue))
            {
                output += HtmlBrandingHelper.GetStandardTableRow("Site private IP in vNET:", $"<code>{configItemValue}</code>");
            }


            output += "</table>";

            output += $"<h3>Connectivity tests</h3>";

            if (_httpClient != null)
            {

                string httpClientUrlToResolve = string.Empty;
                
                output += $"<h4>HttpClient test</h4> <small>";
                output += HtmlBrandingHelper.GetBootstrapWhatItMeans("OutboundConnectivityHttpClient",
                    $"<p>This test performs HTTP request via outbound connectivity of the application to hostname specified in <code>{Constants.TEST_HTTPCLIENT_RESOLVE_URL_VARIABLE}</code> configuration variable.</p>", false, false, "Test description");
                output += "</small>";


                _environmentVariablesDictionary.TryGetValue(Constants.TEST_HTTPCLIENT_RESOLVE_URL_VARIABLE, out httpClientUrlToResolve);

                if (!string.IsNullOrWhiteSpace(httpClientUrlToResolve))
                {
                    output += "<table class=\"table\">";
                    string testResult = "";
                    try
                    {
                        var content = await _httpClient.GetStringAsync(httpClientUrlToResolve);
                        testResult = $"<span class=\"badge text-bg-success\">OK</span><br><code>{HttpUtility.HtmlEncode(content)}</code>";
                    }
                    catch (Exception e)
                    {
                        testResult = "<span class=\"badge text-bg-danger\">QUERY FAILED</span><br> " + e.Message +"<br>" + (e.InnerException != null ? e.InnerException.Message : "");
                    }
                    finally
                    {
                        output += HtmlBrandingHelper.GetStandardTableRow($"{httpClientUrlToResolve}", testResult);
                    }

                    output += "</table>";
                }


                if (!string.IsNullOrWhiteSpace(Constants.MY_IP_ADDRESS_EXTERNAL_SERVICE_URL))
                {
                    output += $"<h4>My outbound IP address</h4> <small>";
                    output += HtmlBrandingHelper.GetBootstrapWhatItMeans("OutboundConnectivityExternalIp",
                        $"<p>This test performs a HTTP request to external service, which returns a IP address of HTTP request.</p>", false, false, "Test description");
                    output += "</small>";

                    output += "<table class=\"table\">";
                    string testResult = "";
                    try
                    {
                        var content = await _httpClient.GetStringAsync(Constants.MY_IP_ADDRESS_EXTERNAL_SERVICE_URL);
                        testResult = $"<code>{HttpUtility.HtmlEncode(content)}</code>";
                    }
                    catch (Exception e)
                    {
                        testResult = "<span class=\"badge text-bg-danger\">QUERY FAILED</span><br> " + e.InnerException.Message;
                    }
                    finally
                    {
                        output += HtmlBrandingHelper.GetStandardTableRow($"My IP address", testResult);
                    }

                    output += "</table>";
                }

            }




            return output;
        }
    }
}
