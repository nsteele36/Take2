using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;
using FitMi_Research_Puck;

namespace Take2.Sprites
{
    public class Player : Sprite
    {
        private bool isMoving = true;
        private bool isJumping = false;
        private bool isCrouching = false;
        private float crouchTimer = 2;
        private float jumpTimer = 2;
        public bool puckEnabled = false;
        private int puckData1 = 0;
        private int puckData2 = 0;

        private static HIDPuckDongle _puck;
        private static PuckPacket Puck
        {
            get
            {
                return _puck.PuckPack0;
            }
        }


        public Player(Texture2D texture) : base(texture) { }

        private void Move(GameTime gameTime)
        {
            this.crouchTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.jumpTimer +=(float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState state = Keyboard.GetState();

            if(puckEnabled)
            {
                _puck.CheckForNewPuckData();
                puckData1 = _puck.PuckPack0.Gyrometer[2];
                puckData2 = _puck.PuckPack0.Gyrometer[1];
            }

            
            if (state.IsKeyDown(Keys.Right) || puckData2 < -500 && isMoving )
            {
                this.body.LinearVelocity = new Vector2(25f, 0);
                isMoving = true;
            }
            
            //CROUCH COOLDOWN
            if(isCrouching && crouchTimer >= 1.0f)
                Uncrouch();

            //JUMP COOLDOWN
            if (isJumping && jumpTimer >= 2.1f)
                isJumping = false;

            //CROUCH
            if (state.IsKeyDown(Keys.Down) )
            { 
                Crouch();
                crouchTimer = 0;
            }

             
            if(puckData1 > 500)
            {
                Crouch();
                crouchTimer = 0;
            }

            //JUMP
            if (state.IsKeyDown(Keys.Up) && !isJumping)
            {
                Jump();
                jumpTimer = 0;
            }

            if(puckData1 < -500 && !isJumping)
            {
                {
                    Jump();
                    jumpTimer = 0;
                }
            }
        }

        private void Jump()
        {
            float impulse = this.body.Mass * 15;
            this.body.ApplyLinearImpulse(new Vector2(0, impulse), this.body.WorldCenter);
            this.isJumping = true;
        }

        //CREATE CROUCH HITBOX 
        private void Crouch()
        {
            Vector2 playerPosition = this.body.Position;
            Vector2 prevVel = this.body.LinearVelocity;
            this.world.Remove(this.body);
            this.bodySize = new Vector2(1f, 0.5f);
            this.body = world.CreateRectangle(this.bodySize.X, this.bodySize.Y, 1f, playerPosition);
            this.body.BodyType = BodyType.Dynamic;
            this.body.SetRestitution(0.0f);
            this.body.SetFriction(0.0f);
            this.body.LinearVelocity = prevVel;
            this.textureSize = new Vector2(this.texture.Width, this.texture.Height);
            this.textureOrigin = this.textureSize / 2f;
            this.isCrouching = true;
        }

        //CREATE STANDING HITBOX
        private void Uncrouch()
        {
            Vector2 playerPosition = this.body.Position;
            Vector2 prevVel = this.body.LinearVelocity;
            this.world.Remove(this.body);
            this.bodySize = new Vector2(1f, 1f);
            this.body = world.CreateRectangle(this.bodySize.X, this.bodySize.Y, 1f, playerPosition);
            this.body.BodyType = BodyType.Dynamic;
            this.body.SetRestitution(0.0f);
            this.body.SetFriction(0.0f);
            this.body.LinearVelocity = prevVel;
            this.textureSize = new Vector2(this.texture.Width, this.texture.Height);
            this.textureOrigin = this.textureSize / 2f;
            this.isCrouching = false;
        }

        public void Update(GameTime gameTime)
        {
            Move(gameTime);
        }

        public bool initializePuck()
        {
            try
            {
                _puck = new HIDPuckDongle();
                _puck.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to initialize puck");
                return false;
            }

            return true;
        }
    }
}
