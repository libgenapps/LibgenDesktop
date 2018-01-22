using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.ViewModels
{
    internal abstract class LibgenWindowViewModel : ViewModel
    {
        protected IWindowContext CurrentWindowContext
        {
            get
            {
                return WindowManager.GetWindowContext(this);
            }
        }
    }
}
