namespace LibgenDesktop.Models.ProgressArgs
{
    internal class GenericProgress
    {
        internal enum Event
        {
            SCAN_CREATING_INDEXES
        }

        public GenericProgress(Event progressEvent)
        {
            ProgressEvent = progressEvent;
        }

        public Event ProgressEvent { get; }
    }
}
