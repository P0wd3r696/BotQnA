using BotQnA.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BotQnA.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : ContentPage
	{
        MainVM viewModel;
		public MainPage ()
		{
			InitializeComponent ();
            viewModel = Resources["vm"] as MainVM;

            viewModel.Messages.CollectionChanged += Messages_CollectionChanged;
		}

        private void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var newMessage = viewModel.Messages[viewModel.Messages.Count - 1];

            //was not running on the UI Thread. Has to run on the main thread
            Device.BeginInvokeOnMainThread(() =>
            {
                chatListView.ScrollTo(newMessage, ScrollToPosition.End, true);
            });
            

        }
    }
}