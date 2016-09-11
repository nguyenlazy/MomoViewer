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
using MomoViewer.Repository.DataAcess;
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
        private IDataAccess _dataAccess;
        private ObservableCollection<LinkInfo> _recentList;

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

        public ObservableCollection<LinkInfo> RecentList
        {
            get { return _recentList; }
            set { _recentList = value; }
        }

        public MainPageVM(IDataAccess dataAccess, ChapterVM chapterVm)
        {
            OpenFileCommand = new RelayCommand(ExecuteOpenFile);
            OpenFolderCommand = new RelayCommand(ExecuteOpenFolder);
            OpenLinkCommand = new RelayCommand(ExecuteOpenLinkCommand);
            OpenRecentCommand = new RelayCommand(ExecuteOpenRecentCommand);



            View = new ReadingView();

            _chapterVm = chapterVm;
            _dataAccess = dataAccess;

            using (var db = new DatabaseContext())
            {
                RecentList = new ObservableCollection<LinkInfo>(db.Links.ToList());
            }

        }

        private void ExecuteOpenRecentCommand()
        {
            View = ViewManager.GetView(ViewType.RecentView, new object());
        }


        private async void ExecuteOpenLinkCommand()
        {
            _info = new LinkInfo();
            DownloadDialog dialog = new DownloadDialog();
            var res = await dialog.ShowAsync();
            switch (res)
            {
                case ContentDialogResult.None:
                    break;
                case ContentDialogResult.Primary:
                    _info.Path = dialog.Link;
                    _info.Type = LinkType.Online;
                    ReadLinkInfo(_info);
                    break;
                case ContentDialogResult.Secondary:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _dataAccess.AddRecent(_info);
            RecentList.Add(_info);


        }

        public async void ReadLinkInfo(LinkInfo info)
        {
            Images = new ObservableCollection<BitmapImage>(await ChapterVm.GetBitmapImages(info));
            View = ViewManager.GetView(ViewType.ReadingView, this);
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
            if (item == null)
            {
                return;
            }
            _info = new LinkInfo();
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
            ReadLinkInfo(_info);
            _dataAccess.AddRecent(_info);
            RecentList.Add(_info);
        }
    }
}
