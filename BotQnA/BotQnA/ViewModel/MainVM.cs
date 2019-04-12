using BotQnA.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace BotQnA.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        BotServiceHelper botHelper;
        public Command SendCommand { get; set; }
        private string message;
        public ObservableCollection<ChatMessage> Messages { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged("Message");
            }
        }
        
        public MainVM()
        {
            botHelper = new BotServiceHelper();
            SendCommand = new Command(SendActivity);
            Messages = new ObservableCollection<ChatMessage>();

            botHelper.MessageRecieved += BotHelper_MessageRecieved;
        }

        private void BotHelper_MessageRecieved(object sender, BotServiceHelper.BotResponseEventAgrs e)
        {
            foreach(var activity in e.Activities)
            {
                if(activity.From.Id != "user1")
                {
                    Messages.Add(new ChatMessage()
                    {
                        Text = activity.Text,
                        IsIncoming = true
                    });
                }
            }
        }

        void SendActivity()
        {
            Messages.Add(new ChatMessage()
            {
                Text = Message,
                IsIncoming = false
            });

            botHelper.SendActivity(Message);
            Message = string.Empty;
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class ChatMessage
        {
            public string Id { get; set; }

            public string Text { get; set; }

            public bool IsIncoming { get; set; }
        }

    }
}
