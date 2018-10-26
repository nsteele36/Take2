using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using FitMi_Research_Puck;
using System.Collections.Generic;
using Take2.Models;

namespace Take2.Sprites
{
    public class Player : Sprite
    {
        public bool isMoving = false;
        private bool isJumping = false;
        private bool isCrouching = false;
        public bool isOnRoad = false;

        private float crouchTimer = 2;
        private float jumpTimer = 2;
        private int jumpCounter = 0;
        private int numOfJumps = 0;
        private int numOfCrouches = 0;

        public bool crashed = false;
        public int currentRoad;
        public int obstaclesPassed = 0;

        public bool puckEnabled = false;
        private int puckData1 = 0;
        private int prevPD1 = 0;
        public int puckData2 = 0;
        private int prevPD2 = 0;

        private readonly float starting_pos = -29f;
        private readonly float max_vel = 25f;
        private readonly float max_jumps = 2;

        private int controlScheme;

        public float score;

        private static HIDPuckDongle _puck;
        private static PuckPacket Puck
        {
            get
            {
                return _puck.PuckPack0;
            }
        }

        private KeyboardState old;

        public Player(Texture2D texture) : base(texture) {
            Input = new Input()
            {
                Left = Keys.Left,
                Right = Keys.Right,
                Up = Keys.Up,
                Down = Keys.Down,
                Jump = Keys.Space,
            };
            color = Color.White;
            textureSize = new Vector2(texture.Width, texture.Height);
            textureOrigin = textureSize / 2f;
            puckEnabled = initializePuck();
            currentRoad = 2;
            score = 0;
        }

        public void SetPlayerPhysics(World w)
        {
            world = w;
            bodySize = new Vector2(1.65f, 3f);
            Vector2 playerPosition = new Vector2(-29f, 1.5f);
            body = world.CreateRectangle(bodySize.X, bodySize.Y, 1f, playerPosition);
            body.BodyType = BodyType.Dynamic;
            body.SetRestitution(0f);
            body.SetFriction(0f);
            body.OnCollision += Collision;
            body.SetCollisionGroup(0);
            //body.Mass = 75f;
            body.FixedRotation = true;
        }

        public void Update(GameTime gameTime)
        {
            if (world.ContactCount == 0)
                isOnRoad = false;
            else
                isOnRoad = true;

            if (world.ContactCount > 2)
                crashed = true;

            Move(gameTime);

            //fall off map condition
            if (body.Position.Y < -15)
                crashed = true;
        }

        private void Move(GameTime gameTime)
        {
            //if(body.LinearVelocity.X < max_vel && !crashed && this.body.Position.X != starting_pos)
              //  body.LinearVelocity = new Vector2(max_vel, 0f);
            
            this.crouchTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.jumpTimer +=(float)gameTime.ElapsedGameTime.TotalSeconds;

            keyboardMove();
            if(puckEnabled)
                getPuckData();
            
            //CROUCH COOLDOWN
            if (isCrouching && crouchTimer >= 1.0f)
                Uncrouch();

            //JUMP COOLDOWN

            if (isJumping && jumpTimer >= 0.7f && jumpCounter == max_jumps)
            {
                isJumping = false;
                jumpCounter = 0; 
            }

            if(this.body.LinearVelocity.X < max_vel / 2 && this.body.Position.X != starting_pos)
            {
                isMoving = false;
                crashed = true;
            }

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
            numOfJumps++;
        }

        //CREATE CROUCH HITBOX 
        private void Crouch()
        {
            Vector2 playerPosition = body.Position;
            playerPosition.Y -= 0.15f;
            Vector2 prevVel = body.LinearVelocity;
            world.Remove(body);
            bodySize = new Vector2(1.65f, 1.5f);
            body = world.CreateRectangle(bodySize.X, bodySize.Y, 1f, playerPosition);
            body.BodyType = BodyType.Dynamic;
            body.SetRestitution(0.0f);
            body.SetFriction(0.0f);
            body.LinearVelocity = prevVel;
            body.FixedRotation = true;
            textureSize = new Vector2(texture.Width, texture.Height);
            textureOrigin = textureSize / 2f;
            isCrouching = true;
            crouchTimer = 0;
            numOfCrouches++;
        }

        //CREATE STANDING HITBOX
        private void Uncrouch()
        {
            Vector2 playerPosition = body.Position;
            Vector2 prevVel = body.LinearVelocity;
            world.Remove(body);
            bodySize = new Vector2(1.65f, 3f);
            body = world.CreateRectangle(bodySize.X, bodySize.Y, 1f, playerPosition);
            body.BodyType = BodyType.Dynamic;
            body.SetRestitution(0.0f);
            body.SetFriction(0.0f);
            body.LinearVelocity = prevVel;
            body.FixedRotation = true;
            textureSize = new Vector2(texture.Width, texture.Height);
            textureOrigin = textureSize / 2f;
            isCrouching = false;
        }

        public void keyboardMove()
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Right) && !crashed && !isMoving)
            {
                body.LinearVelocity = new Vector2(max_vel, 0);
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
            controlScheme = 3;

            return true;

        }

        private void getPuckData()
        {
            _puck.CheckForNewPuckData();
            if (controlScheme == 1)
            {
                puckData1 = _puck.PuckPack0.Accelerometer[1];
                puckData2 = _puck.PuckPack0.Loadcell;
            }
            else if (controlScheme == 2)
            {
                puckData1 = _puck.PuckPack0.Loadcell;
                puckData2 = _puck.PuckPack0.Accelerometer[1];
            }
            else if (controlScheme == 3)
            {
                puckData1 = _puck.PuckPack0.Loadcell;
                puckData2 = _puck.PuckPack1.Loadcell;
            }

            puckMove();
            prevPD1 = puckData1;
            prevPD2 = puckData2;
        }

        private void puckMove()
        {
            if(controlScheme == 1)
            {
                if (puckData1 > -400 && prevPD1 < -400 && !crashed)
                    Crouch();

                if (puckData1 < 400 && prevPD1 > 400 && jumpCounter < max_jumps && !crashed)
                    Jump();

                //if (puckData2 < -400 && !crashed && !isMoving)
                if (puckData2 > 500 && !crashed && !isMoving)
                {
                    this.body.LinearVelocity = new Vector2(25f, 0);
                    isMoving = true;
                }
            }
            else if(controlScheme == 2)
            {
                if (puckData2 > -400 && prevPD2 < -400 && !crashed)
                    Crouch();

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
                {
                    this.body.LinearVelocity = new Vector2(25f, 0);
                    isMoving = true;
                }
            }
        }
         
        public void getCurrentRoad(List<Road> _road1, List<Road> _road2, List<Road> _road3)
        {
            if (body.Position.Y > _road1[0].body.Position.Y && body.Position.Y < _road2[0].body.Position.Y)
                currentRoad = 1;
            if (body.Position.Y > _road2[0].body.Position.Y)
                currentRoad = 2;
            else if (body.Position.Y > _road3[0].body.Position.Y && body.Position.Y < _road1[0].body.Position.Y)
                currentRoad = 3;
        }

        public bool Collision(Fixture a, Fixture b, Contact c)
        {
           //obstacle collision
            if (b.CollisionGroup == 1)
                crashed = true;
           return true;
        }
    }
}
