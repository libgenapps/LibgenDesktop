using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace LibgenDesktop.ViewModels.Panels
{
    internal class ImportLogPanelViewModel : ViewModel
    {
        internal class ImportLogItemViewModel : ViewModel
        {
            private string stepIndex;
            private bool isStepIndexVisible;
            private string headerText;
            private ObservableCollection<string> logLines;

            public ImportLogItemViewModel(string stepIndex, string headerText)
            {
                this.stepIndex = stepIndex;
                isStepIndexVisible = !String.IsNullOrEmpty(stepIndex);
                this.headerText = headerText;
                logLines = new ObservableCollection<string>();
            }

            public string StepIndex
            {
                get
                {
                    return stepIndex;
                }
                set
                {
                    stepIndex = value;
                    NotifyPropertyChanged();
                }
            }

            public bool IsStepIndexVisible
            {
                get
                {
                    return isStepIndexVisible;
                }
                set
                {
                    isStepIndexVisible = value;
                    NotifyPropertyChanged();
                }
            }

            public string HeaderText
            {
                get
                {
                    return headerText;
                }
                set
                {
                    headerText = value;
                    NotifyPropertyChanged();
                }
            }

            public ObservableCollection<string> LogLines
            {
                get
                {
                    return logLines;
                }
                set
                {
                    logLines = value;
                    NotifyPropertyChanged();
                }
            }

            public string LogLine
            {
                get
                {
                    return logLines.FirstOrDefault();
                }
                set
                {
                    if (!logLines.Any())
                    {
                        logLines.Add(value);
                    }
                    else
                    {
                        logLines[logLines.Count - 1] = value;
                    }
                }
            }
        }

        private string resultLogLine;
        private bool isResultLogLineVisible;
        private string errorLogLine;
        private bool isErrorLogLineVisible;

        public ImportLogPanelViewModel()
        {
            resultLogLine = String.Empty;
            isResultLogLineVisible = false;
            errorLogLine = String.Empty;
            isErrorLogLineVisible = false;
            LogItems = new ObservableCollection<ImportLogItemViewModel>();
        }

        public ObservableCollection<ImportLogItemViewModel> LogItems { get; }

        public string ResultLogLine
        {
            get
            {
                return resultLogLine;
            }
            set
            {
                resultLogLine = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsResultLogLineVisible
        {
            get
            {
                return isResultLogLineVisible;
            }
            set
            {
                isResultLogLineVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string ErrorLogLine
        {
            get
            {
                return errorLogLine;
            }
            set
            {
                errorLogLine = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsErrorLogLineVisible
        {
            get
            {
                return isErrorLogLineVisible;
            }
            set
            {
                isErrorLogLineVisible = value;
                NotifyPropertyChanged();
            }
        }

        public void AddLogItem(string stepIndex, string headerText, string logLine = null)
        {
            ImportLogItemViewModel importLogItemViewModel = new ImportLogItemViewModel(stepIndex, headerText);
            if (logLine != null)
            {
                importLogItemViewModel.LogLine = logLine;
            }
            LogItems.Add(importLogItemViewModel);
        }

        public void ShowErrorLogLine(string errorLogLine)
        {
            ErrorLogLine = errorLogLine;
            IsErrorLogLineVisible = true;
        }

        public void ShowResultLogLine(string resultLogLine)
        {
            ResultLogLine = resultLogLine;
            IsResultLogLineVisible = true;
        }
    }
}
