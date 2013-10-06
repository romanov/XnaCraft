using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaCraft.Engine.Input.Commands;

namespace XnaCraft.Engine.Input
{
    interface ICommand
    {
        bool WasInvoked(InputState context);

        void Execute();
    }
}
