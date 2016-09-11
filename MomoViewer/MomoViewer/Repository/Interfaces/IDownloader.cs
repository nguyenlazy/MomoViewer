using System.Threading.Tasks;
using Windows.Storage;

namespace MomoViewer.Repository.Interfaces
{
    public interface IDownloader
    {
        Task<StorageFile> Download(string uri);
    }
}
