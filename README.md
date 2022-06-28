# Azure Functions Environment Debugger

<img src="docs/intro.gif" alt="Tool screeshots">

## Description

This .NET tool is for Cloud Solution Architects and Cloud Engineers to debug Azure Functions applications, 
deployed in large complex (typically corporate) environments, with firewalls, network security groups, App Service Environments and other tool.

## License

The tool is released under the MIT License, see LICENSE file.

## Usage

Just install this codebase as package to given Azure FunctionApp.

To control tool set Configuration variables:<br>
`TEST_DNS_RESOLVE_DOMAINS` = <em>`<<comma-delimited list of domains>>`</em> - performs resolutions of given domains via all available DNS servers<br>
<em>e.g. `TEST_DNS_RESOLVE_DOMAINS` = `azure.com,someonpremresource.contoso.internal,vjirovsky.cz` </em>

`TEST_HTTPCLIENT_GET_URL` = <em>`<<url to test>>`</em> - performs HTTP request to given URL via same outbound connectivity configuration as the real application will use<br>
<em> e.g.  `TEST_DNS_RESOLVE_DOMAINS` = `https://vjirovsky.cz` </em>


## Issues

Please report any issue into [GitHub issues](https://github.com/vjirovsky/AzFunDebugger/issues).
