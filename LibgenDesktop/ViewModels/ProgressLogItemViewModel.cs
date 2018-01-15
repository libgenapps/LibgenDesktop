using System.Collections.ObjectModel;
using System.Linq;

namespace LibgenDesktop.ViewModels
{
    internal class ProgressLogItemViewModel : ViewModel
    {
        private string step;
        private string header;
        private ObservableCollection<string> logLines;

        public ProgressLogItemViewModel(int stepIndex, string header)
        {
            step = $"Шаг {stepIndex}";
            this.header = header;
            logLines = new ObservableCollection<string>();
        }

        public string Step
        {
            get
            {
                return step;
            }
            set
            {
                step = value;
                NotifyPropertyChanged();
            }
        }

        public string Header
        {
            get
            {
                return header;
            }
            set
            {
                header = value;
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
}
