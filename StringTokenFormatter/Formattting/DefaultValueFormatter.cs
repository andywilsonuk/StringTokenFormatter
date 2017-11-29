﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class DefaultValueFormatter : IValueFormatter
    {
        private readonly IFormatProvider provider;

        public DefaultValueFormatter()
        {
        }

        public DefaultValueFormatter(IFormatProvider formatProvider)
        {
            provider = formatProvider;
        }

        public string Format(TokenMatchingSegment token, object value)
        {
            if (value == null) return null;
            string padding = string.IsNullOrEmpty(token.Padding) ? "0" : token.Padding;
            string format = $"{{0,{padding}:{token.Format}}}";
            return string.Format(provider, format, value);
        }
    }
}
