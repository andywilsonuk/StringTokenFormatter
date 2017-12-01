﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class TokenToFunctionObjectNoInputValueMapper : ITokenToValueMapper
    {
        public bool TryMap(IMatchedToken token, object value, out object mapped)
        {
            var func = value as Func<object>;
            if (func == null)
            {
                mapped = null;
                return false;
            }
            mapped = func();
            return true;
        }
    }
}
