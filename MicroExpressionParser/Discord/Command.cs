using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGEngine.Discord
{
    using System.Text.RegularExpressions;

    public class Command
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string[] Args { get; set; }

        public override string ToString()
        {
            string args = "";
            foreach (var arg in Args)
            {
                args +=  $"\"{arg}\" ";
            }
            return $"{Name} {args.Trim()}";
        }

        public static Command FromMessage(long user, string message)
        {
            List<string> parts = message.Split('"')
                                     .Select((element, index) => index % 2 == 0  // If even index
                                                                     ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
                                                                     : new string[] { element })  // Keep the entire item
                                     .SelectMany(element => element).ToList();
            string command = parts[0];
            parts.RemoveAt(0);
            return new Command() { UserId = user,Name = command, Args = parts.ToArray()}; 
        }
    }
}
