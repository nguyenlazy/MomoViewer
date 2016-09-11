using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Practices.Unity;
using MomoViewer.Model;
using MomoViewer.Repository.Enums;
using MomoViewer.Repository.Interfaces;

namespace MomoViewer.Repository.Implements
{
    public class Reader : IReader
    {
        private IExtractor _extractor;
        private IDownloader _downloader;
        private ChapterInfo _info = new ChapterInfo();
        public async Task<ChapterInfo> Read(LinkInfo Uri)
        {
            switch (Uri.Type)
            {
                case LinkType.OfflineZip:
                    _extractor = MainPageVM.DiContainer.Resolve<IExtractor>("zipExtractor");
                    //   _extractor = DIContainer.GetModule<ZipExtractor>();
                    break;
                case LinkType.OfflineFolder:
                    _extractor = MainPageVM.DiContainer.Resolve<IExtractor>("folderExtractor");
                    break;
                case LinkType.Online:
                    _extractor = MainPageVM.DiContainer.Resolve<IExtractor>("zipExtractor");
                    _downloader = MainPageVM.DiContainer.Resolve<IDownloader>();
                    var file = await _downloader.Download(Uri.Path);
                    _info.Pages = CreatePageInfos(await _extractor.Extract(file.Path));
                    return _info;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var images = await _extractor.Extract(Uri.Path);
            _info.Pages = CreatePageInfos(images);

            return _info;
        }


        public Reader(IDownloader downloader)
        {
            _downloader = downloader;
        }


        private List<PageInfo> CreatePageInfos(List<BitmapImage> images)
        {
            List<PageInfo> infos = new List<PageInfo>();
            int pageNum = 0;
            foreach (var bitmapImage in images)
            {
                PageInfo pageInfo = new PageInfo();
                pageInfo.Image = bitmapImage;
                pageInfo.Number = pageNum++;

                infos.Add(pageInfo);
            }
            return infos;
        }
    }
}
