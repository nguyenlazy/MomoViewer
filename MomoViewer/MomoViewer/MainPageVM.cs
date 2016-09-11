using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.Unity;
using MomoViewer.Controls;
using MomoViewer.Model;
using MomoViewer.Repository;
using MomoViewer.Repository.Implements;
using MomoViewer.Repository.Interfaces;
using MomoViewer.View;
using MomoViewer.ViewModel;

namespace MomoViewer
{
    public class MainPageVM : ViewModelBase
    {
        private ICommand _openFileCommand;
        private ICommand _openFolderCommand;
        private ICommand _openLinkCommand;
        private ICommand _openRecentCommand;
        private LinkInfo _info;
        private ChapterVM _chapterVm;
        private int _totalPage;
        private FrameworkElement _view;
        public static IUnityContainer DiContainer = new UnityContainer();

        ObservableCollection<BitmapImage> images = new ObservableCollection<BitmapImage>();

        public ICommand OpenFileCommand
        {
            get { return _openFileCommand; }
            set { _openFileCommand = value; }
        }



        public ObservableCollection<BitmapImage> Images
        {
            get { return images; }
            set
            {
                images = value;
                RaisePropertyChanged();
            }
        }

        public ICommand OpenFolderCommand
        {
            get { return _openFolderCommand; }
            set { _openFolderCommand = value; }
        }

        public ICommand OpenLinkCommand
        {
            get { return _openLinkCommand; }
            set { _openLinkCommand = value; }
        }

        public object Content
        {
            get
            {
                FrameworkElement view = new ReadingView();
                view.DataContext = this;
                return view;
            }
        }

        public LinkInfo Info
        {
            get { return _info; }
            set { _info = value; }
        }

        public ChapterVM ChapterVm
        {
            get { return _chapterVm; }
            set { _chapterVm = value; }
        }

        public int TotalPage
        {
            get { return _chapterVm.TotalPages; }
            set { _chapterVm.TotalPages = value; }
        }
        public int SelectedIndex { get; set; }

        public ICommand OpenRecentCommand
        {
            get { return _openRecentCommand; }
            set { _openRecentCommand = value; }
        }

        public FrameworkElement View
        {
            get { return _view; }
            set
            {
                _view = value;
                RaisePropertyChanged();
            }
        }

        public MainPageVM()
        {
            OpenFileCommand = new RelayCommand(ExecuteOpenFile);
            OpenFolderCommand = new RelayCommand(ExecuteOpenFolder);
            OpenLinkCommand = new RelayCommand(ExecuteOpenLinkCommand);
            OpenRecentCommand = new RelayCommand(ExecuteOpenRecentCommand);
            DiContainer.RegisterType<IExtractor, ZipExtractor>("zipExtractor");
            DiContainer.RegisterType<IExtractor, FolderExtractor>("folderExtractor");
            DiContainer.RegisterType<IReader, Reader>();
            DiContainer.RegisterType<IDownloader, DRDownloader>();
            DiContainer.RegisterType<ChapterVM, ChapterVM>();
            View = new ReadingView();
            //DIContainer.SetModule<ZipExtractor, ZipExtractor>();
            //DIContainer.SetModule<FolderExtractor, FolderExtractor>();
            //DIContainer.SetModule<IDownloader, DRDownloader>();
            //DIContainer.SetModule<IReader, Reader>();
            _info = new LinkInfo();
            _chapterVm = DiContainer.Resolve<ChapterVM>();

        }

        private void ExecuteOpenRecentCommand()
        {
            View = ViewManager.GetView(ViewType.RecentView, new object());
        }


        private async void ExecuteOpenLinkCommand()
        {
            DownloadDialog dialog = new DownloadDialog();
            var res = await dialog.ShowAsync();
            switch (res)
            {
                case ContentDialogResult.None:
                    break;
                case ContentDialogResult.Primary:
                    _info.Path = dialog.Link;
                    _info.Type = LinkType.Online;
                    Images = new ObservableCollection<BitmapImage>(await ChapterVm.GetBitmapImages(_info));
                    View = ViewManager.GetView(ViewType.ReadingView, this);
                    break;
                case ContentDialogResult.Secondary:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


        }

        private async void ExecuteOpenFolder()
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add(".zip");
            var folder = await picker.PickSingleFolderAsync();
            ReadLocation(folder);



        }

        private async void ExecuteOpenFile()
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".zip");
            var file = await picker.PickSingleFileAsync();
            ReadLocation(file);


        }

        private async void ReadLocation(IStorageItem item)
        {
            _info.Path = item.Path;
            StorageApplicationPermissions.FutureAccessList.Add(item);
            if (item.IsOfType(StorageItemTypes.File))
            {
                _info.Type = LinkType.OfflineZip;
            }
            else
            {
                _info.Type = LinkType.OfflineFolder;
            }
            Images = new ObservableCollection<BitmapImage>(await ChapterVm.GetBitmapImages(_info));
            View = ViewManager.GetView(ViewType.ReadingView, this);

        }
    }
}
