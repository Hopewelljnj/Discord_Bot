﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using System.IO;
using Discord_Bot.Commands;
using Newtonsoft.Json;

namespace Discord_Bot
{
    static class Tools
    {
        static Dictionary<string, ServerInfo> serverInfo = new Dictionary<string, ServerInfo>();
        public static Random random = new Random();
        
        static Tools()
        {
            if (File.Exists("../LocalFiles/ServerInfo.json"))
            {
                var sw = new StreamReader("../LocalFiles/ServerInfo.json");

                string json = sw.ReadToEnd();
                serverInfo = JsonConvert.DeserializeObject<Dictionary<string, ServerInfo>>(json);
                sw.Close();
            }
        }
        
        public static string CalculateTime(int minutes)
        {
            if (minutes == 0)
                return "No time.";

            int years, months, days, hours = 0;

            hours = minutes / 60;
            minutes %= 60;
            days = hours / 24;
            hours %= 24;
            months = days / 30;
            days %= 30;
            years = months / 12;
            months %= 12;

            string animeWatched = "";

            if (years > 0)
            {
                animeWatched += years;
                if (years == 1)
                    animeWatched += " **year**";
                else
                    animeWatched += " **years**";
            }

            if (months > 0)
            {
                if (animeWatched.Length > 0)
                    animeWatched += ", ";
                animeWatched += months;
                if (months == 1)
                    animeWatched += " **month**";
                else
                    animeWatched += " **months**";
            }

            if (days > 0)
            {
                if (animeWatched.Length > 0)
                    animeWatched += ", ";
                animeWatched += days;
                if (days == 1)
                    animeWatched += " **day**";
                else
                    animeWatched += " **days**";
            }

            if (hours > 0)
            {
                if (animeWatched.Length > 0)
                    animeWatched += ", ";
                animeWatched += hours;
                if (hours == 1)
                    animeWatched += " **hour**";
                else
                    animeWatched += " **hours**";
            }

            if (minutes > 0)
            {
                if (animeWatched.Length > 0)
                    animeWatched += " and ";
                animeWatched += minutes;
                if (minutes == 1)
                    animeWatched += " **minute**";
                else
                    animeWatched += " **minutes**";
            }

            return animeWatched;
        }

        #region messaging
        public static async Task Reply(User user, Channel channel, string text, bool mentionUser)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    if (channel.IsPrivate || !mentionUser)
                        await channel.SendMessage(text);
                    else
                        await channel.SendMessage($"{user.Mention}: {text}");
                }
            }
            catch (Exception e)
            {
                LogError("Couldn't send message.", e.Message);
            }
        }

        public static Task Reply(CommandArgs e, string text)
            => Reply(e.User, e.Channel, text, true);
        public static Task Reply(CommandArgs e, string text, bool mentionUser)
            => Reply(e.User, e.Channel, text, mentionUser);

        #endregion

        public static void LogError(string ErrorMessage, string exMessage)
        {
            StreamWriter sw = new StreamWriter("./errorLog.txt", true);
            sw.WriteLine($"[Error] {ErrorMessage} [Exception] {exMessage}");
            sw.Close();
        }

        #region server info
       
        public static ServerInfo GetServerInfo(ulong serverId)
        {
            if (serverInfo.ContainsKey(serverId.ToString()))
                return serverInfo[serverId.ToString()];
            else
            {
                var info = new ServerInfo();
                serverInfo.Add(serverId.ToString(), info);
                return info;
            }
        }

        public static void SaveServerInfo()
        {
            StreamWriter sw = new StreamWriter("../LocalFiles/ServerInfo.json", false);
            string json = JsonConvert.SerializeObject(serverInfo);
            sw.Write(json);
            sw.Close();
        }
        #endregion

        #region USER INFO
        public static User GetUser(CommandArgs eventArgs)
        {
            try
            {
                string userName = string.Empty;

                for (int i = 0; i < eventArgs.Args.Length - 1; i++)
                {
                    userName += eventArgs.Args[i] + ' ';
                }

                if (userName[0] == '@')
                    userName = userName.Substring(1);

                userName = userName.Remove(userName.Length - 1);

                var user = eventArgs.Server.FindUsers(userName).FirstOrDefault();

                return user;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[EXCEPTION] Couldn't get user! {e.Message} @" + DateTime.Now.ToString("HH:mm:ss tt"));
                return null;
            }
        }

        public static int GetPerms(CommandArgs e, User u)
        {
            if (serverInfo.ContainsKey(e.serverId))
            {
                var surfer = serverInfo[e.serverId];
                if( u.ServerPermissions.ManageServer)
                {
                    return 9000;
                }
                else if(ulong.Parse((string)Program.ProgramInfo.DevID) == e.User.Id)
                {
                    return 3000;
                }
                else if(u.ServerPermissions.DeafenMembers)
                {
                    return 500;
                }
                else if(u.ServerPermissions.SendMessages)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }




            }

            //returns -1 if failed or role has no tier set.
            return -1;
        }
        #endregion

        #region StreamReader/Writer

        public static string ReadFile(string Path)
        {
            if (!File.Exists(Path))
                return null;

            using (StreamReader sr = new StreamReader(Path))
            {
                return sr.ReadToEnd();
            }
        }

        public static void CreateFile(string Path)
        {
            File.Create(Path);
        }

        public static void SaveFile(string content, string Path, bool append)
        {
            using (StreamWriter sw = new StreamWriter(Path, append))
            {
                sw.WriteLine(content);
            }
        }

        #endregion

        public static async Task OfflineMessage(MessageEventArgs e)
        {
            if (e.User.Id == Program.Client.CurrentUser.Id)
                return;

            try
            {
                string msg = String.Empty;
                bool morethanone = false;
                foreach (var usr in e.Message.MentionedUsers)
                {
                    if (usr.Id == Program.Client.CurrentUser.Id)
                        continue;

                    if (usr.Status == UserStatus.Offline)
                    {
                        if (msg == String.Empty)
                            msg += usr.Mention;
                        else
                        {
                            msg += $", {usr.Mention}";
                            morethanone = true;
                        }

                        if (e.Message.Text.Length > 1500)
                            continue;

                        string msgToSend = $"**{e.User.Name} tagged you and said**: {e.Message.Text}";
                        await usr.SendMessage(msgToSend);
                        await Task.Delay(400);
                    }
                }

                string isare = morethanone ? "are" : "is";
                if (msg != String.Empty)
                {
                    msg += $" {isare} offline. Sending private message with content.";
                    await Task.Delay(100);
                    await Reply(e.User, e.Channel, msg, true);
                }
            }
            catch (Exception) { }
        }

        public static int CommandSpam(CommandArgs e, User u, bool IsAdmin, bool Add)
        {
            uint Point = 0;
            if (IsAdmin) { Point = 2; }
            else { Point = 5; }

            if(Tools.GetPerms(e, u) > 1 && !IsAdmin)
            {
                return 0;
            }
            

            string PathToSpammers = "../LocalFiles/Spammers.json";
            string json = Tools.ReadFile(PathToSpammers);
            int returnint = -1;
            string[] Users = json.Split('}');
            if (Users.Contains(u.Id.ToString()))
            {
                foreach (string user in Users)
                {
                    user.Replace("{", "");
                    string[] parameters = user.Split(',');
                    if (parameters.Length != 2)
                    {
                        return -1;
                    }
                    ulong userID;
                    uint numberSpams;
                    if (!uint.TryParse(parameters[1], out numberSpams) || !ulong.TryParse(parameters[0], out userID))
                    {
                        Console.WriteLine("Error with parameters in CommandSpam testing. @" + DateTime.Now.ToString("HH:mm:ss tt"));
                        return -1;
                    }
                    if (userID != u.Id)
                    {

                    }
                    else if (numberSpams < Point)
                    {
                        numberSpams++;
                        Tools.Reply(e, "Please stop attempting admin commands. Last warning.");
                        Tools.SaveFile(json, PathToSpammers, false);
                    }
                    else if (numberSpams >= Point)
                    {
                        Tools.Reply(e, "Too many spams. Message sent and no reply unavailable for 10 minutes.");
                        Tools.SaveFile(json, PathToSpammers, false);
                        returnint = 1;
                    }
                    if (Add)
                    {
                        json += "{" + userID + "," + numberSpams + "}";
                    }

                }
            }
            else
            {
                if (Add)
                {
                    json = json + "{" + u.Id.ToString() + ",1}";
                    Tools.SaveFile(json, PathToSpammers, false);
                }
            }
            if (Add) { return returnint; }
            else { return 0; }
        
        }


        public static void update(List<VoteObject> Entries)
        {
            string PathToVotes = "../LocalFiles/Votes.json";

            string json = "[";
            List<uint> Ids = new List<uint>();
            foreach(VoteObject voteObj in Entries)
            {
                if(Ids.Contains(voteObj.ID))
                {
                    Entries.Remove(voteObj);
                }
                else
                {
                    Ids.Add(voteObj.ID);
                }
            }


            foreach(VoteObject voteObj in Entries)
            {
                json += "{ID:" + voteObj.ID + ",Name:" + voteObj.name + ",Vote:" + voteObj.Vote + ",UsersVoted:[";
                foreach(ulong userId in voteObj.UsersVoted)
                {
                    json += userId + ",";
                }
                if (json.ElementAt(json.Length - 1) == ',') {json = json.Remove(json.Length - 1); }
                
                json += "]}";
            }
            json += "]";
            Tools.SaveFile(json, PathToVotes, false);

        }

        public static List<VoteObject> currentInfo()
        {
            string PathToVotes = "../LocalFiles/Votes.json";

            string json = Tools.ReadFile(PathToVotes);



            return Deserialize(json);

        }


        public static List<ulong> getUsers(string Value)
        {
            List<ulong> userList = new List<ulong>();

            Value = Value.Replace("]", "");
            Value = Value.Replace("[", "");

            string[] users = Value.Split(',');

           
            
            
            foreach(string user in users)
            {
                if (user != "")
                {
                    ulong i = 0;
                    ulong.TryParse(user, out i);
                    userList.Add(i);
                }
            }

            return userList;
          
        }




        public static List<VoteObject> Deserialize(string json)
        {
            List<int> parenthList = new List<int>();




            List<VoteObject> objects = new List<VoteObject>();


            string[] objectStrings = json.Split('{');



            foreach (string Name in objectStrings)
            {

          

                VoteObject voteObj = new VoteObject("");

                string[] items = Name.Split(',');

                

                foreach (string obj in items)
                {
                    string obj2 = obj.Replace("\"", "");
                    obj2 = obj2.Replace("\n", "");
                    obj2 = obj2.Replace("\r", "");
                    obj2 = obj2.Replace("}", "");
                    if(obj2.ElementAt(obj2.Length - 1) == ']') { obj2 = obj2.Remove(obj2.Length - 1); }

                    for (int i = 0; i < obj2.Length; i++)
                    {
                        char currentChar = obj2.ElementAt(i);
                        if (currentChar == ':')
                        {
                            string Type = obj2.Substring(0, i);
                            string Value = obj2.Substring(i + 1);
                            int k;
                            switch (Type)
                            {
                                case "Name":
                                    voteObj.name = Value;
                                    break;
                                case "ID":
                                    k = 0; int.TryParse(Value, out k); voteObj.ID = (uint)k;
                                    break;
                                case "Vote":
                                    k = 0; int.TryParse(Value, out k); voteObj.Vote = (uint)k;
                                    break;
                                case "UsersVoted": voteObj.UsersVoted = getUsers(Value);
                                    break;
                            }

                        }
                    }

                }
                if(voteObj.name != ""){ objects.Add(voteObj); }
               
            }


            return objects;
        }


        public static List<VoteObject> order(List<VoteObject> Entries)
        {
            List<uint> numbers = new List<uint>();
            List < VoteObject > Entries2 = new List<VoteObject>();
            foreach(VoteObject entry in Entries)
            {
                numbers.Add(entry.ID);
            }
            numbers.Sort();
            foreach(uint number in numbers)
            {
                foreach(VoteObject entry in Entries)
                {
                    if(entry.ID == number)
                    {
                        Entries2.Add(entry);
                    }
                }
            }

            return Entries2;

        }

        public static Func<CommandArgs, Task> ReadChannel = async e =>
        {
            string message = e.User.Name + " : " + e.ArgText + " @ " + e.Message.Timestamp;
            string PathToChannel = "../ LocalFiles / PriChannel.json";
            Tools.SaveFile(message, PathToChannel, true);
        };


    }


}
