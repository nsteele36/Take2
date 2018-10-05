using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Take2.Models;
using tainicom.Aether.Physics2D.Dynamics;

namespace Take2.Sprites
{
    public class Sprite
    {
        public Texture2D texture;
        public Body body;
        public Vector2 textureSize;
        public Vector2 textureOrigin;
        public Vector2 bodySize;
        public Color color;
        public Input Input;
        public Vector2 pos;
        public World world;
        public Rectangle Rectangle
        { get { return new Rectangle((int)pos.X, (int)pos.Y, texture.Width, texture.Height);} }

        public Sprite(Texture2D tex){ texture = tex; }

        public virtual void Update(GameTime gameTime, Sprite sprite){}

        public virtual void Draw(SpriteBatch sb) { sb.Draw(texture, this.body.Position, color); }

    }

}
