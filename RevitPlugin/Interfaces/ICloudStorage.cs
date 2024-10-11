using RevitPlugin.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RevitPlugin.Interfaces
{
    public interface ICloudStorage
    {
        Task InitializeAsync();
        Task<ObservableCollection<CloudFile>> GetAllElementsAsync();
        Task UploadAsync(string filePath);
        Task DeleteAsync<T>(T id);
        Task DownloadAsync(CloudFile cloudFile, string filePath);
    }
}
