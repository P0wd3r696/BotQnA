using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using static BotQnA.ViewModel.MainVM;

namespace BotQnA.Views.UserControls
{
    public class ChatViewCellTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate incomingDataTemplate;
        private readonly DataTemplate outgoingDataTemplate;

        public ChatViewCellTemplateSelector()
        {
            incomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            outgoingDataTemplate = new DataTemplate(typeof(OutGoingViewCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var chatMessage = item as ChatMessage;
            if(chatMessage != null)
            {
                return chatMessage.IsIncoming ? incomingDataTemplate : outgoingDataTemplate;
            }
            return null;
        }
    }
}
