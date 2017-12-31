using System.Windows;
using LibgenDesktop.Infrastructure;

namespace LibgenDesktop.Views.Utils
{
    public static class EventListenerExtensions
    {
        public static readonly DependencyProperty EventProviderProperty = DependencyProperty.RegisterAttached("EventProvider", typeof(EventProvider), typeof(EventListenerExtensions), new PropertyMetadata(EventProviderChanged));

        public static EventProvider GetEventProvider(DependencyObject element)
        {
            return (EventProvider)element.GetValue(EventProviderProperty);
        }

        public static void SetEventProvider(DependencyObject element, EventProvider value)
        {
            element.SetValue(EventProviderProperty, value);
        }

        private static void EventProviderChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is EventProvider oldEventProvider)
            {
                oldEventProvider.SetEventListener(null);
            }
            if (e.NewValue is EventProvider newEventProvider)
            {
                newEventProvider.SetEventListener(dependencyObject as IEventListener);
            }
        }
    }
}
