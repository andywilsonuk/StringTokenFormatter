namespace System.Diagnostics {
    internal static class IGetDebuggerDisplayExtensions {
        public static string GetDebuggerDisplay(this object This) {
            if(This is IGetDebuggerDisplay V1) {
                return V1.GetDebuggerDisplay();
            } else {
                return This.ToString() ?? string.Empty;
            }
        }
    }

}
