using System;
using System.Threading;
using System.Threading.Tasks;

namespace LibgenDesktop.Infrastructure
{
    internal abstract class ProgressOperation
    {
        private readonly CancellationTokenSource cancellationTokenSource;
        private Task task;

        public ProgressOperation()
        {
            cancellationTokenSource = new CancellationTokenSource();
            task = null;
        }

        public virtual string Title { get; }
        public virtual bool IsUnbounded => false;

        public event EventHandler<ProgressEventArgs> ProgressEvent;
        public event EventHandler<EventArgs> CompletedEvent;
        public event EventHandler<EventArgs> CancelledEvent;

        public void Start()
        {
            task = Task.Factory.StartNew(() => DoWork(cancellationTokenSource.Token), cancellationTokenSource.Token);
        }

        public void Cancel()
        {
            if (task != null)
            {
                cancellationTokenSource.Cancel();
                task.Wait();
                task = null;
                RaiseCancelledEvent();
            }
        }

        protected abstract void DoWork(CancellationToken token);

        protected virtual void RaiseProgressEvent(ProgressEventArgs progressEventArgs)
        {
            ProgressEvent?.Invoke(this, progressEventArgs);
        }

        protected virtual void RaiseCompletedEvent()
        {
            CompletedEvent?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseCancelledEvent()
        {
            CancelledEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
