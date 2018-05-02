using LibgenDesktop.Models.Entities;

namespace LibgenDesktop.ViewModels.Bookmarks
{
    internal class BookmarkViewModel
    {
        public string Name { get; set; }
        public string SearchQuery { get; set; }
        public LibgenObjectType LibgenObjectType { get; set; }
        public bool IsEnabled { get; set; }
    }
}
