// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Microsoft.Azure.SignalR.Samples.ChatRoom
{
    //The Chat class name becomes 
    public class Chat : Hub
    {
        public void BroadcastMessage(string name, string message)
        {
            Clients.All.SendAsync("broadcastMessage", name, message);
        }

        public void Echo(string name, string message)
        {
            Clients.Client(Context.ConnectionId).SendAsync("echo", name, message + " (echo from server)");
        }

        public override Task OnConnectedAsync()
        {
            Clients.Others.SendAsync("pingUsers", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public void PingUsers(string source)
        {
            Clients.Others.SendAsync("returnTimeStamp", source);
        }

        public void ReturnTimeStamp(string source, DateTime time)
        {
            Clients.Client(source).SendAsync("broadcastMessage", time);
        }
    }
}
