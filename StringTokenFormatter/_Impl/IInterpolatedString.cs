﻿
using System.Collections.Generic;

namespace StringTokenFormatter.Impl {
    public interface IInterpolatedString : IEnumerable<IInterpolatedStringSegment> {
        string FormatContainer(
            ITokenValueContainer container, 
            ITokenValueConverter valueConverter, 
            ITokenValueFormatter valueFormatter
            );

    }

}