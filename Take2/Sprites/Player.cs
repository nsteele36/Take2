using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;
using FitMi_Research_Puck;

namespace Take2.Sprites
{
    public class Player : Sprite
    {
        private bool isMoving = false;
        private bool isJumping = false;
        private bool isCrouching = false;

        private float crouchTimer = 2;
        private float jumpTimer = 2;
        private int jumpCounter = 0;

        public bool crashed = false;
        public int currentRoad;
        public int obstaclesPassed = 0;

        public bool puckEnabled = false;
        private int puckData1 = 0;
        private int prevPD1 = 0;
        public int puckData2 = 0;
        private int prevPD2 = 0;

        private readonly float max_vel = 25f;
        private readonly float max_jumps = 2;

        private int controlScheme;

        private static HIDPuckDongle _puck;
        private static PuckPacket Puck
        {
            get
            {
                return _puck.PuckPack0;
            }
        }

        private KeyboardState old;
<<<<<<< HEAD
=======

>>>>>>> 2de298d0373233046f72575b0b907ed9338c66a8

        public Player(Texture2D texture) : base(texture) { }

        private void Move(GameTime gameTime)
        {
            this.crouchTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.jumpTimer +=(float)gameTime.ElapsedGameTime.TotalSeconds;

            keyboardMove();
            if(puckEnabled)
                getPuckData();
            
            //CROUCH COOLDOWN
            if (isCrouching && crouchTimer >= 1.0f)
                Uncrouch();

            //JUMP COOLDOWN
<<<<<<< HEAD
            if (isJumping && jumpTimer >= 1.5f && jumpCounter == max_jumps)
=======
            if (isJumping && jumpTimer >= 2f && jumpCounter == max_jumps)
>>>>>>> 2de298d0373233046f72575b0b907ed9338c66a8
            {
                isJumping = false;
                jumpCounter = 0; 
            }

<<<<<<< HEAD
            if(this.body.LinearVelocity.X != max_vel && this.body.Position.X != -29f)
=======
            if(this.body.LinearVelocity.X != max_vel && this.body.Position.X != -99f)
>>>>>>> 2de298d0373233046f72575b0b907ed9338c66a8
                crashed = true;
        }

        private void Jump()
        {
            if(jumpCounter == 0)
            {
                float impulse = this.body.Mass * 12;
                this.body.ApplyLinearImpulse(new Vector2(0, impulse), this.body.WorldCenter);
            }
            else if(jumpCounter == 1)
            {
                float impulse = this.body.Mass * 10;
                this.body.ApplyLinearImpulse(new Vector2(0, impulse), this.body.WorldCenter);
            }
            this.isJumping = true;
            jumpTimer = 0;
            jumpCounter++;
        }

        //CREATE CROUCH HITBOX 
        private void Crouch()
        {
            Vector2 playerPosition = body.Position;
            Vector2 prevVel = body.LinearVelocity;
            world.Remove(body);
            bodySize = new Vector2(1f, 0.5f);
            body = world.CreateRectangle(bodySize.X, bodySize.Y, 1f, playerPosition);
            body.BodyType = BodyType.Dynamic;
            body.SetRestitution(0.0f);
            body.SetFriction(0.0f);
            body.LinearVelocity = prevVel;
            textureSize = new Vector2(texture.Width, texture.Height);
            textureOrigin = textureSize / 2f;
            isCrouching = true;
            crouchTimer = 0;
        }

        //CREATE STANDING HITBOX
        private void Uncrouch()
        {
            Vector2 playerPosition = body.Position;
            Vector2 prevVel = body.LinearVelocity;
            world.Remove(body);
            bodySize = new Vector2(1f, 1f);
            body = world.CreateRectangle(bodySize.X, bodySize.Y, 1f, playerPosition);
            body.BodyType = BodyType.Dynamic;
            body.SetRestitution(0.0f);
            body.SetFriction(0.0f);
            body.LinearVelocity = prevVel;
            textureSize = new Vector2(texture.Width, texture.Height);
            textureOrigin = textureSize / 2f;
            isCrouching = false;
        }

        public void Update(GameTime gameTime)
        {
            Move(gameTime);
            if (body.Position.Y < -15)
                crashed = true;
        }

        public void keyboardMove()
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Right) && !crashed && !isMoving)
            {
                body.LinearVelocity = new Vector2(25f, 0);
                isMoving = true;
            }

            //CROUCH
            if (state.IsKeyDown(Keys.Down) && !crashed)
                Crouch();

            //JUMP
            if (state.IsKeyUp(Keys.Up) && old.IsKeyDown(Keys.Up) && jumpCounter < max_jumps && !crashed)
                Jump();

            old = state;
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
                Console.WriteLine("Failed to initialize puck\n" + e);
                return false;
            }
<<<<<<< HEAD
            controlScheme = 2;
=======
            controlScheme = 1;
>>>>>>> 2de298d0373233046f72575b0b907ed9338c66a8
            return true;
        }

        private void getPuckData()
        {
            _puck.CheckForNewPuckData();
<<<<<<< HEAD
            if (controlScheme == 1)
=======
            if(controlScheme == 0)
>>>>>>> 2de298d0373233046f72575b0b907ed9338c66a8
            {
                puckData1 = _puck.PuckPack0.Accelerometer[1];
                puckData2 = _puck.PuckPack0.Loadcell;
            }
<<<<<<< HEAD
            else if (controlScheme == 2)
=======
            else if(controlScheme == 1)
>>>>>>> 2de298d0373233046f72575b0b907ed9338c66a8
            {
                puckData1 = _puck.PuckPack0.Loadcell;
                puckData2 = _puck.PuckPack0.Accelerometer[1];
            }
<<<<<<< HEAD
            else if (controlScheme == 3)
            {
                puckData1 = _puck.PuckPack0.Loadcell;
                puckData2 = _puck.PuckPack1.Loadcell;
            }
=======
>>>>>>> 2de298d0373233046f72575b0b907ed9338c66a8

            puckMove();
            prevPD1 = puckData1;
            prevPD2 = puckData2;
        }

        private void puckMove()
        {
<<<<<<< HEAD
            if(controlScheme == 1)
=======
            if(controlScheme == 0)
>>>>>>> 2de298d0373233046f72575b0b907ed9338c66a8
            {
                if (puckData1 > -400 && prevPD1 < -400 && !crashed)
                    Crouch();

<<<<<<< HEAD
                if (puckData1 < 400 && prevPD1 > 400 && jumpCounter < max_jumps && !crashed)
                    Jump();

               // if (puckData2 < -400 && !crashed && !isMoving)
                if (puckData2 > 550 && !crashed && !isMoving)
=======

                if (puckData1 < 400 && prevPD1 > 400 && jumpCounter < max_jumps && !crashed)
                    Jump();


                //if (puckData2 < -400 && !crashed && !isMoving)
                if (puckData2 > 500 && !crashed && !isMoving)
>>>>>>> 2de298d0373233046f72575b0b907ed9338c66a8
                {
                    this.body.LinearVelocity = new Vector2(25f, 0);
                    isMoving = true;
                }
            }
<<<<<<< HEAD
            else if(controlScheme == 2)
=======
            else if(controlScheme == 1)
>>>>>>> 2de298d0373233046f72575b0b907ed9338c66a8
            {
                if (puckData2 > -400 && prevPD2 < -400 && !crashed)
                    Crouch();

<<<<<<< HEAD
                if (puckData1 < 520 && prevPD1 > 520 && jumpCounter < max_jumps && !crashed)
                    Jump();

                if (puckData2 < -400 && !crashed && !isMoving)
                //if (puckData2 > 400 && !crashed && !isMoving)
                {
                    this.body.LinearVelocity = new Vector2(25f, 0);
                    isMoving = true;
                }
            }

            else if(controlScheme == 3)
            {
                if (puckData1 > 520 && prevPD1 < 520 && jumpCounter < max_jumps && !crashed)
                    Jump();

                if (puckData2 > 520 && prevPD2 < 520 && !crashed)
                    Crouch();

                // if (puckData2 < -400 && !crashed && !isMoving)
                if (puckData1 + puckData2 > 1040 && !crashed && !isMoving)
=======

                if (puckData1 < 520 && prevPD1 > 520 && jumpCounter < max_jumps && !crashed)
                    Jump();


                //if (puckData2 < -400 && !crashed && !isMoving)
                if (puckData2 > 400 && !crashed && !isMoving)
>>>>>>> 2de298d0373233046f72575b0b907ed9338c66a8
                {
                    this.body.LinearVelocity = new Vector2(25f, 0);
                    isMoving = true;
                }
            }
        }
    }
}
