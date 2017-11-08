namespace LibgenDesktop.Settings
{
    internal class AppSettings
    {
        internal class WindowSettings
        {
            public bool Maximized { get; set; }
            public int Left { get; set; }
            public int Top { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public static AppSettings Default
        {
            get
            {
                return new AppSettings
                {
                    DatabaseName = "libgen.db",
                    ResultLimit = 0
                };
            }
        }

        public string DatabaseName { get; set; }
        public int ResultLimit { get; set; }
        public WindowSettings Window { get; set; }
    }
}
