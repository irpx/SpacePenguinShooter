using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Platformer3D
{
    public class Input
    {
        public static GamePadState pad;
        public static KeyboardState key;
        private static GamePadState lastPad;
        private static KeyboardState lastKey;

        public Input()
        {
            
        }
        public static void Update()
        {
            Input.lastPad = Input.pad;
            Input.lastKey = Input.key;

            Input.pad = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
            Input.key = Keyboard.GetState();
        }
        public static bool keyPressedDown(Keys key)
        {
            if (Input.lastKey.IsKeyUp(key) && Input.key.IsKeyDown(key)) return true;
            return false;
        }
        public static bool buttonPressedFirsttime(Buttons button)
        {
            if (Input.lastPad.IsButtonUp(button) && Input.pad.IsButtonDown(button)) return true;
            return false;
        }
    }
}
