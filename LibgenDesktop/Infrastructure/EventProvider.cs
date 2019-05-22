using System.Collections.Generic;
using System.Linq;

namespace LibgenDesktop.Infrastructure
{
    public class EventProvider
    {
        private readonly Queue<ViewModelEvent> eventQueue;
        private IEventListener eventListener;

        public EventProvider()
        {
            eventQueue = new Queue<ViewModelEvent>();
            eventListener = null;
        }

        public void SetEventListener(IEventListener eventListener)
        {
            this.eventListener = eventListener;
            PassEvents();
        }

        public void RaiseEvent(ViewModelEvent viewModelEvent)
        {
            eventQueue.Enqueue(viewModelEvent);
            PassEvents();
        }

        public void RaiseEvent(ViewModelEvent.RegisteredEventId eventId)
        {
            RaiseEvent(new ViewModelEvent(eventId));
        }

        private void PassEvents()
        {
            if (eventListener != null)
            {
                while (eventQueue.Any())
                {
                    ViewModelEvent viewModelEvent = eventQueue.Dequeue();
                    eventListener.OnViewModelEvent(viewModelEvent);
                }
            }
        }
    }
}
