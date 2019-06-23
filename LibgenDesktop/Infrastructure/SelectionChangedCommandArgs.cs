using System.Collections;

namespace LibgenDesktop.Infrastructure
{
    internal class SelectionChangedCommandArgs
    {
        public SelectionChangedCommandArgs(IList addedItems, IList removedItems)
        {
            AddedItems = addedItems;
            RemovedItems = removedItems;
        }

        public IList AddedItems { get; }
        public IList RemovedItems { get; }
    }
}
