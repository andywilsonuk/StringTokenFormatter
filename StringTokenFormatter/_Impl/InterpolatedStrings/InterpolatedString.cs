using StringTokenFormatter.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace StringTokenFormatter.Impl.InterpolatedStrings {

    [DebuggerDisplay(Debugger2.GetDebuggerDisplay)]
    public class InterpolatedString : IInterpolatedString, IGetDebuggerDisplay {
        protected IInterpolatedStringSegment[] Segments { get; }

        public InterpolatedString(IEnumerable<IInterpolatedStringSegment> Segments) {
            this.Segments = Segments.ToArray();
        }

        public string FormatContainer(ITokenValueContainer container, ITokenValueConverter ValueConverter, ITokenValueFormatter ValueFormatter) {

            var sb = new StringBuilder();
            
            foreach (var segment in Segments) {
                sb.Append(segment.Evaluate(container, ValueConverter, ValueFormatter));
            }

            return sb.ToString();
        }

        public IEnumerator<IInterpolatedStringSegment> GetEnumerator() {
            foreach (var item in Segments) {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        internal string GetDebuggerDisplay() {
            var sb = new StringBuilder();
            foreach (var segment in Segments) {
                sb.Append(segment.GetDebuggerDisplay());
            }

            return sb.ToString();
        }

        string IGetDebuggerDisplay.GetDebuggerDisplay() {
            return GetDebuggerDisplay();
        }
    }

}