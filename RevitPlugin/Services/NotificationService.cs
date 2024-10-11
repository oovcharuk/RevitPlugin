using RevitPlugin.ViewModels;
using RevitPlugin.Views;
using System;
using System.Threading.Tasks;

namespace RevitPlugin.Services
{
    internal class NotificationService
    {
        private NotificationWindow _notificationView;
        private NotificationViewModel _notificationViewModel;

        public NotificationService(string title)
        {
            _notificationView = new NotificationWindow();
            _notificationViewModel = new NotificationViewModel();
            _notificationView.DataContext = _notificationViewModel;
            _notificationViewModel.Title = title;
        }

        public void Show(string initialMessage)
        {
            _notificationViewModel.Message = initialMessage;
            _notificationViewModel.IsOperationRunning = true;
            _notificationView.Dispatcher.BeginInvoke(new Action(() =>
            {
                bool? result = _notificationView.ShowDialog();
            }));
        }

        public void UpdateStatus(string status)
        {
            _notificationViewModel.Message = status;
            _notificationViewModel.IsOperationRunning = false;
        }

        public void Close()
        {
            _notificationView?.Close();
            _notificationView = null;
        }

        public async Task ExecuteWithNotificationAsync(Func<Task> operation, string operationStatus)
        {
            try
            {
                Show(operationStatus);
                await operation.Invoke();
                UpdateStatus("Operation completed successfully.");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error: {ex.Message}");
            }
        }
    }
}

