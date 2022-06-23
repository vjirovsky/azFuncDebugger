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
                GetCustomCss() +
                "</head>" +
                "<body>" +
                (isWrappedInContainer ? GetBootstrapHtmlContainerWrap(bodyContent) : bodyContent) +
                "</body>" +
                "</html>";
        }

        public static string GetCustomCss()
        {
            return "<style type='text/css'>" +
                ".callout-info {--bd-callout-bg: rgba(var(--bs-info-rgb), .075);--bd-callout-border: rgba(var(--bs-info-rgb), .5);}" +
                ".callout {padding: 1.25rem;margin-top: 1.25rem;margin-bottom: 1.25rem;background-color: var(--bd-callout-bg, var(--bs-gray-100));border-left: 0.25rem solid var(--bd-callout-border, var(--bs-gray-300));}" +
                "</style>";
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
                "<script src='https://cdn.jsdelivr.net/npm/bootstrap@5.2.0-beta1/dist/js/bootstrap.bundle.min.js' integrity='sha384-pprn3073KE6tl6bjs2QrFaJGz5/SUsLqktiwsUTF55Jfv3qYSDhgCecCxMW52nD2' crossorigin='anonymous'></script>" +
                "<link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap-icons@1.8.3/font/bootstrap-icons.css'>";

        }

        internal static string GetStandardTableRow(string key, string value)
        {
            return $"<tr><th style='width:20%;'>{key}</th><td style='width:80%;'>{value}</td></tr>";
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
            return $"<div class='tab-pane fade" + (isActive ? " show active" : "") + $"' id='main-tab{id}-body' role='tabpanel' tabindex='0'><br>{text}</div>";
        }


        internal static string GetBootstrapWhatItMeans(string uniqueId, string text, bool isActive = false)
        {
            return $"<p><small><a data-bs-toggle='collapse' href='#whatItMeans{uniqueId}' role='button'>What it means <i class='bi bi-patch-question-fill'></i></a></small></p>" +
                $"<div class='"+ (isActive ? "": "collapse") + $"' id='whatItMeans{uniqueId}'><div class='callout callout-info'>{text}</div></div>";
        }



    }
}
