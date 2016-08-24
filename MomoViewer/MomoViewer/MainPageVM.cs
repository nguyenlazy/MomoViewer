using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MomoViewer.ViewModel;

namespace MomoViewer
{
    public class MainPageVM : ViewModelBase
    {
        private ICommand _openFileCommand;
        private ICommand _openFolderCommand;

        ChapterVM _chapterVm = new ChapterVM();
        ObservableCollection<BitmapImage> images = new ObservableCollection<BitmapImage>();

        public ICommand OpenFileCommand
        {
            get { return _openFileCommand; }
            set { _openFileCommand = value; }
        }

        public ChapterVM ChapterVm
        {
            get { return _chapterVm; }
            set { _chapterVm = value; }
        }

        public ObservableCollection<BitmapImage> Images
        {
            get { return images; }
            set { images = value; }
        }

        public ICommand OpenFolderCommand
        {
            get { return _openFolderCommand; }
            set { _openFolderCommand = value; }
        }

        public MainPageVM()
        {
            OpenFileCommand = new RelayCommand(ExecuteOpenFile);
            OpenFolderCommand = new RelayCommand(ExecuteOpenFolder);
        }

        private async void ExecuteOpenFolder()
        {
            var page = await _chapterVm.LoadChapterFromFolder();
            if (page != null)
            {
                Images = new ObservableCollection<BitmapImage>(page.Images);
                RaisePropertyChanged("Images");
            }

        }

        private async void ExecuteOpenFile()
        {
            var page = await _chapterVm.LoadChapterFromFile();
            if (page != null)
            {
                Images = new ObservableCollection<BitmapImage>(page.Images);
                RaisePropertyChanged("Images");
            }

        }
    }
}
