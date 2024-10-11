using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using RevitPlugin.Interfaces;
using RevitPlugin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RevitPlugin.Services
{
    internal class GoogleDriveService : ICloudStorage
    {
        // This is not secure and is intended solely for demonstration purposes
        private const string _token = "{\"installed\":{\"client_id\":\"397248792904-dg5v3imgh7s20buhu39tqqudeutdls17.apps.googleusercontent.com\",\"project_id\":\"rdstudioplugin-437313\",\"auth_uri\":\"https://accounts.google.com/o/oauth2/auth\",\"token_uri\":\"https://oauth2.googleapis.com/token\",\"auth_provider_x509_cert_url\":\"https://www.googleapis.com/oauth2/v1/certs\",\"client_secret\":\"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\",\"redirect_uris\":[\"http://localhost\"]}}";
        private UserCredential credentials;
        private DriveService driveService;

        public string[] Scopes { get; private set; } = new string[]
        {
            DriveService.Scope.Drive
        };
        public List<string> FilesName { get; private set; } = new List<string>();

        public GoogleDriveService()
        {
            _ = InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            await GetCredentialAsync();

            if (credentials != null)
            {
                driveService = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credentials,
                    ApplicationName = "RDStudioPlugin",
                });
            }
        }

        public async Task DeleteAsync<T>(T id)
        {
            try
            {
                await driveService.Files.Delete(id.ToString()).ExecuteAsync();
                Console.WriteLine("File deleted successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }

        public async Task DownloadAsync(CloudFile cloudFile, string filePath)
        {
            try
            {
                var request = driveService.Files.Get(cloudFile.Id);
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    request.MediaDownloader.ProgressChanged += (progress) =>
                    {
                        Console.WriteLine(progress.Status + " " + progress.BytesDownloaded);
                    };
                    await request.DownloadAsync(stream);
                }
                Console.WriteLine("File downloaded successfully to " + filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
        }

        public async Task<ObservableCollection<CloudFile>> GetAllElementsAsync()
        {
            if (driveService == null)
            {
                return null;
            }

            var request = driveService.Files.List();
            var response = await request.ExecuteAsync();

            ObservableCollection<CloudFile> files = new ObservableCollection<CloudFile>();

            foreach (var file in response.Files)
            {
                files.Add(new CloudFile()
                {
                    Id = file.Id,
                    Name = file.Name
                });
            }

            return files;
        }

        public async Task UploadAsync(string filePath)
        {
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = Path.GetFileName(filePath)
                    };

                    FilesResource.CreateMediaUpload request = driveService.Files.Create(fileMetadata, stream, "application/octet-stream");
                    request.Fields = "id";

                    await request.UploadAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error uploading files: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        async Task GetCredentialAsync()
        {
            string sessionPath = Path.Combine(Path.GetTempPath(), "sessions.json");
            var fileDataStore = new FileDataStore(sessionPath, true);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(_token)))
            {
                credentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    clientSecrets: GoogleClientSecrets.FromStream(stream).Secrets,
                    scopes: Scopes,
                    user: "user",
                    taskCancellationToken: CancellationToken.None,
                    fileDataStore);
            }
        }
    }
}
