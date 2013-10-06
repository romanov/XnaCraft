using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace XnaCraft.Engine.Input
{
    class InputState
    {
        public KeyboardState PreviousKeyboardState;
        public KeyboardState CurrentKeyboardState;

        public MouseState PreviousMouseState;
        public MouseState CurrentMouseState;
    }
}
