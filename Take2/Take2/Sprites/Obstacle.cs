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
        public void Initialize(Texture2D texture, Vector2 newPosition, float speed, bool vis) 
        {
            this.texture = texture;
            this.isVisible = vis;
            
        }
        public override void Update(GameTime gameTime, Sprite s)
        {
            //this.body.Position.X -= this.body.;
            if (this.body.Position.X <= -texture.Width) isVisible = false;
            
            /*
            if (this.vel.X > 0 && this.IsTouchingLeft(s) || this.vel.X < 0 && this.IsTouchingRight(s))
                this.vel.X = 0;

            if (this.vel.Y > 0 && this.IsTouchingTop(s) || this.vel.Y < 0 && this.IsTouchingBottom(s))
                this.vel.Y = 0;*/
                
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible)
                spriteBatch.Draw(texture, this.body.Position, null, Color.Red, 0.0f,
                    new Vector2(0, texture.Height / 2), Vector2.One, SpriteEffects.None, 0.0f);
        }

    }
}
