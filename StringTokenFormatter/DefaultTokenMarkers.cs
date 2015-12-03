using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StringTokenFormatter
{
    public class DefaultTokenMarkers : TokenMarkers
    {
        public string StartToken
        {
            get { return "{"; }
        }

        public string EndToken
        {
            get { return "}"; }
        }

        public string StartTokenEscaped
        {
            get { return this.StartToken + this.StartToken; }
        }

        public IEqualityComparer<string> TokenNameComparer
        {
            get { return StringComparer.InvariantCultureIgnoreCase; }
        }
    }
}
