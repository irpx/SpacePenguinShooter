using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer3D
{
    public class Shaders
    {
        public static Dictionary<String,Effect> list;

        public Shaders()
        {
            Shaders.list = new Dictionary<string, Effect>();

            list.Add("test", Game1.Instance.Content.Load<Effect>("shaders/test"));
        }
    }
}
