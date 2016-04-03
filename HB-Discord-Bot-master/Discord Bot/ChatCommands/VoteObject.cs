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
//using System.Web.Script.Serialization;
using Newtonsoft.Json;


namespace Discord_Bot
{
    public class VoteObject
    {
        public uint ID = 0;
        public String name;
        public uint Vote = 0;
        public List<ulong> UsersVoted = new List<ulong>();
        public VoteObject(String name)
        {
            this.name = name;
        }

        public void changeVote(int change)
        {
            Vote = Vote + (uint)change;
        }

        public void changeUser(User u, bool add)
        {
            if (add) {UsersVoted.Add(u.Id);
            }else{ UsersVoted.Remove(u.Id); }
        }

        public String ToString(int spaceID, int spaceName, int spaceVote)
        {
            int idSpace, nameSpace, voteSpace, idHalfSpace, nameHalfSpace, voteHalfSpace = 0;
            bool idOdd, nameOdd, voteOdd;
            String idString, nameString, voteString, idReturn, nameReturn, voteReturn;
            ///Covert to Strings
            idString = ID.ToString();
            nameString = name;
            voteString = Vote.ToString();
            ///Find extra space needed
            idSpace = spaceID - idString.Length;
            nameSpace = spaceName - nameString.Length;
            voteSpace = spaceVote - voteString.Length;
            ///Test if even or odd and find halfSpace
            /// Currently One lined to save space. ** May want to expand later**
            if (idSpace % 2 == 1) { idOdd = true; idHalfSpace = (idSpace - 1) / 2; } else { idOdd = false; idHalfSpace = idSpace / 2; }
            if (nameSpace % 2 == 1) { nameOdd = true; nameHalfSpace = (nameSpace - 1) / 2; } else { nameOdd = false; nameHalfSpace = nameSpace / 2; }
            if (voteSpace % 2 == 1) { voteOdd = true; voteHalfSpace = (voteSpace - 1) / 2; } else { voteOdd = false; voteHalfSpace = voteSpace / 2; }
            ///Sets individual return Strings
            idReturn = strings(idOdd, idString, idHalfSpace);
            nameReturn = strings(nameOdd, nameString, nameHalfSpace);
            voteReturn = strings(voteOdd, voteString, voteHalfSpace);
            ///Prints the strings
            return idReturn + "**|**" + nameReturn + "**|**" + voteReturn;

        }

        public String strings(bool currentBool, String currentString, int numSpaces)
        {
            String returnString = "" ;
            if (currentBool) { returnString += " "; }
            for (int i = 0; i < numSpaces; i++) { returnString += " "; }
            returnString += currentString;
            for (int i = 0; i < numSpaces; i++) { returnString += " "; }
            return returnString;
        }

        public int testID() { return (ID.ToString().Length); }
        public int testName() { return (name.Length); }
        public int testVote() { return (Vote.ToString().Length); }


        


    }
}