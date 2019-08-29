using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGEngine.Commands
{
    using RPGEngine.Core;
    using RPGEngine.Discord;

    interface ICommand
    {
        string GetKey();
        void Execute(Command command, IGameEngine engine);
    }
}
