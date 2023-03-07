using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AllInOneApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Navigation : Page
    {
        private NavigationViewItem _lastItem;
        public Navigation()
        {
            this.InitializeComponent();
        }

        private void NavigationViewControl_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var items = args.InvokedItemContainer as NavigationViewItem;

            //Selecting same tab.
            if (items == null || items == _lastItem) return;

            //Selecting another tab.
            var clickedView = items.Tag.ToString();

            if(!NavigatToView(clickedView)) return;
            _lastItem = items;
        }

        private bool NavigatToView(string clickedView)
        {
            var view = Assembly.GetExecutingAssembly().GetType($"AllInOneApp.Views.{clickedView}");
            if (string.IsNullOrWhiteSpace(clickedView)) return false;

            ContentFrame.Navigate(view, null, new EntranceNavigationTransitionInfo());
            return true;
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {

        }
    }
}
