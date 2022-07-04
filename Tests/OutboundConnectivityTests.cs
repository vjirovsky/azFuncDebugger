using DnsClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AzFappDebugger.Tests
{
    internal class OutboundConnectivityTests
    {
        private IDictionary<string, string> _environmentVariablesDictionary = null;
        private HttpClient _defaultHttpClient;

        internal OutboundConnectivityTests(IDictionary<string, string> environmentVariablesDictionary, HttpClient defaultHttpClient)
        {

            _environmentVariablesDictionary = environmentVariablesDictionary;
            _defaultHttpClient = defaultHttpClient;
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
            else
            {
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

            string httpClientUrlToResolve = string.Empty;

            output += $"<h4>HttpClient test</h4> <small>";
            output += HtmlBrandingHelper.GetBootstrapWhatItMeans("OutboundConnectivityHttpClient",
                $"<p>This test performs HTTP request via outbound connectivity of the application to hostname specified in <code>{Constants.TEST_HTTPCLIENT_RESOLVE_URL_VARIABLE}</code> configuration variable.</p>", false, false, "Test description");
            output += "</small>";


            _environmentVariablesDictionary.TryGetValue(Constants.TEST_HTTPCLIENT_RESOLVE_URL_VARIABLE, out httpClientUrlToResolve);

            if (!string.IsNullOrWhiteSpace(httpClientUrlToResolve) && _defaultHttpClient != null)
            {
                output += "<table class=\"table\">";
                string testResult = "";
                try
                {
                    var httpClientUrlToResolveUri = new Uri(httpClientUrlToResolve);
                    var content = await _defaultHttpClient.GetAsync(httpClientUrlToResolveUri);
                    content.EnsureSuccessStatusCode();
                    string text = await content.Content.ReadAsStringAsync();


                    testResult = $"<span class=\"badge text-bg-success\">OK</span>&nbsp; {(int)content.StatusCode} {content.StatusCode}<br><br>";


                    testResult += $"<strong>Connection details</strong><br>" +
                        $"Protocol: <code>HTTP {content.Version}</code><br>";
                    try
                    {
                        RemoteCertificateValidationCallback certCallback = (_, _, _, _) => true;
                        using var client = new TcpClient(httpClientUrlToResolveUri.Host, httpClientUrlToResolveUri.Port);
                        using var sslStream = new SslStream(client.GetStream(), true, certCallback);
                        await sslStream.AuthenticateAsClientAsync(httpClientUrlToResolveUri.Host);
                        var serverCertificate = sslStream.RemoteCertificate;
                        var certificate = new X509Certificate2(serverCertificate);

                        if (certificate != null)
                        {
                            testResult += $"Security protocol: <code>{HtmlBrandingHelper.NiceTlsProtocol(sslStream.SslProtocol)}</code><br>" +
                                $"Negotiated cipher: <code>{sslStream.NegotiatedCipherSuite}</code><br><br>";

                            testResult += $"<strong>Certificate</strong><br>" +
                                $"Subject: <code>{certificate.Subject}</code><br>" +
                                $"Issuer: <code>{certificate.Issuer}</code><br>" +
                                $"Valid from: <code>{certificate.GetEffectiveDateString()}</code><br>" +
                                $"Expires on: <code>{certificate.GetExpirationDateString()}</code><br>" +
                                $"Thumbprint: <code>{certificate.Thumbprint}</code><br><br>"
                                ;
                        }
                    }
                    catch (Exception ee)
                    {
                        testResult += $"<i>Connection is not secured</i><br><br>";

                    }

                    testResult += $"<strong>Body:</strong><br>" +
                        $"<code>{HttpUtility.HtmlEncode(HtmlBrandingHelper.NormalizeLength(text, 1000))}</code>";
                }
                catch (Exception e)
                {
                    testResult = "<span class=\"badge text-bg-danger\">QUERY FAILED</span><br> " + e.Message + "<br>" + (e.InnerException != null ? e.InnerException.Message : "");
                }
                finally
                {
                    output += HtmlBrandingHelper.GetStandardTableRow($"{httpClientUrlToResolve}", testResult);
                }


                output += "</table>";
            }
            else
            {
                output += $"<div class='callout callout-warning'>No valid <code>{Constants.TEST_HTTPCLIENT_RESOLVE_URL_VARIABLE}</code> variable defined, test has been skipped.</div>";
            }


            if (!string.IsNullOrWhiteSpace(Constants.MY_IP_ADDRESS_EXTERNAL_SERVICE_URL) && _defaultHttpClient != null)
            {
                output += $"<h4>My outbound IP address</h4> <small>";
                output += HtmlBrandingHelper.GetBootstrapWhatItMeans("OutboundConnectivityExternalIp",
                    $"<p>This test performs a HTTP request to external service, which returns a IP address of HTTP request.</p>", false, false, "Test description");
                output += "</small>";

                output += "<table class=\"table\">";
                string testResult = "";
                try
                {
                    var content = await _defaultHttpClient.GetStringAsync(Constants.MY_IP_ADDRESS_EXTERNAL_SERVICE_URL);
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

            return output;
        }
    }
}
