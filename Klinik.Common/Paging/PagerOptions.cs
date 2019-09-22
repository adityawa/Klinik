﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Ajax;
using System.Web.Routing;

namespace Klinik.Common.Paging
{
    public class PagerOptions
    {
        public static class DefaultDefaults
        {
            public const int MaxNrOfPages = 10;
            public const string DisplayTemplate = null;
            public const bool AlwaysAddFirstPageNumber = false;
            public const string DefaultPageRouteValueKey = "page";
            public const string PreviousPageText = "«";
            public const string PreviousPageTitle = "Previous page";
            public const string NextPageText = "»";
            public const string NextPageTitle = "Next page";
            public const string FirstPageText = "<";
            public const string FirstPageTitle = "First page";
            public const string LastPageText = ">";
            public const string LastPageTitle = "Last page";
            public const bool DisplayFirstPage = false;
            public const bool DisplayLastPage = false;
            public const bool HideFirstPageNumber = false;
            public const bool HideLastPageNumber = false;
            public const bool UseItemCountAsPageCount = false;
            public static bool HidePreviousAndNextPage = false;
            public const string CustomRouteName = null;
        }

        /// <summary>
        /// The static Defaults class allows you to set Pager defaults for the entire application.
        /// Set values at application startup.
        /// </summary>
        public static class Defaults
        {
            public static int MaxNrOfPages = DefaultDefaults.MaxNrOfPages;
            public static string DisplayTemplate = DefaultDefaults.DisplayTemplate;
            public static bool AlwaysAddFirstPageNumber = DefaultDefaults.AlwaysAddFirstPageNumber;
            public static string DefaultPageRouteValueKey = DefaultDefaults.DefaultPageRouteValueKey;
            public static string PreviousPageText = DefaultDefaults.PreviousPageText;
            public static string PreviousPageTitle = DefaultDefaults.PreviousPageTitle;
            public static string NextPageText = DefaultDefaults.NextPageText;
            public static string NextPageTitle = DefaultDefaults.NextPageTitle;
            public static string FirstPageText = DefaultDefaults.FirstPageText;
            public static string FirstPageTitle = DefaultDefaults.FirstPageTitle;
            public static string LastPageText = DefaultDefaults.LastPageText;
            public static string LastPageTitle = DefaultDefaults.LastPageTitle;
            public static bool DisplayFirstPage = DefaultDefaults.DisplayFirstPage;
            public static bool DisplayLastPage = DefaultDefaults.DisplayLastPage;
            public static bool HideFirstPageNumber = DefaultDefaults.HideFirstPageNumber;
            public static bool HideLastPageNumber = DefaultDefaults.HideLastPageNumber;
            public static bool UseItemCountAsPageCount = DefaultDefaults.UseItemCountAsPageCount;
            public static bool HidePreviousAndNextPage = DefaultDefaults.HidePreviousAndNextPage;
            public static string CustomRouteName = DefaultDefaults.CustomRouteName;

            public static void Reset()
            {
                MaxNrOfPages = DefaultDefaults.MaxNrOfPages;
                DisplayTemplate = DefaultDefaults.DisplayTemplate;
                AlwaysAddFirstPageNumber = DefaultDefaults.AlwaysAddFirstPageNumber;
                DefaultPageRouteValueKey = DefaultDefaults.DefaultPageRouteValueKey;
                PreviousPageText = DefaultDefaults.PreviousPageText;
                PreviousPageTitle = DefaultDefaults.PreviousPageTitle;
                NextPageText = DefaultDefaults.NextPageText;
                NextPageTitle = DefaultDefaults.NextPageTitle;
                FirstPageText = DefaultDefaults.FirstPageText;
                FirstPageTitle = DefaultDefaults.FirstPageTitle;
                LastPageText = DefaultDefaults.LastPageText;
                LastPageTitle = DefaultDefaults.LastPageTitle;
                DisplayFirstPage = DefaultDefaults.DisplayFirstPage;
                DisplayLastPage = DefaultDefaults.DisplayLastPage;
                HideFirstPageNumber = DefaultDefaults.HideFirstPageNumber;
                HideLastPageNumber = DefaultDefaults.HideLastPageNumber;
                UseItemCountAsPageCount = DefaultDefaults.UseItemCountAsPageCount;
                HidePreviousAndNextPage = DefaultDefaults.HidePreviousAndNextPage;
                CustomRouteName = DefaultDefaults.CustomRouteName;
            }
        }

        public RouteValueDictionary RouteValues { get; internal set; }

        public string DisplayTemplate { get; internal set; }

        public int MaxNrOfPages { get; internal set; }

        public AjaxOptions AjaxOptions { get; internal set; }

        public bool AlwaysAddFirstPageNumber { get; internal set; }

        public string Action { get; internal set; }

        public string Controller { get; internal set; }

        public string PageRouteValueKey { get; set; }

        public string PreviousPageText { get; set; }

        public string PreviousPageTitle { get; set; }

        public string NextPageText { get; set; }

        public string NextPageTitle { get; set; }

        public string FirstPageText { get; set; }

        public string FirstPageTitle { get; set; }

        public string LastPageText { get; set; }

        public string LastPageTitle { get; set; }

        public bool DisplayFirstAndLastPage { get { return DisplayFirstPage && DisplayLastPage; } }

        public bool DisplayFirstPage { get; set; }

        public bool DisplayLastPage { get; set; }

        public bool HideFirstPageNumber { get; set; }

        public bool HideLastPageNumber { get; set; }

        public bool HidePreviousAndNextPage { get; internal set; }

        public bool UseItemCountAsPageCount { get; internal set; }

        public string CustomRouteName { get; set; }

        public PagerOptions()
        {
            RouteValues = new RouteValueDictionary();
            DisplayTemplate = Defaults.DisplayTemplate;
            MaxNrOfPages = Defaults.MaxNrOfPages;
            AlwaysAddFirstPageNumber = Defaults.AlwaysAddFirstPageNumber;
            PageRouteValueKey = Defaults.DefaultPageRouteValueKey;
            PreviousPageText = Defaults.PreviousPageText;
            PreviousPageTitle = Defaults.PreviousPageTitle;
            NextPageText = Defaults.NextPageText;
            NextPageTitle = Defaults.NextPageTitle;
            FirstPageText = Defaults.FirstPageText;
            FirstPageTitle = Defaults.FirstPageTitle;
            LastPageText = Defaults.LastPageText;
            LastPageTitle = Defaults.LastPageTitle;
            DisplayFirstPage = Defaults.DisplayFirstPage;
            DisplayLastPage = Defaults.DisplayLastPage;
            HideFirstPageNumber = Defaults.HideFirstPageNumber;
            HideLastPageNumber = Defaults.HideLastPageNumber;
            UseItemCountAsPageCount = Defaults.UseItemCountAsPageCount;
            HidePreviousAndNextPage = Defaults.HidePreviousAndNextPage;
            CustomRouteName = Defaults.CustomRouteName;
        }
    }

}