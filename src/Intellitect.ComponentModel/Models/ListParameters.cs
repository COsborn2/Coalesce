﻿using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intellitect.ComponentModel.Models
{
    public class ListParameters
    {
        public string Where { get; set; }
        public string Include { get; set; }
        public string Includes { get; set; }
        public string OrderBy { get; set; }
        public string OrderByDescending { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        /// <summary>
        /// Text to search fields for.
        /// </summary>
        public string Search { get; set; }
        /// <summary>
        /// Data source to use for the list.
        /// </summary>
        public string ListDataSource { get; set; }
        /// <summary>
        /// CSV list of fields to return
        /// </summary>
        public string Fields { get; set; }

        /// <summary>
        /// Calculated list from Fields
        /// </summary>
        public List<string> FieldList
        {
            get
            {
                if (Fields == null) return new List<string>();
                return Fields.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }
        /// <summary>
        /// List of filters added from the controller based on property=value on the URL.
        /// </summary>
        public Dictionary<string, string> Filters { get; }
        /// <summary>
        /// Calculated list from Include CSV
        /// </summary>
        public List<string> IncludeList {
            get
            {
                if (Include == null) return new List<string>();
                return Include.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
        }


        public ListParameters(string fields = null, string include = null,
            string includes = null, string orderBy = null, string orderByDescending = null,
            int? page = null, int? pageSize = null, string where = null,
            string listDataSource = null, string search = null)
        {
            Fields = fields;
            Include = include;
            Includes = includes;
            OrderBy = orderBy;
            OrderByDescending = orderByDescending;
            Page = page;
            PageSize = pageSize;
            Where = where;
            ListDataSource = listDataSource;
            Search = search;
            Filters = new Dictionary<string, string>();
        }

        /// <summary>
        /// List of OrderBy clauses keyed by column and with a value of Asc or Desc.
        /// </summary>
        public Dictionary<string, string> OrderByList
        {
            get
            {
                var result = new Dictionary<string, string>();
                // Add order by
                if (OrderBy != null)
                {
                    foreach (var orderBy in OrderBy.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var parts = orderBy.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2 && parts[1].ToUpper().StartsWith("D"))
                        {
                            result.Add(parts[0], "Desc");
                        }
                        else {
                            result.Add(parts[0], "Asc");
                        }
                    }
                }
                // Add order by descending
                if (OrderByDescending != null)
                {
                    foreach (var orderBy in OrderBy.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        result.Add(orderBy, "Desc");
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a name value condition to the Filters list.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void AddFilter(string propertyName, string propertyValue)
        {
            if (propertyValue != null)
            {
                Filters.Add(propertyName, propertyValue);
            }
        }
    }
}