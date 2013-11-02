﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaCraft.Engine.Diagnostics;
using XnaCraft.GameLogic.InputCommands;

namespace XnaCraft.Engine.Input
{
    class InputController : IUpdateLogic
    {
        private readonly IEnumerable<IInputHandler> _inputHandlers;
        private readonly IEnumerable<IInputCommand> _commands;
        private readonly InputState _inputState = new InputState();

        public InputController(IEnumerable<IInputHandler> inputHandlers, IEnumerable<IInputCommand> commands)
        {
            _inputHandlers = inputHandlers;
            _commands = commands;
        }

        public void OnUpdate(GameTime gameTime)
        {
            _inputState.CurrentKeyboardState = Keyboard.GetState();
            _inputState.CurrentMouseState = Mouse.GetState();

            ExecuteLogicScripts(gameTime);
            ExecuteCommands();

            _inputState.PreviousKeyboardState = _inputState.CurrentKeyboardState;
            _inputState.PreviousMouseState = _inputState.CurrentMouseState;
        }

        private void ExecuteLogicScripts(GameTime gameTime)
        {
            foreach (var handler in _inputHandlers)
            {
                handler.HandleInput(gameTime, _inputState);
            }
        }

        private void ExecuteCommands()
        {
            foreach (var command in _commands)
            {
                if (command.WasInvoked(_inputState))
                {
                    command.Execute();
                }
            }
        }
    }
}
