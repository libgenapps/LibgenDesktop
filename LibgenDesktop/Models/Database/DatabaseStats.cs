using System;

namespace LibgenDesktop.Models.Database
{
    internal class DatabaseStats
    {
        public int NonFictionBookCount;
        public DateTime? NonFictionLastUpdate;
        public int FictionBookCount;
        public DateTime? FictionLastUpdate;
        public int SciMagArticleCount;
        public DateTime? SciMagLastUpdate;
    }
}
