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
    internal class ConnectivityTests
    {
        private static IDictionary<string, string> _environmentVariablesDictionary = null;

        private HttpClient _httpClient = null;

        internal ConnectivityTests(IDictionary<string, string> environmentVariablesDictionary, HttpClient httpClient)
        {

            _environmentVariablesDictionary = environmentVariablesDictionary;
            _httpClient = httpClient;


        }



        internal async Task<string> RunAllTestsAsHtmlOutputAsync()
        {
            string output = "";


            output += $"<h2>Connectivity tests</h2>";

            if (_httpClient != null)
            {

                string httpClientUrlToResolve = string.Empty;
                output += $"<h3>HttpClient tests</h3>";

                _environmentVariablesDictionary.TryGetValue(Constants.TEST_HTTPCLIENT_RESOLVE_URL_VARIABLE, out httpClientUrlToResolve);

                if (!string.IsNullOrWhiteSpace(httpClientUrlToResolve))
                {
                    output += "<table class=\"table\">";
                    string testResult = "";
                    try
                    {
                        var content = await _httpClient.GetStringAsync(httpClientUrlToResolve);
                        testResult = $"<span class=\"badge text-bg-success\">OK</span><br> {HttpUtility.HtmlEncode(content)}";
                    }
                    catch (Exception e)
                    {
                        testResult = "<span class=\"badge text-bg-danger\">QUERY FAILED</span><br> " + e.InnerException.Message;
                    }
                    finally
                    {
                        output += HtmlBrandingHelper.GetStandardTableRow($"{httpClientUrlToResolve}", testResult);
                    }

                    output += "</table>";
                }


                if (!string.IsNullOrWhiteSpace(Constants.MY_IP_ADDRESS_EXTERNAL_SERVICE_URL))
                {

                    output += "<table class=\"table\">";
                    string testResult = "";
                    try
                    {
                        var content = await _httpClient.GetStringAsync(Constants.MY_IP_ADDRESS_EXTERNAL_SERVICE_URL);
                        testResult = $"<span class=\"badge text-bg-success\">OK</span><br> {HttpUtility.HtmlEncode(content)}";
                    }
                    catch (Exception e)
                    {
                        testResult = "<span class=\"badge text-bg-danger\">QUERY FAILED</span><br> " + e.InnerException.Message;
                    }
                    finally
                    {
                        output += HtmlBrandingHelper.GetStandardTableRow($"My outbound IP address<br><small>by external service</small>", testResult);
                    }

                    output += "</table>";
                }

            }




            return output;
        }
    }
}
