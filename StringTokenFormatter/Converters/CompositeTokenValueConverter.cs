using System;
using System.Collections.Generic;
using System.Linq;

namespace StringTokenFormatter {
    /// <summary>
    ///  Loops through all child converters until it finds one that applies to the current value.
    /// </summary>
    public class CompositeTokenValueConverter : ITokenValueConverter {
        private readonly IEnumerable<ITokenValueConverter> converters;

        public CompositeTokenValueConverter(IEnumerable<ITokenValueConverter> converters) {
            this.converters = converters ?? throw new ArgumentNullException(nameof(converters));
        }

        public bool TryConvert(IMatchedToken matchedToken, object? value, out object? mapped) {
            foreach (var converter in converters) {
                if (converter.TryConvert(matchedToken, value, out mapped)) return true;
            }
            mapped = null;
            return false;
        }
    }

}
