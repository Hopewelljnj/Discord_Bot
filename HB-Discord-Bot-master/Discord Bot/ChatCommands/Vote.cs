using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Discord_Bot.Commands;
using Discord;
using System.IO;

namespace Discord_Bot
{
    public class Vote
    {
        VotePlugin VoteObj = new VotePlugin();
        VoteObject obj = new VoteObject("");
        public async Task vote(CommandArgs e)
        {
            string args = e.ArgText;
            User u = e.User;
            int ID;
                if (args == null || u == null)
                {
                    await Tools.Reply(e, $"No ID or Name given or no User ID found.");
                }
                else if (int.TryParse(args, out ID))
                {
                    VoteObj.vote(e, u, ID);
                }
                else
                {
                    VoteObj.vote(e, u, args);
                }
            
            
        }

        public async Task GetVotes(CommandArgs e)
        {
            VotePlugin Entries5 = new VotePlugin();
            string returnString = "";
            returnString = Entries5.ToString();
            await e.User.SendMessage(returnString);
        }

        public async Task CreateVote(CommandArgs e)
        {
            if ((Tools.GetPerms(e, e.User)) >= 1000 || ulong.Parse((string)Program.ProgramInfo.DevID) == e.User.Id)
            {
                User u = e.User;
                string args = e.ArgText;
                List<int> commaList = new List<int>();

                VoteObj.Entries.RemoveRange(0, VoteObj.Entries.Count);
                Tools.update(VoteObj.getEntries());

                int index = 0;

                string[] Names = args.Split(',');

                foreach (string Name in Names)
                {
                    Name.Trim();
                    VoteObj.add(Name);
                }

                await Tools.Reply(e, VoteObj.ToString() + "\nVote using /vote and the ID or name of your choice." ,false);
       
                int c = 3;
            }
            else { await Tools.Reply(e, "You do not have permission."); }


        }

        public async Task EndVote(CommandArgs e)
        {
            if ((Tools.GetPerms(e, e.User)) >= 1000 || ulong.Parse((string)Program.ProgramInfo.DevID) == e.User.Id)
            {
                uint winnerVotes = 0;
                string winner = "";
                String reply = "";
                bool Tie = false;
                VoteObj.setEntries(Tools.currentInfo());
                foreach (VoteObject obj in VoteObj.getEntries())
                {
                    if (obj.Vote == winnerVotes)
                    {
                        winner += " , " + obj.name;
                        Tie = true;
                    }
                    else if (obj.Vote > winnerVotes)
                    {
                        winnerVotes = obj.Vote;
                        winner = obj.name;
                        if (Tie) { Tie = false; }
                    }
                }
                if (Tie) { reply = "The winners were " + winner + " with " + winnerVotes + " votes!"; }
                else { reply = "The winner was " + winner + " with " + winnerVotes + " votes!"; }

                await Tools.Reply(e, ("The voting has ended. Here are your results:\n" + VoteObj.ToString() + reply));

                VoteObj.Entries.RemoveRange(0, VoteObj.Entries.Count);
                Tools.update(VoteObj.getEntries());
            }
            else { await Tools.Reply(e, "You do not have permission."); }

        }

    }

}

