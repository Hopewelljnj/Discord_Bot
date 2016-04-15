using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Discord_Bot.Commands;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.Timers;
using System.IO;
using System.Linq;

/// <summary>
/// Perms:
/// > 1000 - Sabre and Stepper
/// > 100 - Mods
/// > 50 - Allow spamming
/// > 10 - Users
/// </summary>

namespace Discord_Bot
{
    class Program
    {
        private static DiscordClient _client;
        public static CommandsPlugin _commands, _admincommands;
        public static Timeout timeout;
        public static dynamic ProgramInfo = null;
        public static Timer spamTimer;

        

        public static DiscordClient Client
        {
            get
            {
                return _client;
            }
        }
        
        private static Dictionary<string, Timer> timedoutUser = new Dictionary<string, Timer>();
        
        static void Main(string[] args)
        {
            var client = new DiscordClient();
            _client = client;
            _client.Log.Message += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}  @" + DateTime.Now.ToString("HH:mm:ss tt"));

            _commands = new CommandsPlugin(client);
            _admincommands = new CommandsPlugin(client);
            _commands.CreateCommandGroup("", group => BuildCommands(group));
            _commands.CreateCommandGroup("", adminGroup => BuildAdminCommands(adminGroup));

            spamTimer = new Timer(600000);
            spamTimer.Elapsed += new ElapsedEventHandler(resetSpam);
            spamTimer.Start();



            Console.WriteLine("Timer started. @" + DateTime.Now.ToString("HH:mm:ss tt"));


            //Get Programinfo
            if (File.Exists("./../LocalFiles/ProgramInfo.json"))
            {
                using (StreamReader sr = new StreamReader("./../LocalFiles/ProgramInfo.json"))
                {
                    var jsonfile = sr.ReadToEnd();
                    ProgramInfo = JsonConvert.DeserializeObject(jsonfile);
                    Console.WriteLine(ProgramInfo.username + "@" + DateTime.Now.ToString("HH:mm:SS tt"));
                }
            }

            _client.UserJoined += async (s, e) =>
            {
                await Information.NewUserText(e.User, e.Server);
                await Information.WelcomeUser(_client, e.User, e.Server.Id);

                //await WelcomeUser(e.User, e.Server.Id);
            };

            _client.MessageReceived += async (s, e) =>
            {
                await Tools.OfflineMessage(e);
                await Fun.AyyGame(e);
            };
            _client.GatewaySocket.Disconnected += async (s, e) =>
            {
                while(_client.State != ConnectionState.Connected)
                {
                    try
                    {
                        await _client.Connect(ProgramInfo.username, ProgramInfo.password);
                    }
                    catch(Exception ex)
                    {
                        Tools.LogError("Couldn't connect!", ex.Message);
                    }

                    await Task.Delay(30000);
                }
            };
            _commands.CommandError += async (s, e) =>
            {
                var ex = e.Exception;
                if (ex is PermissionException)
                    await Tools.Reply(e, "Sorry, you do not have the permissions to use this command!");
                else
                    await Tools.Reply(e, $"Error: {ex.Message}.");

            };
            try
            {
                _client.ExecuteAndWait(async () =>
                {
                    var username = ProgramInfo.username;
                    var password = ProgramInfo.password;
                    var nottoken = Convert.ToString(ProgramInfo.bot_token);

                    await _client.Connect(nottoken);
                    timeout = new Timeout(_client);
                });
            }
            catch (Discord.Net.HttpException)
            {
                while (client.Status != UserStatus.Online)
                {

                }
            }
        }



        public static void resetSpam(object source, ElapsedEventArgs e)
        {
            string PathToSpammers = "../LocalFiles/Spammers.json";
            string PathToBackup = "../LocalFiles/BackupofSpammers.json";

            string json = Tools.ReadFile(PathToBackup) + "/n" + Tools.ReadFile(PathToSpammers);
            string blank = "";

            Tools.SaveFile(json, PathToBackup, false);
            Tools.SaveFile(blank, PathToSpammers, false);

            Console.WriteLine("Reset Spam List. @" + DateTime.Now.ToString("HH:mm:ss tt"));
        }


        #region New Users




        #endregion


        #region commands
        private static void BuildCommands(CommandGroupBuilder group)
        {
            group.DefaultMinPermissions(0);

            group.CreateCommand("normie")
                .Do(async e =>
                {
                    await Tools.Reply(e, "https://www.youtube.com/watch?v=JCeOf2q6_TA", false);
                });

            //Added by Will (d0ubtless)
            group.CreateCommand("kazoo")
                .Do(async e =>
                {
                    await Tools.Reply(e, "You need the kazoo, if you can't take part in this episode, you're a fucking faggot, you should just go kill yourself https://youtu.be/g-sgw9bPV4A", false);
                });

            group.CreateCommand("hb")
                .WithPurpose("Find a User's HummingBird account with it's information!")
                .ArgsAtMax(1)
                .IsHidden()
                .Do(AnimeTools.GetHBUser);

            //Idea by Will but I perfected it and it's a fucking game now, who would've thought.
           /* group.CreateCommand("shoot")
                .DelayIsUnignorable()
                .SecondDelay(30)
                .WithPurpose("shoot a user, with a chance to miss! Usage: type /shoot and tag any amount of users you want.\n`/shoot stats` for your personal score\n`/shoot top` for the top 5 killers!")
                .Do(Fun.ShootUser);*/

            group.CreateCommand("8ball")
                .WithPurpose("The magic eightball will answer all your doubts and questions!")
                .AnyArgs()
                .SecondDelay(20)
                .Do(Fun.EightBall);

            group.CreateCommand("lmao")
                .AnyArgs()
                .HourDelay(1)
                .Do(async e =>
                {
                    await Tools.Reply(e, "https://www.youtube.com/watch?v=HTLZjhHIEdw");
                });

            //Added by Will (d0ubtless)
            group.CreateCommand("noice")
                .AnyArgs()
                .MinuteDelay(1)
                .Do(async e =>
                {
                    await Tools.Reply(e, "https://youtu.be/a8c5wmeOL9o");
                });

            group.CreateCommand("no")
                .SecondDelay(120)
                .AnyArgs()
                .Do(async e =>
                {
                    await Tools.Reply(e, "pignig", false);
                });

            //Added by Will (d0ubtless)
            group.CreateCommand("codekeem")
                .SecondDelay(120)
                .AnyArgs()
                .Do(async e =>
                {
                    await e.Channel.SendFile("keemstar.png");
                    await Tools.Reply(e, "You have used code 'KEEM'", true);
                });

            group.CreateCommand("hello")
                .AnyArgs()
                .HourDelay(1)
                .Do(async e =>
                {
                    await Tools.Reply(e, $"Hello, {e.User.Mention}", false);
                });

            group.CreateCommand("ayy")
                .MinuteDelay(2)
                .AnyArgs()
                .Do(Fun.Ayy);

            group.CreateCommand("bullying")
                .AnyArgs()
                .WithPurpose("Getting bullied?")
                .IsHidden()
                .MinuteDelay(30)
                .Do(Fun.Bullying);

            group.CreateCommand("commands")
                .WithPurpose("Print list of commands to private message.")
                .AnyArgs()
                .IsHidden()
                .Do(Information.Commands);

            group.CreateCommand("help")
                .WithPurpose("Show the getting-started guide!")
                .AnyArgs()
                .IsHidden()
                .Do(async e =>
                {
                    await Information.NewUserText(e.User, e.Server);
                });

            group.CreateCommand("feedback")
                .WithPurpose("Give feedback to the bot! Stepper will read it sometime soon.. I think.")
                .ArgsAtLeast(1)
                .IsHidden()
                .Do(async e =>
                {
                    StreamWriter fs = new StreamWriter("../feedback.txt", true);
                    await fs.WriteLineAsync($"{e.User.Name} suggested: {e.ArgText}");
                    fs.Close();
                });

            group.CreateCommand("img")
                .MinuteDelay(1)
                .ArgsAtLeast(1)
                .WithPurpose("Get an image of Google. (100 per day pls)")
                .Do(Fun.GetImageFromGoogleDotCom);

            group.CreateCommand("mala")
                .ArgsAtLeast(1)
                .WithPurpose("Find an anime of MAL and Link it together with it's synopsis.")
                .Do(AnimeTools.GetAnimeFromMAL);

            group.CreateCommand("malm")
                .ArgsAtLeast(1)
                .WithPurpose("Find a manga of MAL and Link it together with it's synopsis.")
                .Do(AnimeTools.GetMangaFromMAL);

            group.CreateCommand("createVote")
                 .ArgsAtLeast(1)
                 .WithPurpose("Submit a vote with the Name or ID. Usage `/createVote name1,name2,name3...` /req rank Admin")
                 .IsAdmin()
                 .IsHidden()
                 .Do(async e =>
                 {
                     Vote voter = new Vote();
                     await voter.CreateVote(e);
                 });

            group.CreateCommand("vote")
                .ArgsAtLeast(1)
                .WithPurpose("Submit a vote. Usage `/vote Id` or Name")
                .Do(async e =>
                {
                    Vote voter = new Vote();
                    await voter.vote(e);
                });

            group.CreateCommand("endVote")
                .AnyArgs()
                .WithPurpose("End the voting and show results. Usage `/endVote` /req rank Admin")
                .IsAdmin()
                .IsHidden()
                .Do(async e =>
                {
                    Vote voter = new Vote();
                    await voter.EndVote(e);
                });

            group.CreateCommand("getVotes")
                .AnyArgs()
                .WithPurpose("Show current votes. Usage `/getVotes` ")
                .IsHidden()
                .Do(async e =>
                {
                    Vote voter = new Vote();
                    await voter.GetVotes(e);
                });

            group.CreateCommand("quote")
                .ArgsAtLeast(1)
                .WithPurpose("Return a saved quote. Usage `/quote #`")
                .Do(Quoting.quote);

            group.CreateCommand("quoteList")
                .AnyArgs()
                .WithPurpose("Direct messages a list of current quotes. Usage `/quoteList`")
                .IsHidden()
                .Do(Quoting.getQuotes);

            group.CreateCommand("readChannel")
                .AnyArgs()
                .Do(Tools.ReadChannel);

            _commands.CommandChar = '/';
        }


        private static void BuildAdminCommands(CommandGroupBuilder adminGroup)
        {
            adminGroup.DefaultMinPermissions(0);
            
            adminGroup.CreateCommand("delete")
                .WithPurpose("Delete messages on this channel. Usage `/delete {number of messages to delete}`. / req: rank Admin")
                .ArgsEqual(1)
                .IsAdmin()
                .Do(AdminCommands.DeleteMessages);

            adminGroup.CreateCommand("addpermission")
                .WithPurpose("Add number to rank. Usage: `/addpermission {rank name} {number}` / req: rank Owner")
                .IsAdmin()
                .Do(AdminCommands.AddPermissionToRank);

            adminGroup.CreateCommand("removePerm")
                .WithPurpose("Remove number of rank. Usage: `/removePerm {rank name}` / req: rank Owner")
                .IsAdmin()
                .Do(AdminCommands.RemovePermissionToRank);

            adminGroup.CreateCommand("editServer")
                .WithPurpose("Edits initial channel or initial role. Usage: `/editServer standardrole {role}` \n `/editServer welcomechannel {channel name}`  / req: rank Owner")
                .IsAdmin()
                .Do(AdminCommands.EditServer);

            adminGroup.CreateCommand("kick")
                .WithPurpose("Kicks specified user. Usage: `/kick {@username}` / req: rank Owner")
                .IsAdmin()
                .ArgsEqual(1)
                .Do(AdminCommands.KickUser);
            adminGroup.CreateCommand("timeout")
                .WithPurpose("Time out someone. Usage: `/timeout {@username}` {time in minutes}`. / req: rank Admin")
                .IsAdmin()
                .ArgsAtLeast(1)
                .Do(AdminCommands.TimeoutUser);
            adminGroup.CreateCommand("admincommands")
                .IsAdmin()
                .IsHidden()
                .AnyArgs()
                .Do(AdminCommands.GetCommands);
            adminGroup.CreateCommand("pause")
                .IsAdmin()
                .WithPurpose("Pause bot if it is acting wrongly. Will stop bot until Hopewell can access the server to reset bot.\n Please type a reason after the command so that the issue can be looked into. \n Usage: `/pause {reason}` /req: rank Admin")
                .ArgsAtLeast(1)
                .Do(AdminCommands.PauseBot);

            adminGroup.CreateCommand("createQuote")
                .IsAdmin()
                .WithPurpose("Add a quote to the saved list. Usage : `/createQuote quoted text @User")
                .IsHidden()
                .ArgsAtLeast(1)
                .Do(Quoting.createQuote);

            adminGroup.CreateCommand("deleteQuote")
                .IsAdmin()
                .IsHidden()
                .WithPurpose("Remove a quote from the saved list. Usage : /deleteQuote #")
                .ArgsAtLeast(1)
                .Do(Quoting.deleteQuote);
        }
        #endregion
        
    }
}
