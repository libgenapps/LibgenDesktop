namespace LibgenDesktop.Infrastructure
{
    public class ViewModelEvent
    {
        public enum RegisteredEventId
        {
            FOCUS_SEARCH_TEXT_BOX = 1,
            SCROLL_TO_SELECTION,
            BRING_TO_FRONT,
            FOCUS_SQL_QUERY_TEXT_BOX
        }

        public ViewModelEvent(RegisteredEventId eventId)
        {
            EventId = eventId;
        }

        public RegisteredEventId EventId { get; }
    }
}
