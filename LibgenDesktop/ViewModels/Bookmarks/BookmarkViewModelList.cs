using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LibgenDesktop.Models;
using static LibgenDesktop.Models.Settings.AppSettings;

namespace LibgenDesktop.ViewModels.Bookmarks
{
    internal class BookmarkViewModelList : ObservableCollection<BookmarkViewModel>
    {
        public BookmarkViewModelList(MainModel mainModel)
        {
            List<BookmarkSettings.Bookmark> bookmarks = mainModel.AppSettings.Bookmarks.Bookmarks;
            if (bookmarks.Any())
            {
                foreach (BookmarkSettings.Bookmark bookmark in bookmarks)
                {
                    Add(new BookmarkViewModel
                    {
                        Name = bookmark.Name,
                        SearchQuery = bookmark.Query,
                        LibgenObjectType = bookmark.Type,
                        IsEnabled = true
                    });
                }
            }
            else
            {
                Add(new BookmarkViewModel
                {
                    Name = mainModel.Localization.CurrentLanguage.MainWindow.ToolbarNoBookmarks,
                    IsEnabled = false
                });
            }
        }
    }
}
