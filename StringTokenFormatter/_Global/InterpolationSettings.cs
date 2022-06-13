namespace StringTokenFormatter {
    public static class InterpolationSettings {
        public static IInterpolationSettings Default { get; }
        public static InterpolationSettingsBuilder DefaultBuilder { get; }


        static InterpolationSettings() {

            DefaultBuilder = new();

            Default = DefaultBuilder.Build();
        }
    }

}
