namespace LibgenDesktop.Infrastructure
{
    public class ViewModelEvent
    {
        public enum RegisteredEventId
        {
            FOCUS_SEARCH_TEXT_BOX = 1,
            SCROLL_TO_SELECTION,
            BRING_TO_FRONT
        }

        public ViewModelEvent(RegisteredEventId eventId)
        {
            EventId = eventId;
        }

        public RegisteredEventId EventId { get; }
    }
}
