using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaCraft.Engine.Input
{
    interface IInputCommand
    {
        bool WasInvoked(InputState context);

        void Execute();
    }
}
