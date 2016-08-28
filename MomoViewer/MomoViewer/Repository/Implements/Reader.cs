using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using MomoViewer.Model;
using MomoViewer.Repository.Enums;
using MomoViewer.Repository.Interfaces;
using MomoViewer.Repository.ModuleManager;

namespace MomoViewer.Repository.Implements
{
    public abstract class Reader : IReader
    {
        private IExtractor _extractor;
        private IDownloader _downloader;
        private ChapterInfo _info = new ChapterInfo();
        public async Task<ChapterInfo> Read(LinkInfo Uri)
        {
            switch (Uri.Type)
            {
                case LinkType.OfflineZip:
                    _extractor = ExtractorModule.Module[MangaType.Zip];
                    break;
                case LinkType.OfflineFolder:
                    _extractor = ExtractorModule.Module[MangaType.Folder];
                    break;
                case LinkType.Online:
                    _extractor = ExtractorModule.Module[MangaType.Zip];
                    _downloader = new DRDownloader();
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



        public List<PageInfo> CreatePageInfos(List<BitmapImage> images)
        {
            List<PageInfo> infos = new List<PageInfo>();
            int pageNum = 0;
            foreach (var bitmapImage in images)
            {
                PageInfo pageInfo = new PageInfo();
                pageInfo.Image = bitmapImage;
                pageInfo.Number = pageNum++;
            }
            return infos;
        }
    }
}
