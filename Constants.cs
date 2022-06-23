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

        public const string APPSERVICE_DNS_SERVER_RESERVED_PRIVATE_DNS = "168.63.129.16";

        public const string TEST_DNS_RESOLVE_DOMAIN_VARIABLE = "TEST_DNS_RESOLVE_DOMAIN";
        public const string TEST_HTTPCLIENT_RESOLVE_URL_VARIABLE = "TEST_HTTPCLIENT_GET_URL";
        
        public const string MY_IP_ADDRESS_EXTERNAL_SERVICE_URL = "https://api.ipify.org/";
    }
}
