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
        private Texture2D texture;
        private Body body;
        private Vector2 textureSize;
        private Vector2 textureOrigin;
        private Vector2 bodySize;
        private Color color;
        private Input Input;
        private Vector2 pos;
        private World world;
        public Rectangle Rectangle
        { get { return new Rectangle((int)pos.X, (int)pos.Y, texture.Width, texture.Height);} }

        public Sprite(Texture2D tex){ texture = tex; }

        public virtual void Update(GameTime gameTime, Sprite sprite){}

        public virtual void Draw(SpriteBatch sb) { sb.Draw(texture, this.body.Position, color); }

        //ACCESSORS
        public Texture2D getTexture() { return texture; }
        public Body getBody() { return body; }
        public Vector2 getTextureSize() { return textureSize; }
        public Vector2 getTextureOrigin() { return textureOrigin; }
        public Vector2 getBodySize() { return bodySize; }
        public Color getColor() { return color; }
        public Input getInput() { return Input; }
        public Vector2 getPosition() { return pos; }
        public World getWorld() { return world; }

        //MUTATORS
        public void setTexture(Texture2D t) { texture = t; }
        public void setBody(Body b) { body = b; }
        public void setTextureSize(Vector2 v) { textureSize = v; }
        public void setTextureOrigin(Vector2 v) { textureOrigin = v; }
        public void setBodySize(Vector2 v) { bodySize = v; }
        public void setColor(Color c) { color = c; }
        public void setInput(Input i) { Input = i; }
        public void setPosition(Vector2 v) { pos = v; }
        public void setWorld(World w) { world = w; }
    }

}
