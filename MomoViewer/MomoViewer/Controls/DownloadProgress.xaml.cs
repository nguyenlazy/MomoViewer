using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MomoViewer.Controls
{
    public sealed partial class DownloadProgress : ContentDialog
    {
        public DownloadProgress()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        public static readonly DependencyProperty PercentProperty = DependencyProperty.Register(
            "Percent", typeof(int), typeof(DownloadProgress), new PropertyMetadata(default(int)));

        public int Percent
        {
            get { return (int)GetValue(PercentProperty); }
            set { SetValue(PercentProperty, value); }
        }

        public static readonly DependencyProperty PercentStringProperty = DependencyProperty.Register(
            "PercentString", typeof(string), typeof(DownloadProgress), new PropertyMetadata(default(string)));

        public string PercentString
        {
            get { return (string)GetValue(PercentStringProperty); }
            set { SetValue(PercentStringProperty, value); }
        }
    }
}
