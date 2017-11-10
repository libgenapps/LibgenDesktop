using System.Runtime.Serialization;
using System.Windows.Forms;

namespace LibgenDesktop.Settings
{
    [DataContract]
    internal class AppSettings
    {
        [DataContract]
        internal class WindowSettings
        {
            [DataMember]
            public bool Maximized { get; set; }

            [DataMember]
            public int Left { get; set; }

            [DataMember]
            public int Top { get; set; }

            [DataMember]
            public int Width { get; set; }

            [DataMember]
            public int Height { get; set; }
        }

        private const int DEFAULT_MAIN_WINDOW_WIDTH = 1200;
        private const int DEFAULT_MAIN_WINDOW_HEIGHT = 650;

        public static AppSettings Default
        {
            get
            {
                return new AppSettings
                {
                    DatabaseFileName = "libgen.db",
                    OfflineMode = true,
                    // ResultLimit = 0,
                    Window = new WindowSettings
                    {
                        Maximized = false,
                        Left = (Screen.PrimaryScreen.WorkingArea.Width - DEFAULT_MAIN_WINDOW_WIDTH) / 2,
                        Top = (Screen.PrimaryScreen.WorkingArea.Height - DEFAULT_MAIN_WINDOW_HEIGHT) / 2,
                        Width = DEFAULT_MAIN_WINDOW_WIDTH,
                        Height = DEFAULT_MAIN_WINDOW_HEIGHT
                    }
                };
            }
        }

        [DataMember]
        public string DatabaseFileName { get; set; }

        [DataMember]
        public bool OfflineMode { get; set; }

        // [DataMember]
        // public int ResultLimit { get; set; }

        [DataMember]
        public WindowSettings Window { get; set; }
    }
}
