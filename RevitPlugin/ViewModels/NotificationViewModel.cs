using RevitPlugin.ViewModels.Base;

namespace RevitPlugin.ViewModels
{
    internal class NotificationViewModel : BaseViewModel
    {
        private string _message;
        private string _title;
        private bool _isOperationRunning;

        public bool IsOperationRunning
        {
            get { return _isOperationRunning; }
            set
            {
                _isOperationRunning = value;
                OnPropertyChanged(nameof(IsOperationRunning));
            }
        }
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}
