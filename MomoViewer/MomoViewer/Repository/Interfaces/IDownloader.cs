using System.Threading.Tasks;
using Windows.Storage;

namespace MomoViewer.Repository.Interfaces
{
    interface IDownloader
    {
        Task<StorageFile> Download(string uri);
    }
}
