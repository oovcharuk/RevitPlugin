using RevitPlugin.Models;
using RevitPlugin.ViewModels.Base;

namespace RevitPlugin.ViewModels
{
    public class SelectableCloudFileViewModel : BaseViewModel
    {
        public CloudFile CloudFile { get; set; }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isChecked;
    }
}
