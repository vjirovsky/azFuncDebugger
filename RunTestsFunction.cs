using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DnsClient;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using AzFappDebugger.Tests;

namespace AzFappDebugger
{
    public static class RunTestsFunction
    {
        private static HttpClient _httpClient = null;

        private static IDictionary<string, string> _environmentVariablesDictionary = new Dictionary<string, string>();



        static RunTestsFunction()
        {

            foreach (DictionaryEntry variable in Environment.GetEnvironmentVariables())
            {
                _environmentVariablesDictionary.Add((string)variable.Key, (string)variable.Value);
            }



            // ignore SSL cert errors for this debugger (e.g. DPI, proxy, etc.)
            var httpClientHandler = new HttpClientHandler();

            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>{return true;};
            _httpClient = new HttpClient(httpClientHandler);

        }


        [FunctionName("RunTests")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
            ILogger log)
        {
            string responseMessage = "";
            responseMessage += "<br><h1>" +
                "Azure Functions environment debugger tool<br>"+
                "</h1>";


            var dnsTestsProvider = new DnsTests(_environmentVariablesDictionary);
            var connectivityTestsProvider = new ConnectivityTests(_environmentVariablesDictionary, _httpClient);
            var overviewTestsProvider = new OverviewTests(_environmentVariablesDictionary);

            string dnsTestsOutput = "", connectivityTestsOutput = "", overviewTestsOutput = ""; 

            var task1 = Task.Factory.StartNew(() => dnsTestsProvider.RunAllTestsAsHtmlOutput());
            var task2 = Task.Factory.StartNew(async() =>  await connectivityTestsProvider.RunAllTestsAsHtmlOutputAsync());
            var task4 = Task.Factory.StartNew(() => overviewTestsProvider.RunAllTestsAsHtmlOutput());


            await Task.WhenAll(task1, task2, task4);
            dnsTestsOutput = task1.Result;
            connectivityTestsOutput = await task2.Result;
            overviewTestsOutput = task4.Result;


            responseMessage += "" +
                "<br><ul class='nav nav-tabs' id='myTab'>" +
                    HtmlBrandingHelper.GetBootstrapTabNavItem(0, "Overview", true) +
                    HtmlBrandingHelper.GetBootstrapTabNavItem(1, "DNS") +
                    HtmlBrandingHelper.GetBootstrapTabNavItem(2, "Outbound connectivity") +
                    HtmlBrandingHelper.GetBootstrapTabNavItem(3, "Content storage access") +
                "</ul>" +
                "<div class='tab-content' id='myTabContent'>" +
                    HtmlBrandingHelper.GetBootstrapTabBody(0, overviewTestsOutput, true) +
                    HtmlBrandingHelper.GetBootstrapTabBody(1, dnsTestsOutput) +
                    HtmlBrandingHelper.GetBootstrapTabBody(2, connectivityTestsOutput) +
                "</div>";



            return new ContentResult()
            {
                Content = HtmlBrandingHelper.RenderPage(responseMessage),
                ContentType = "text/html",
            };
        }

    }
}
