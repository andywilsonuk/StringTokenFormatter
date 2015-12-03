using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringTokenFormatter
{
    public class StringFormatBuilder
    {
        private StringBuilder builder = new StringBuilder();
        private TokenMarkers markers;

        public StringFormatBuilder(TokenMarkers markers)
        {
            this.markers = markers;
        }

        public void Append(string value)
        {
            value = value.Replace(this.markers.StartTokenEscaped, this.markers.StartToken);
            value = value.Replace("{{", "{");
            value = value.Replace("}}", "}");
            value = value.Replace("{", "{{");
            value = value.Replace("}", "}}");
            this.builder.Append(value);
        }

        public void AppendToken(string token)
        {
            this.builder.Append("{" + token + "}");
        }

        public override string ToString()
        {
            return this.builder.ToString();
        }
    }
}
