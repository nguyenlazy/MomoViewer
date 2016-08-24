﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Audio;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight;
using MomoViewer.Model;

namespace MomoViewer.ViewModel
{
    public class ChapterVM : ViewModelBase
    {
        private static readonly string ExtractPath = @"\temp";

        public ChapterVM()
        {
            _chapter = new ChapterInfo();
        }
        private ChapterInfo _chapter;

        public ChapterInfo Chapter
        {
            get { return _chapter; }
            set
            {
                _chapter = value;
                RaisePropertyChanged();
            }
        }

        public async Task<ChapterInfo> LoadChapterInfo()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".zip");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                StorageFolder d = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalCacheFolder.Path);
                await UnZipFileAync(file, d);
                StorageFolder fd = await StorageFolder.GetFolderFromPathAsync(Path.Combine(d.Path, file.DisplayName));

                var fileList = await fd.GetFilesAsync();
                int pageNumber = 0;
                foreach (var storageFile in fileList)
                {
                    PageInfo page = new PageInfo();
                    page.Path = storageFile.Path;
                    page.Number = pageNumber++;
                    using (var stream = await storageFile.OpenAsync(FileAccessMode.Read))
                    {
                        var bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
                        await bitmapImage.SetSourceAsync(stream);
                        page.Image = bitmapImage;
                    }
                    Chapter.Pages.Add(page);
                }

            }
            return Chapter;
        }


        public static IAsyncAction UnZipFileAync(StorageFile zipFile, StorageFolder destinationFolder)
        {
            return UnZipFileHelper(zipFile, destinationFolder).AsAsyncAction();
        }

        private static async Task UnZipFileHelper(StorageFile zipFile, StorageFolder destinationFolder)
        {
            if (zipFile == null || destinationFolder == null ||
                !Path.GetExtension(zipFile.FileType).Equals(".zip", StringComparison.CurrentCultureIgnoreCase)
                )
            {
                throw new ArgumentException("Invalid argument...");
            }
            Stream zipMemoryStream = await zipFile.OpenStreamForReadAsync();
            // Create zip archive to access compressed files in memory stream 
            using (ZipArchive zipArchive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Read))
            {
                // Unzip compressed file iteratively. 
                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                {
                    await UnzipZipArchiveEntryAsync(entry, entry.FullName, destinationFolder);
                }
            }
        }



        private static bool IfPathContainDirectory(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                return false;
            }
            return entryPath.Contains("/");
        }

        private static async Task<bool> IfFolderExistsAsync(StorageFolder storageFolder, string subFolderName)
        {
            try
            {
                await storageFolder.GetFolderAsync(subFolderName);
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }
        private static async Task UnzipZipArchiveEntryAsync(ZipArchiveEntry entry, string filePath,
            StorageFolder unzipFolder)
        {
            if (IfPathContainDirectory(filePath))
            {
                string subFolderName = Path.GetDirectoryName(filePath);
                bool isSubFolderExist = await IfFolderExistsAsync(unzipFolder, subFolderName);
                StorageFolder subFolder;
                if (!isSubFolderExist)
                {
                    subFolder =
                        await unzipFolder.CreateFolderAsync(subFolderName, CreationCollisionOption.ReplaceExisting);

                }
                else
                {
                    subFolder = await unzipFolder.GetFolderAsync(subFolderName);
                }

                string newFilePath = Path.GetFileName(filePath);
                if (!string.IsNullOrEmpty(newFilePath))
                {
                    await UnzipZipArchiveEntryAsync(entry, newFilePath, subFolder);
                }

            }
            else
            {
                using (Stream entryStream = entry.Open())
                {
                    byte[] buffer = new byte[entry.Length];
                    entryStream.Read(buffer, 0, buffer.Length);
                    StorageFile uncompressedFile =
                        await unzipFolder.CreateFileAsync(entry.Name, CreationCollisionOption.ReplaceExisting);
                    using (IRandomAccessStream uncompressedFileStream = await uncompressedFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        using (Stream outStream = uncompressedFileStream.AsStreamForWrite())
                        {
                            outStream.Write(buffer, 0, buffer.Length);
                            outStream.Flush();
                        }
                    }
                }
            }
        }

    }
}
