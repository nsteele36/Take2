using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Take2.Sprites
{
    public class Obstacle : Sprite
    {
        //public List<Vector2> obstacles;
        public bool isVisible;

        public Obstacle(Texture2D texture) : base(texture) { }
    }
}
