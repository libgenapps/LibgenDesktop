namespace LibgenDesktop.Infrastructure
{
    public interface IEventListener
    {
        void OnViewModelEvent(ViewModelEvent viewModelEvent);
    }
}
