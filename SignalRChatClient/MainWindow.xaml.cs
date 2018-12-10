#region snippet_MainWindowClass
using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRChatClient
{
    public partial class MainWindow : Window
    {
        HubConnection connection;
        Menus menu;
        public MainWindow()
        {
            InitializeComponent();

            connection = new HubConnectionBuilder()
                //WHEN DONE TESTING LOCALLY...
                //MAKE SURE NEXT LINE ENDS WITH THE URL AND HUB NAME THAT MATCHES
                //OF YOUR PUBLISHED CHATROOM PROJECT.

                //http://triquetrachatroom.azurewebsites.net/
                //http://localhost:5000
                .WithUrl("http://triquetrachatroom.azurewebsites.net/chat")
                .Build();

            #region snippet_ClosedRestart
            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0,5) * 1000);
                await connection.StartAsync();
            };
            #endregion
            menu = new Menus(this);
        }
        private void RunAsync()
        {
            //await Task.Delay(new Random().Next(0, 5) * 1000);
            menu.CheckForUpdateFromOtherUser();
        }
        public async void SendInfo(string message)
        {
            #region snippet_ErrorHandling
            try
            {
                #region snippet_InvokeAsync
                await connection.InvokeAsync("BroadcastMessage",
                    userTextBox.Text, message);
                #endregion
            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }
            #endregion
        }

        private async void connectButton_Click(object sender, RoutedEventArgs e)
        {
            #region snippet_ConnectionOn
            
            connection.On<string>("PingUsers", (source) =>//this part gets called on connect
            {
                this.Dispatcher.Invoke(() =>
                {
                    connection.InvokeAsync("ReturnTimeStamp", source, menu.tree.currentTime);//this part doesn't get called
                    messagesList.Items.Add("trying to call Ping Users");
                });
            });
        
            connection.On<string, string>("broadcastMessage", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = $"{user}: {message}";//this part gets called when you click send
                   messagesList.Items.Add(newMessage);//the reason this part gets called is because of line 105
                    RunAsync();
                });
            });

            connection.On<string, DateTime>("returnTimeStamp", (source, time) =>//this part gets called on connect
            {
                messagesList.Items.Add("Trying to call return timestamp");//this part never gets called. I think it needs to be invoked from here but I have no idea how to pass in the params.
                time = menu.tree.currentTime;
            });

            #endregion

            try
            {
                await connection.StartAsync();
                messagesList.Items.Add("Connection started");
                connectButton.IsEnabled = false;
                sendButton.IsEnabled = true;
                menuButton.IsEnabled = true;
                submitButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }
        }

        private async void sendButton_Click(object sender, RoutedEventArgs e)
        {
            #region snippet_ErrorHandling
            try
            {
                #region snippet_InvokeAsync
                await connection.InvokeAsync("BroadcastMessage", //this is where the code in connection.on is called
                    userTextBox.Text, messageTextBox.Text);
                #endregion
            }
            catch (Exception ex)
            {                
                messagesList.Items.Add(ex.Message);                
            }
            #endregion
        }
    
        private void echoButton_Click(object sender, RoutedEventArgs e)
        {

            #region snippet_ErrorHandling
            try
            {
                #region snippet_InvokeAsync
                #endregion
                messagesList.Items.Add(menu.menuMessage);
            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }
            #endregion
        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            menu.Menu();
        }
    }
}
#endregion
