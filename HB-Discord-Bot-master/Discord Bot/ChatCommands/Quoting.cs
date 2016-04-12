using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Discord_Bot.Commands;
using Discord;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;


namespace Discord_Bot
{
    public class Quoting
    {
        public static string PathToQuotes = "../LocalFiles/Quotes.json";

        public static Func<CommandArgs, Task> createQuote = async e =>
        {
            User u = e.User;
            bool FoundUser = false;
            string[] parameters = e.Args;
            string quoteString = "";
            foreach(User use in e.Server.Users)
            {
                if(parameters[parameters.Length - 1] == ("@" + use.Name))
                {
                    u = use;
                    FoundUser = true;
                    break;
                }
            }

            string quote = "";
            string messageTime = "";
            if(u == null){quote = e.ArgText;}
            else {
                parameters.SetValue("", parameters.Length - 1);
                foreach (string q in parameters){ quote += q + " "; }
                quote = quote.TrimEnd(' ');}
                
            foreach(Message message in e.Channel.Messages)
            {
                if(message.Text.ElementAt(0) == '/') { continue; }
                string mString = message.Text;
                if (mString.Contains(quote))
                {
                    if(!FoundUser)
                    {
                        await e.User.SendMessage("Did " + u.Mention + " say the quote? If yes please put type `/createQuote quote @" + u.ToString() + "` with the quote being your quote.");
                        return;
                    }
                    else
                    {
                        if(message.User == u)
                        {
                            quoteString = mString;
                            messageTime = message.Timestamp.Date.ToString();
                            break;
                        }
                    }
                }
            }
            string quoteID = getQuoteID(e);
            if(quoteID == "") { await e.User.SendMessage("Unknown error. Please try again. If this is the second time its failed please message Hopewell"); }
            string finalString = "";
            if(quoteString == "") {await e.User.SendMessage("Quote Not Found. Please copy and paste the entire text of the quote with the user at the end with an @ in front."); }
            
            finalString +="|\"" + quoteString + "\" - " + u.Name + " on " + messageTime + "," + quoteID;
            
            Tools.SaveFile(finalString, PathToQuotes, true);
            await Tools.Reply(e,"Quote saved as quote #" + quoteID);

        };

        public static string getQuoteID(CommandArgs e)
        {
            string idString = Tools.ReadFile(PathToQuotes);
            idString = idString.Replace("\r", "");
            idString = idString.Replace("\n", "");
            if (idString == "") { return "1"; }
            char[] idArray = idString.ToCharArray();
            List<uint> IDs = new List<uint>();
            for(int i = idArray.Length - 1; i >= 0; i--)
            {
                uint c = 0;
                if (uint.TryParse(idArray[i].ToString(), out c))
                {
                    IDs.Add(c);
                }
                else { break; }
            }

            uint prevID;
            string prevIDs = "" ;
            foreach(uint i in IDs)
            {
                prevIDs += i.ToString();
            }
            if(!uint.TryParse(prevIDs, out prevID)) { return "" ; }

            uint currID = prevID + 1;

            return currID.ToString();
        }

        public static Func<CommandArgs, Task> quote = async e =>
        {
            uint quoteID = 0;
            uint quoteIDTest = 0;
            bool done = false;
            if(!uint.TryParse(e.ArgText,out quoteID) || e.ArgText == "0")
            {
                await Tools.Reply(e, "Incorrect arguments. /quote #");
            }
            string mainString = Tools.ReadFile(PathToQuotes);
            string[] quoteStrings = mainString.Split('|');
            foreach (string quoteString in quoteStrings)
            {
                string quoteString2 = quoteString.Replace("|", "");
                quoteString2 = quoteString2.Replace("\r", "");
                quoteString2 = quoteString2.Replace("\n", "");
                if (quoteString2 == "") { continue; }

                string[] splits = quoteString2.Split(',');
                if (!uint.TryParse(splits[1], out quoteIDTest)) { await Tools.Reply(e, "Error with Quote document. Contacting developer.");await e.Server.GetUser(146275186142871552).SendMessage("Error with Quote Document Bad ID"); }
                if (quoteIDTest == quoteID)
                {
                    await e.Channel.SendMessage(splits[0]);
                    done = true;
                    break;
                }
            }
            if (!done) { await Tools.Reply(e, "The quote specified does not exist."); }
        };

        public static Func<CommandArgs, Task> deleteQuote = async e =>
        {
            uint quoteID = 0;
            if(!uint.TryParse(e.ArgText,out quoteID)) {await e.User.SendMessage("Arguments incorrect. /deleteQuote #"); }
            string quotes = Tools.ReadFile(PathToQuotes);
            string finalString = "";
            bool changeId = false;
            string[] quoteStrings = quotes.Split('|');
            foreach(string quoteString in quoteStrings)
            {
                if(quoteString == "") { continue; }
                string[] arguments = quoteString.Split(',');
                if (!arguments[1].Contains(quoteID.ToString()))
                {
                    if (changeId)
                    {
                        string temp = arguments[1].Replace("\r", "");
                        temp = temp.Replace("\n", "");
                        int tempint = 0;
                        int.TryParse(temp, out tempint);
                        temp = (tempint - 1).ToString();
                        finalString += arguments[0] + "," + temp + "\r\n|";
                        continue;
                    }

                    finalString += quoteString;
                }
                else { changeId = true; }
            }
            await e.User.SendMessage("Quote " + quoteID +" successfully deleted.");
            Tools.SaveFile(finalString, PathToQuotes, false);
        };
        public static Func<CommandArgs, Task> getQuotes = async e =>
        {
            string quotes = Tools.ReadFile(PathToQuotes);
            string returnString = "";
            string quoteString2 = quotes.Replace("\r", "");
            quoteString2 = quoteString2.Replace("\n", "");

            string[] quoteList = quoteString2.Split('|');

            foreach(string quote in quoteList)
            {
                if(quote == "") { continue; }
                string[] arguments = quote.Split(',');
                returnString += arguments[0] + Environment.NewLine;
            }
            await e.User.SendMessage(returnString);
        };

    }
}



