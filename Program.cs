using System;
using System.Threading.Tasks;
using System.Net.Sockets;
using Discord;
using Discord.WebSocket;
using Discord.Net.Providers.WS4Net;
using System.IO;

namespace Ferry_Bot
{
    class Program
    {
        private static string token;
        private static Stream stream;
        static void Main(string[] args) {
            if(args.Length == 0)
            {
                System.Console.WriteLine("Please enter the bot's key in the arguments.");
            }
            token = args[0];
            stream = new FileStream("weapon_skills.png", FileMode.Open);
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            var client = new DiscordSocketClient(new DiscordSocketConfig
            {
                WebSocketProvider = WS4NetProvider.Instance
            });

            await client.LoginAsync(TokenType.Bot, token);

            await client.StartAsync();

            client.MessageReceived += MessageReceived;

            await Task.Delay(-1);
        }

        /*private Task Log(LogMessageEventArgs msg)
        {
            Console.WriteLine(msg.ToString());
            //return Task.CompletedTask;
            return null;
        }*/

        private async Task MessageReceived(SocketMessage message)
        {

            string content = message.Content.ToLower();


            if (message.Content == "!ping")
            {
                await message.Channel.SendMessageAsync("Pong!");
            }

            if (message.Content == "!jst")
            {
                string sendme = getTimeJST();
                await message.Channel.SendMessageAsync("It is currently: " + sendme + " in JST.");
      
            }

            if (message.Content == "!kaorix")
            {
                await message.Channel.SendMessageAsync("I love S. Zoi.");
            }

            if (message.Content == "!salt")
            {
                await message.Channel.SendMessageAsync("Who?");
            }

            if (content ==  "!weapon skills" || content == "!weapon_skills" || content == "skill_levels")
            {
                await message.Channel.SendFileAsync(stream, "HIIIII.png");
            }
        }


        public string getTimeJST()
        {
            string retme = "ERROR";
            DateTime now = DateTime.Now;
            //now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            try {
                TimeZoneInfo jZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
                //TimeZoneInfo cZone = TimeZoneInfo.FindSystemTimeZoneById("Central Stardard Time");
                //DateTime jst = TimeZoneInfo.ConvertTimeToUtc(now, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
                DateTime jst = TimeZoneInfo.ConvertTime(now, TimeZoneInfo.Local, jZone);

                retme = jst.ToShortTimeString();
            }
            catch (TimeZoneNotFoundException)
            {
                Console.WriteLine("Unable to find the timezone.");
            }

            catch (InvalidTimeZoneException)
            {
                Console.WriteLine("Invalid TZ");
            }

            return retme;
        }
    }
}
