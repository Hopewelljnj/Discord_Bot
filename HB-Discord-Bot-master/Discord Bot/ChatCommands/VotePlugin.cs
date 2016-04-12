using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Discord_Bot.Commands;
using Discord;
using System.IO;
using System.Text.RegularExpressions;

namespace Discord_Bot
{
   
    public class VotePlugin
    {
        
        uint tag = 0;
        
        public List<VoteObject> Entries = new List<VoteObject>();
        
        public List<VoteObject> getEntries()
        {
            return Entries;
        }
        public void setEntries(List<VoteObject> obj)
        {
            Entries = obj;
        }

        public void add(String name)
        {
            List<VoteObject> tempEntries = Tools.currentInfo();
            VoteObject obj2 = new VoteObject(name);
            obj2.ID = tag;
            tempEntries.Add(obj2);
            tag++;
            Tools.update(tempEntries);
        }

        public void remove(String testString)
        {
            VoteObject obj = new VoteObject("");
            foreach (VoteObject var in (Entries = Tools.currentInfo()))
            {
                if (var.name == testString)
                {
                    for (int i = (int)var.ID; i < tag; i++)
                        Entries[i].ID--;

                    Entries.Remove(var);
                    tag--;
                    Tools.update(Entries);
                }
            }
        }

        public void remove(uint testID)
        {
            VoteObject obj = new VoteObject("");
            foreach (VoteObject var in (Entries = Tools.currentInfo()))
            {
                if (var.ID == testID)
                {
                    for (int i = (int)var.ID; i < tag; i++)
                        Entries[i].ID--;

                    Entries.Remove(var);
                    tag--;
                    Tools.update(Entries);
                }
            }
        }

        public void vote(CommandArgs e, User u, String voteName)
        {
            VoteObject obj = new VoteObject("");
            bool found = false;
            foreach (VoteObject var in (Entries = Tools.currentInfo()))
            {
                if(var.name == voteName)
                {
                    vote(e, u, (int)var.ID);
                    found = true;
                }
                if(!found)
                {
                    Tools.Reply(e, "Value not found. Please enter a value from the list. Use /checkVotes to see the list.");
                }
           }
        }

        public void vote(CommandArgs e, User u, int voteID)
        {
            VoteObject obj = new VoteObject("");
            List<VoteObject> Entries2 = Tools.currentInfo();
            List<VoteObject> Entries3 = new List<VoteObject>();
            bool used = false;
            int i = 0;
            foreach(VoteObject voteObj in Entries2)
            {
                obj = voteObj;
                if(voteObj.UsersVoted.Contains(u.Id))
                {
                    voteObj.changeVote(-1);
                    voteObj.changeUser(u,false);
                }
                if(voteObj.ID == voteID)
                {
                    voteObj.changeVote(1);

                    voteObj.changeUser(u,true);
                  
                    
                    Entries2.RemoveAt((i));
                    Entries2.Add(voteObj);

                    Entries3 = Tools.order(Entries2);
                    
                    
                    used = true;
                    break;
                }
                i++;
               
            }
            if (!used) { Tools.Reply(e, "Value not found. Please enter a value from the list. Use /checkVotes to see the list."); }
            else { Tools.Reply(e, "Your vote has been entered."); }

            

            Tools.update(Entries3);
        }

        public override String ToString()
        {
            VoteObject obj = new VoteObject("");
            int idSpace = 2;
            int nameSpace = 4;
            int voteSpace = 5;
            String returnText = "" ;
            Entries = Tools.currentInfo();
            if (Entries.Count == 0)
            {
                return null;
            }

            foreach (VoteObject var in Entries)
            {
                if (var.testID() > idSpace) { idSpace = var.testID(); }
                if (var.testName() > nameSpace) { nameSpace = var.testName(); }
                if (var.testVote() > voteSpace) { voteSpace = var.testVote(); }
            }

            VoteObject topObj = new VoteObject("Name");
            topObj.Vote = 20000;
            topObj.ID = 10;
            string originText = topObj.ToString(idSpace, nameSpace, voteSpace);

            string pattern = "10";
            string replace = "ID";
            string originzText = originText.Replace(pattern, replace);

            string pattern2 = "20000" ;
            string replace2 = "Votes";
            returnText = originzText.Replace(pattern2, replace2);

            returnText = returnText.Replace("*", "");

            string returnText2 = "\n**" + returnText.Trim('*') + "**";

            foreach (VoteObject var in Entries)
            {
                returnText2 += "\n" + var.ToString(idSpace, nameSpace, voteSpace);
            }
            return returnText2;            
        }

    }
}