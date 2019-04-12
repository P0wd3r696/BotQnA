using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotQnA.ViewModel.Helpers
{
    public class BotServiceHelper
    {
        public Conversation _conversation { get; set; }

        public event EventHandler<BotResponseEventAgrs> MessageRecieved;

        public BotServiceHelper()
        {
            CreateConversation();

        }
        
        private async void CreateConversation()
        {
            string endPoint = "/v3/directline/conversations";

            string baseUri = "https://directline.botframework.com";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer 5qX9575VGgk.T_Wd5olK43P9Wr65qqh2hH-w-CrWOXpTqc0Nsy702ss");


                var response = await client.PostAsync(endPoint, null);

                string json = await response.Content.ReadAsStringAsync();

                _conversation = JsonConvert.DeserializeObject<Conversation>(json);

            }
            ReadMessage();
        }
        public async void SendActivity(string message)
        {
            string endPoint = $"/v3/directline/conversations/{_conversation.ConversationId}/activities";

            string baseUri = "https://directline.botframework.com";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUri);
                client.DefaultRequestHeaders.Add("Authorization", "Bearer 5qX9575VGgk.T_Wd5olK43P9Wr65qqh2hH-w-CrWOXpTqc0Nsy702ss");

                Activity activity = new Activity
                {
                    From = new ChannelAccount
                    {
                        Id = "user1",
                    },
                    Text = message,
                    Type = "message"
                };

                string jsonContent = JsonConvert.SerializeObject(activity);

                var buffer = Encoding.UTF8.GetBytes(jsonContent);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(endPoint, byteContent);

                string json = await response.Content.ReadAsStringAsync();

                var obj = JObject.Parse(json);
                string id = (string)obj.SelectToken("id");
            }
        }
        public async void ReadMessage()
        {
            var client = new ClientWebSocket();
            var cts = new CancellationTokenSource();

            await client.ConnectAsync(new Uri(_conversation.StreamUrl), cts.Token);

            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    WebSocketReceiveResult result;
                    var message = new ArraySegment<byte>(new byte[4096]);
                    do
                    {
                        result = await client.ReceiveAsync(message, cts.Token);
                        if (result.MessageType != WebSocketMessageType.Text)
                            break;
                        var messageBytes = message.Skip(message.Offset).Take(result.Count).ToArray();
                        string messageJson = Encoding.UTF8.GetString(messageBytes);
                        BotResponse botResponse = JsonConvert.DeserializeObject<BotResponse>(messageJson);

                        var args = new BotResponseEventAgrs();
                        args.Activities = botResponse.Activities;

                        MessageRecieved?.Invoke(this, args);
                    }
                    while (!result.EndOfMessage);
                }
            }, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        public class BotResponseEventAgrs : EventArgs
        {
            public List<Activity> Activities { get; set; }
        }
        
        public class BotResponse
        {
            public string WaterMark { get; set; }

            public List<Activity> Activities { get; set; }
        }

        public class ChannelAccount
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
        public class Activity
        {
            public ChannelAccount From { get; set; }

            public string Text { get; set; }

            public string Type { get; set; }
        }

        public class Conversation
        {
            public string ConversationId { get; set; }
            public string Token { get; set; }
            public string StreamUrl { get; set; }
            public string ReferenceGrammarId { get; set; }
            public int ExpiresIn { get; set; }
        }
    }
}
