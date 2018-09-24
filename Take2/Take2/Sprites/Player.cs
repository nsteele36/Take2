using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Take2.Sprites
{
    public class Player : Sprite
    {
        public KeyboardState oldKeyState;

        public Player(Texture2D texture) : base(texture) { }

        private void Move()
        {
            KeyboardState state = Keyboard.GetState();
            // Move camera
            if (state.IsKeyDown(Keys.Left))
            {
                //_cameraPosition.X -= totalSeconds * cameraViewWidth;
                this.body.ApplyForce(new Vector2(-100, 0), this.body.WorldCenter);
            }


            if (state.IsKeyDown(Keys.Right))
            {
                this.body.ApplyForce(new Vector2(100, 0), this.body.WorldCenter);
                //_cameraPosition.X += totalSeconds * cameraViewWidth;
            }


            if (state.IsKeyDown(Keys.Up))
            {
                this.body.ApplyForce(new Vector2(0, 50), this.body.WorldCenter);
                //_cameraPosition.Y += totalSeconds * cameraViewWidth;
            }


            if (state.IsKeyDown(Keys.Down))
                this.body.ApplyForce(new Vector2(0, -50), this.body.WorldCenter);
            //_cameraPosition.Y -= totalSeconds * cameraViewWidth;

            if (state.IsKeyDown(Keys.Space) && oldKeyState.IsKeyUp(Keys.Space))
            {
                float impulse = this.body.Mass * 10;
                this.body.ApplyLinearImpulse(new Vector2(0, impulse), this.body.WorldCenter);
            }

            oldKeyState = state;
        }

        public override void Update(GameTime gameTime, Sprite s)
        {
            //Move();
            //this.pos = this.body.Position;
            /*
            if(this.pos.Y >= this.intialPos.Y)
            {
                this.pos.Y = this.intialPos.Y;
                //this.vel.Y = 0f;

            }

            //this.pos.Y = Math.Max(min_player_y, Math.Min(max_player_y, this.pos.Y));

            this.pos.Y = Math.Min(this.pos.Y, this.intialPos.Y);
            //this.vel.X = Math.Max(0, Math.Min(100, this.vel.X));           

            if (this.vel.X > 0 && this.IsTouchingLeft(s) || this.vel.X < 0 && this.IsTouchingRight(s))
                    this.vel.X = 0;

            if (this.vel.Y > 0 && this.IsTouchingTop(s) || this.vel.Y < 0 && this.IsTouchingBottom(s))
                    this.vel.Y = 0;
              */

        }
    }
}
