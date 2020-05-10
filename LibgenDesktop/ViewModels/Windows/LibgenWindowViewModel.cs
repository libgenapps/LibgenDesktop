using LibgenDesktop.Infrastructure;
using LibgenDesktop.Models;

namespace LibgenDesktop.ViewModels.Windows
{
    internal abstract class LibgenWindowViewModel : ContainerViewModel
    {
        public LibgenWindowViewModel(MainModel mainModel)
            : base(mainModel)
        {
        }

        protected IWindowContext CurrentWindowContext
        {
            get
            {
                return WindowManager.GetWindowContext(this);
            }
        }

        protected void ShowMessage(string title, string text)
        {
            ShowMessage(title, text, CurrentWindowContext);
        }

        protected bool ShowPrompt(string title, string text)
        {
            return ShowPrompt(title, text, CurrentWindowContext);
        }
    }
}
