using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzFappDebugger
{
    public class Constants
    {

        public const string APPSERVICE_DNS_SERVER_VARIABLE = "WEBSITE_DNS_SERVER";
        public const string APPSERVICE_DNS_ALT_SERVER_VARIABLE = "WEBSITE_DNS_ALT_SERVER";
        public const string APPSERVICE_VNET_ROUTEALL_VARIABLE = "WEBSITE_VNET_ROUTE_ALL";
        public const string APPSERVICE_WEBSITE_HOSTNAME_VARIABLE = "WEBSITE_HOSTNAME";
        public const string APPSERVICE_WEBSITE_SITE_NAME_VARIABLE = "WEBSITE_SITE_NAME";
        public const string APPSERVICE_WEBSITE_OS_VARIABLE = "WEBSITE_OS";
        public const string APPSERVICE_WEBSITE_RESOURCE_GROUP_VARIABLE = "WEBSITE_RESOURCE_GROUP";
        public const string APPSERVICE_WEBSITE_SKU_VARIABLE = "WEBSITE_SKU";
        public const string APPSERVICE_WEBSITE_RUN_FROM_PACKAGE_VARIABLE = "WEBSITE_RUN_FROM_PACKAGE";
        public const string APPSERVICE_WEBSITE_CONTENTAZUREFILECONNECTIONSTRING_VARIABLE = "WEBSITE_CONTENTAZUREFILECONNECTIONSTRING";
        public const string APPSERVICE_WEBSITE_CONTENTSHARE_VARIABLE = "WEBSITE_CONTENTSHARE";



        public const string APPSERVICE_FUNCTIONS_WORKER_RUNTIME_VARIABLE = "FUNCTIONS_WORKER_RUNTIME";
        

        public const string APPSERVICE_DNS_SERVER_RESERVED_PRIVATE_DNS = "168.63.129.16";

        public const string TEST_DNS_RESOLVE_DOMAIN_VARIABLE = "TEST_DNS_RESOLVE_DOMAIN";
        public const string TEST_HTTPCLIENT_RESOLVE_URL_VARIABLE = "TEST_HTTPCLIENT_GET_URL";
        
        public const string MY_IP_ADDRESS_EXTERNAL_SERVICE_URL = "https://api.ipify.org/";


        public static string GetEnvironmentVariableIfSet(IDictionary<string, string> environmentVariablesDictionary, string variableName) {

            if (environmentVariablesDictionary.ContainsKey(variableName))
            {
                return environmentVariablesDictionary[variableName];
            }

            return string.Empty;

        }
    }
}
