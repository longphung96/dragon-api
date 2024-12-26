// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace DragonAPI.Extensions
{
    /// <summary>
    /// Enable HTTP response caching.
    /// </summary>
    public static class HttpRequestCacheKeyBuilder
    {
        private const string KeyDelimiter = "&&";
        // Use the unit separator for delimiting subcomponents of the cache key to avoid possible collisions
        private const string KeySubDelimiter = "__";

        public static string BuildKey(HttpContext context, string keyGroup)
        {
            var builder = new StringBuilder();
            builder.Append(keyGroup);
            var queryParamKey = CreateParamsKeyValues(context);
            if (!string.IsNullOrEmpty(queryParamKey) && !string.IsNullOrWhiteSpace(queryParamKey))
            {
                builder.Append($"{queryParamKey}");
            }
            return builder.ToString();
        }

        public static string CreateParamsKeyValues(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var request = context.Request;
            var builder = new StringBuilder();

            try
            {
                // Vary by query keys
                if (context.Request.Query.Count > 0)
                {
                    // Append a group separator for the query key segment of the cache key
                    builder
                        .Append(":")
                        .Append('Q');

                    var keys = context.Request.Query.Keys.ToArray();
                    for (var i = 0; i < keys.Count(); i++)
                    {
                        var queryKey = keys[i] ?? string.Empty;
                        var queryKeyValues = context.Request.Query[queryKey];
                        builder.Append(KeyDelimiter)
                            .Append(queryKey)
                            .Append('=');

                        var queryValueArray = queryKeyValues.ToArray();
                        Array.Sort(queryValueArray, StringComparer.Ordinal);

                        for (var j = 0; j < queryValueArray.Length; j++)
                        {
                            if (j > 0)
                            {
                                builder.Append(KeySubDelimiter);
                            }

                            builder.Append(queryValueArray[j]);
                        }
                    }
                }

                return builder.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}


