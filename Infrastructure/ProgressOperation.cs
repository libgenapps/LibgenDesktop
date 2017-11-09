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
            Failed = false;
        }

        public bool Failed { get; private set; }
        public virtual string Title { get; }
        public virtual bool IsUnbounded => false;

        public event EventHandler<ProgressEventArgs> ProgressEvent;
        public event EventHandler<EventArgs> CompletedEvent;
        public event EventHandler<EventArgs> CancelledEvent;
        public event EventHandler<ErrorEventArgs> ErrorEvent;

        public void Start()
        {
            Action taskAction = () =>
            {
                try
                {
                    DoWork(cancellationTokenSource.Token);
                }
                catch (Exception exception)
                {
                    Failed = true;
                    RaiseErrorEvent(new ErrorEventArgs
                    {
                        Exception = exception
                    });
                }
            };
            task = Task.Factory.StartNew(taskAction, cancellationTokenSource.Token);
        }

        public void Cancel()
        {
            if (task != null && !Failed)
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

        protected virtual void RaiseErrorEvent(ErrorEventArgs errorEventArgs)
        {
            ErrorEvent?.Invoke(this, errorEventArgs);
        }
    }
}
