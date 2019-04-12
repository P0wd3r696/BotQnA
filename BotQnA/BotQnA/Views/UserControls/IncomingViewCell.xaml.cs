using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BotQnA.Views.UserControls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class IncomingViewCell : ViewCell
	{
		public IncomingViewCell ()
		{
			InitializeComponent ();
		}
	}
}