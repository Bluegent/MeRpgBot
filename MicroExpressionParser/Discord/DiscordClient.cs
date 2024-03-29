﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGEngine.Discord
{
    using global::Discord;
    using global::Discord.WebSocket;

    using Newtonsoft.Json;

    using RPGEngine.Core;
    using RPGEngine.Game;

    public  class DiscordClient : GameInterface.ILogger
    {
        private SocketGuild server;
        private SocketTextChannel channel;
        private DiscordSocketClient client;
        private Action ready;

        private bool isReady;

        public IGameEngine Engine { get; set; }

        private DiscordConfig config;

        public DiscordSocketClient Client
        {
            get { return client; }
        }
        public Action Ready
        {
            get { return ready; }
            set { ready = value; }
        }

        public SocketGuild Server
        {
            get { return server; }
        }
        public SocketTextChannel Channel
        {
            get { return channel; }
        }

        public DiscordClient()
        { 
        }

        public async void Init()
        {
            config = JsonConvert.DeserializeObject<DiscordConfig>(Utils.FileHandler.Read("config.json"));
            client = new DiscordSocketClient();

            client.Log += DiscordLog;
            client.MessageReceived += MessageReceived;
            client.Ready += _client_Ready;
            isReady = false;

            await client.LoginAsync(TokenType.Bot, config.Token);
            await client.StartAsync();
        }

        SocketGuild GetServer()
        {
            foreach (SocketGuild s in client.Guilds)
            {
                if (s.Name.Equals(config.Server))
                    return s;
            }
            return null;
        }
        SocketTextChannel GetChannel()
        {
            if (server != null)
            {
                foreach (SocketTextChannel c in server.TextChannels)
                    if (c.Name.Equals(config.Channel))
                        return c;
            }
            return null;
        }

        private Task _client_Ready()
        {
            server = GetServer();
            if (server != null)
            {
                channel = GetChannel();
            }
            isReady = true;
            Log($"```css\n #RPG-Bot is ready! [{Version.VersionString()}]\n```");
            ready?.Invoke();
            
            return Task.CompletedTask;
        }

        private Task MessageReceived(SocketMessage message)
        {
            if (message.Channel == channel)
            {
                if (message.Content[0].ToString() == config.Prefix)
                {
                    Command command=Command.FromMessage((long)message.Author.Id,message.Content.Substring(1, message.Content.Length - 1));
                    Engine.EnqueueCommand(command);
                }
            }
            return Task.CompletedTask;
        }


        private Task DiscordLog(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
        public void Log(string msg)
        {
            if(isReady)
            if (!string.IsNullOrEmpty(msg))
                Channel.SendMessageAsync(msg);
        }
    }
}
