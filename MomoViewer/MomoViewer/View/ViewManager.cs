using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MomoViewer.View
{
    public static class ViewManager
    {
        public static FrameworkElement GetView(ViewType viewType, object dataContext)
        {
            FrameworkElement view;
            switch (viewType)
            {
                case ViewType.ReadingView:
                    view = new ReadingView();
                    break;
                case ViewType.RecentView:
                    view = new ManagementView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(viewType), viewType, null);
            }
            view.DataContext = dataContext;
            return view;
        }
    }

    public enum ViewType
    {
        ReadingView = 0,
        RecentView = 1
    }
}
