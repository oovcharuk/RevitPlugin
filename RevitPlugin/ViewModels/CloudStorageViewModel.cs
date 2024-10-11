using RevitPlugin.Helpers;
using RevitPlugin.Interfaces;
using RevitPlugin.Models;
using RevitPlugin.Services;
using RevitPlugin.ViewModels.Base;
using RevitPlugin.ViewModels.Commands;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace RevitPlugin.ViewModels
{
    internal class CloudStorageViewModel : BaseViewModel
    {
        private readonly ICloudStorage _cloudStorage;

        private ObservableCollection<SelectableCloudFileViewModel> _selectableCloudFiles;

        public ObservableCollection<SelectableCloudFileViewModel> SelectableCloudFiles
        {
            get => _selectableCloudFiles;
            set => SetProperty(ref _selectableCloudFiles, value);
        }

        public ICommand InitializeCommand { get; }
        public ICommand UploadCommand { get; }
        public ICommand DownloadCommand { get; }
        public ICommand DeleteCommand { get; }

        public CloudStorageViewModel(ICloudStorage cloudStarage)
        {
            _cloudStorage = cloudStarage;

            InitializeCommand = new RelayCommand<object>(async _ => await InitializeAsync());
            UploadCommand = new RelayCommand<object>(async _ => await UploadFilesAsync());
            DownloadCommand = new RelayCommand<object>(async _ => await DownloadFilesAsync());
            DeleteCommand = new RelayCommand<object>(async _ => await DeleteFilesAsync());

            _ = GetCloudFilesAsync();
        }

        private async Task InitializeAsync()
        {
            await _cloudStorage.InitializeAsync();
            await GetCloudFilesAsync();
        }

        private async Task UploadFilesAsync()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Revit files (*.rvt)|*.rvt|All files (*.*)|*.*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    NotificationService _notificationService = new NotificationService("Uploading");

                    await _notificationService.ExecuteWithNotificationAsync(async () =>
                    {
                        await _cloudStorage.UploadAsync(openFileDialog.FileName);
                    },
                    "Uploading file...");

                    await GetCloudFilesAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error uploading files: {ex.Message}");
            }
        }

        private async Task DeleteFilesAsync()
        {
            if (SelectableCloudFiles == null || !SelectableCloudFiles.Any(item => item.IsChecked))
            {
                MessageBox.Show("It is required to select at least one file.");
                return;
            }

            try
            {
                NotificationService _notificationService = new NotificationService("Deleting");

                await _notificationService.ExecuteWithNotificationAsync(async () =>
                {
                    foreach (var item in SelectableCloudFiles)
                    {
                        if (item.IsChecked)
                        {
                            await _cloudStorage.DeleteAsync(item.CloudFile.Id);
                        }
                    }
                },
                "Deleting files...");

                await GetCloudFilesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting files: {ex.Message}");
            }
        }

        private async Task DownloadFilesAsync()
        {
            if (SelectableCloudFiles == null || !SelectableCloudFiles.Any(item => item.IsChecked))
            {
                MessageBox.Show("It is required to select at least one file.");
                return;
            }

            try
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        NotificationService _notificationService = new NotificationService("Downloading");

                        await _notificationService.ExecuteWithNotificationAsync(async () =>
                        {
                            foreach (SelectableCloudFileViewModel item in SelectableCloudFiles)
                            {
                                if (item.IsChecked)
                                {
                                    var filePath = Path.Combine(fbd.SelectedPath, item.CloudFile.Name);
                                    filePath = FileHelper.GetUniqueFilePath(filePath);
                                    await _cloudStorage.DownloadAsync(item.CloudFile, filePath);
                                }
                            }
                        },
                        "Downloading files...");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading files: {ex.Message}");
            }
        }

        private async Task GetCloudFilesAsync()
        {
            try
            {
                var cloudFiles = await _cloudStorage.GetAllElementsAsync();

                if (cloudFiles == null)
                {
                    return;
                }

                var selectableFiles = cloudFiles.Select(cloudFile =>
                    new SelectableCloudFileViewModel
                    {
                        CloudFile = new CloudFile
                        {
                            Id = cloudFile.Id,
                            Name = cloudFile.Name
                        }
                    }).ToList();

                SelectableCloudFiles = new ObservableCollection<SelectableCloudFileViewModel>();

                foreach (var selectableFile in selectableFiles)
                {
                    SelectableCloudFiles.Add(selectableFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading cloud files: {ex.Message}");
            }
        }
    }
}
