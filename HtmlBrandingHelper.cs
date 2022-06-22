using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzFappDebugger
{
    internal static class HtmlBrandingHelper
    {
        public static string RenderPage(string bodyContent, string pageTitle = "", bool isWrappedInContainer = true)
        {
            return "<html>" +
                "<head>" +
                $"<title>{pageTitle}</title>" +
                GetBootstrapHtmlHeadContent() +
                "</head>" +
                "<body>" +
                (isWrappedInContainer ? GetBootstrapHtmlContainerWrap(bodyContent) : bodyContent) +
                "</body>" +
                "</html>";
        }

        public static string GetBootstrapHtmlContainerWrap(string content)
        {
            return $"<div class=\"container\">" +
                content +
                $"</div>";

        }

        public static string GetBootstrapHtmlHeadContent()
        {
            return "<link href='https://cdn.jsdelivr.net/npm/bootstrap@5.2.0-beta1/dist/css/bootstrap.min.css' rel='stylesheet' integrity='sha384-0evHe/X+R7YkIZDRvuzKMRqM+OrBnVFBL6DOitfPri4tjfHxaWutUpFmBp4vmVor' crossorigin='anonymous'>" +
                "<script src='https://cdn.jsdelivr.net/npm/bootstrap@5.2.0-beta1/dist/js/bootstrap.bundle.min.js' integrity='sha384-pprn3073KE6tl6bjs2QrFaJGz5/SUsLqktiwsUTF55Jfv3qYSDhgCecCxMW52nD2' crossorigin='anonymous'></script>";

        }

        internal static string GetStandardTableRow(string key, string value)
        {
            return $"<tr><th>{key}</th><td>{value}</td></tr>";
        }
        internal static string GetColspanTableRow(string value, int colspan = 2)
        {
            return $"<tr><td colspan=\"{colspan}\">{value}</td></tr>";
        }



        internal static string GetBootstrapTabNavItem(int id, string text, bool isActive = false)
        {
            return $"<li class='nav-item'><button class='nav-link" + (isActive ? " active" : "") + $"' id='main-tab{id}' data-bs-toggle='tab' data-bs-target='#main-tab{id}-body' type='button' role='tab'>{text}</button></li>";
        }
        internal static string GetBootstrapTabBody(int id, string text, bool isActive = false)
        {
            return $"<div class='tab-pane fade" + (isActive ? " show active" : "") + $"' id='main-tab{id}-body' role='tabpanel' tabindex='0'>{text}</div>";
        }
    }
}
