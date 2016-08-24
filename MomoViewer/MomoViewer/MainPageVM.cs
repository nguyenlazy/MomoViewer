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
        public ICommand _openFileCommand;
        ChapterVM _chapterVm = new ChapterVM();
        ObservableCollection<string> images = new ObservableCollection<string>();

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

        public ObservableCollection<string> Images
        {
            get { return images; }
            set { images = value; }
        }

        public MainPageVM()
        {
            OpenFileCommand = new RelayCommand(ExecuteOpenFile);
        }

        private async void ExecuteOpenFile()
        {
            var page = await _chapterVm.LoadChapterInfo();
            Images =  new ObservableCollection<string>();

            foreach (var p in page.Pages)
            {
                images.Add(p.Path);
            }

            RaisePropertyChanged("Images");
        }
    }
}
