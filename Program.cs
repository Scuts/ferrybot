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
        static void Main(string[] args)
        {
            if (args.Length == 0)
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

            if (message.Content.StartsWith("!jst"))
            {
                string[] jstArgs = message.Content.Split(new string[] {" "}, StringSplitOptions.None);
                if (jstArgs.Length == 4)
                {
                    await message.Channel.SendMessageAsync(getTimeJST(jstArgs[1], jstArgs[2], jstArgs[3]));
                } else
                {
                    string sendme = getTimeJST();
                    await message.Channel.SendMessageAsync(sendme);
                }

            }

            if (message.Content.StartsWith("!sticker"))
            {
                string[] stickerArgs = message.Content.Split(new string[] { " " }, StringSplitOptions.None);
                if (stickerArgs.Length == 2)
                {
                    sendSticker(message, stickerArgs[1]);
                } else
                {
                    await message.Channel.SendMessageAsync("Please specify the sticker you would like with the following format: !sticker <file>");
                }
            }

            if (message.Content == "!kaorix")
            {
                await message.Channel.SendMessageAsync("I love S. Zoi.");
            }

            if (message.Content == "!salt")
            {
                await message.Channel.SendMessageAsync("Who?");
            }

            if (content == "!skill_levels")
            {
                await message.Channel.SendFileAsync(stream, "Skill Level Table.png");
            }
        }

        public async void sendSticker(SocketMessage message, string stickerName)
        {
            FileStream stickerStream;
            try
            {
                stickerStream = new FileStream("Images/Stickers/" + stickerName + ".png", FileMode.Open);

                await message.Channel.SendFileAsync(stickerStream, stickerName + ".png");

                stickerStream.Close();
            }
            catch (FileNotFoundException)
            {
                await message.Channel.SendMessageAsync("The specified sticker does not exist!");
            }
        }


        public string getTimeJST()
        {
            string retme = "ERROR";
            DateTime now = DateTime.Now;
            //now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            try
            {
                TimeZoneInfo jZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
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

            return "It is currently: " + retme + " in JST.";
        }

        public string getTimeJST(string time, string ampm, string tz)
        {
            string retme = "ERROR";

            string timeR = time.Replace(":", "");
            ampm = ampm.ToLower();
            tz = tz.ToUpper();

            if(timeR.Length > 4 || timeR.Length < 1)
            {
                return time + "Invalid Time format, first parameter must be of the form H, HH, HMM, HHMM, H:MM, or H:MM where H is the time in hours, and M is the time in minutes.";
            }

            if(ampm != "am" && ampm != "pm")
            {
                return "Invalid Time format, second parameter must be equal to AM or PM";
            }

            string tzlong = timeZoneExpand(tz);
            if(tzlong == "")
            {
                return "Invalid Timezone: Accepted timezones are as follows (In order of GMT/UTC offset): GMT, CET, IST, EET, MSK, EAT, AST, PKT, BTT, THA, CT, SGT, AWST, ACST, AEST, NCT, NZST, TOT, BRT, CLT, EST, CST, MST, PST, AKST, HAST, SST, AOE.";
            }

            int hour = 0;
            int minute = 0;

            if (timeR.Length == 1 || timeR.Length == 2)
            {
                hour = Int32.Parse(time);
            } else if (timeR.Length == 3)
            {
                hour = Int32.Parse(timeR.Substring(0, 1));
                minute = Int32.Parse(timeR.Substring(1));
            } else if (timeR.Length == 4)
            {
                hour = Int32.Parse(timeR.Substring(0, 2));
                minute = Int32.Parse(timeR.Substring(2));
            }

            if(ampm == "pm")
            {
                hour = hour + 12;
                if(hour == 24)
                {
                    hour = 0;
                }
            }

            if (hour > 23 || minute > 59)
            {
                return "Invalid time, the hours section cannot be greater than 12, and the minutes section cannot be greater than 59";
            }

            DateTime now = DateTime.Now;
            DateTime timeToSend = new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);

            try
            {
                TimeZoneInfo jZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                TimeZoneInfo tZone = TimeZoneInfo.FindSystemTimeZoneById(tzlong);
                DateTime jst = TimeZoneInfo.ConvertTime(timeToSend, tZone, jZone);

                retme = time + " in " + tZone + " converted to JST is: " + jst.ToShortTimeString();
            }
            catch (TimeZoneNotFoundException)
            {
                Console.WriteLine("Unable to find the timezone.");
                retme = "The author lied, you entered what he said is a valid timezone and it failed.";
            }

            catch (InvalidTimeZoneException)
            {
                Console.WriteLine("Invalid TZ");
                retme = "The author lied, you entered what he said is a valid timezone and it failed.";
            }

            return retme;
        }


        public string timeZoneExpand(string tz)
        {
            if (tz == "GMT")
            {
                return "GMT Standard Time";
            }
            if(tz == "CET")
            {
                return "Central European Standard Time";
            }
            if(tz == "IST")
            {
                return "Israel Standard Time";
            }
            if(tz == "EET")
            {
                return "E. Europe Standard Time";
            }
            if(tz == "MSK")
            {
                return "Russian Standard Time";
            }
            if(tz == "EAT")
            {
                return "E. Africa Standard Time";
            }
            if(tz == "AST")
            {
                return "Arabian Standard Time";
            }
            if(tz == "PKT")
            {
                return "Pakistan Standard Time";
            }
            if(tz == "BTT")
            {
                return "Central Asia Standard Time";
            }
            if(tz == "THA")
            {
                return "SE Asia Standard Time";
            }
            if(tz == "CT")
            {
                return "China Standard Time";
            }
            if(tz == "SGT")
            {
                return "Singapore Standard Time";
            }
            if (tz == "AWST")
            {
                return "W. Australia Standard Time";
            }
            if (tz == "ACST")
            {
                return "Cen. Australia Standard Time";
            }
            if (tz == "AEST")
            {
                return "E. Australia Standard Time";
            }
            if (tz == "NCT")
            {
                return "Central Pacific Standard Time";
            }
            if (tz == "NZST")
            {
                return "New Zealand Standard Time";
            }
            if (tz == "TOT")
            {
                return "Tonga Standard Time";
            }
            if (tz == "BRT")
            {
                return "E. South America Standard Time";
            }
            if (tz == "CLT")
            {
                return "SA Western Standard Time";
            }
            if (tz == "EST")
            {
                return "Eastern Standard Time";
            }
            if (tz == "CST")
            {
                return "Central Standard Time";
            }
            if (tz == "MST")
            {
                return "Mountain Standard Time";
            }
            if (tz == "PST")
            {
                return "Pacific Standard Time";
            }
            if (tz == "AKST")
            {
                return "Alaskan Standard Time";
            }
            if (tz == "HAST")
            {
                return "Hawaiian Standard Time";
            }
            if (tz == "SST")
            {
                return "Samoa Standard Time";
            }
            if (tz == "AOE")
            {
                return "Dateline Standard Time";
            }

            return "";
        }
    }
}
