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
        private enum playerPosition { MIDDLE = 1, TOP, BOTTOM, OUTOFBOUNDS};
        private bool isMoving = false;
        private bool isJumping = false;
        private bool isCrouching = false;
        private bool isOnRoad = false;
        private bool isOutOfBounds = false;

        private float crouchTimer = 2;
        private float jumpTimer = 2;
        private int jumpCounter = 0;
        private int numOfJumps = 0;
        private int numOfCrouches = 0;

        private bool crashed = false;
        private int currentRoad;
        private int obstaclesPassed = 0;
        private bool passed = false;
        private float passedTime = 0f;

        public bool puckEnabled = false;
        private int puckData1 = 0;
        private int prevPD1 = 0;
        private int puckData2 = 0;
        private int prevPD2 = 0;

        private readonly float starting_pos = -29f;
        private readonly float max_vel = 25f;
        private readonly float max_jumps = 2;

        private int controlScheme;

        private float score;
        private KeyboardState old;

        private static HIDPuckDongle _puck;
        private static PuckPacket Puck
        {
            get
            {
                return _puck.PuckPack0;
            }
        }

        public bool getIsOnRoad() { return isOnRoad; }
        public bool getIsMoving() { return isMoving; }
        public bool getIsCrashed() { return crashed; }
        public int getCurrentRoad() { return currentRoad; }
        public int getObstaclesPassed() { return obstaclesPassed; }
        public void setObstaclesPassed(int i) { obstaclesPassed = i; }
        public bool getIsPassed() { return passed; }
        public void setIsPassed(bool b) { passed = b; }
        public float getPassedTime() { return passedTime; }
        public void setPassedTime(float f) { passedTime = f; }
        public float getPuckData2() { return puckData2; }
        public float getScore() { return score; }
        public void setScore(float f) { score = f; }
        public bool getIsOutOfBounds() { return isOutOfBounds; }

        public Player(Texture2D texture) : base(texture) {
            Input i = new Input()
            {
                Left = Keys.Left,
                Right = Keys.Right,
                Up = Keys.Up,
                Down = Keys.Down,
                Jump = Keys.Space,
            };
            setInput(i);
            setColor(Color.White);
            setTextureSize(new Vector2(texture.Width, texture.Height));
            setTextureOrigin(getTextureSize() / 2f);
            puckEnabled = initializePuck();
            currentRoad = 2;
            score = 0;
        }

        public void SetPlayerPhysics(World w)
        {
            setWorld(w);
            setBodySize( new Vector2(1.65f, 3f));
            Vector2 playerPosition = new Vector2(-29f, 1.5f);
            //Vector2 playerPosition = new Vector2(-29f, 5.7f);
            setBody(getWorld().CreateRectangle(getBodySize().X, getBodySize().Y, 1f, playerPosition));
            getBody().BodyType = BodyType.Dynamic;
            getBody().SetRestitution(0f);
            getBody().SetFriction(0f);
            getBody().OnCollision += Collision;
            getBody().SetCollisionGroup(0);
            getBody().FixedRotation = true;
        }

        public void Update(GameTime gameTime, List<Road> _road1, List<Road> _road2, List<Road> _road3)
        {
            if (getWorld().ContactCount == 0)
                isOnRoad = false;
            else
                isOnRoad = true;

            if (getWorld().ContactCount > 2)
            {
                getBody().FixedRotation = false;
                crashed = true;
            }

            if (currentRoad == (int)Player.playerPosition.OUTOFBOUNDS)
            {
                isOutOfBounds = true;
                getBody().FixedRotation = false;
                crashed = true;
            }

            Move(gameTime);
            getCurrentRoad(_road1, _road2, _road3);
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

            
            if(this.getBody().LinearVelocity.X < max_vel / 2 && this.getBody().Position.X != starting_pos)
            {
                getBody().FixedRotation = false;
                isMoving = false;
                crashed = true;
            }

        }

        private void Jump()
        {
            if(jumpCounter == 0)
            {
                float impulse = this.getBody().Mass * 12;
                this.getBody().ApplyLinearImpulse(new Vector2(0, impulse), this.getBody().WorldCenter);
            }
            else if(jumpCounter == 1)
            {
                float impulse = this.getBody().Mass * 10;
                this.getBody().ApplyLinearImpulse(new Vector2(0, impulse), this.getBody().WorldCenter);
            }
            this.isJumping = true;
            jumpTimer = 0;
            jumpCounter++;
            numOfJumps++;
        }

        //CREATE CROUCH HITBOX 
        private void Crouch()
        {
            Vector2 playerPosition = getBody().Position;
            playerPosition.Y -= 0.15f;
            Vector2 prevVel = getBody().LinearVelocity;
            getWorld().Remove(getBody());
            setBodySize(new Vector2(1.65f, 1.5f));
            setBody(getWorld().CreateRectangle(getBodySize().X, getBodySize().Y, 1f, playerPosition));
            getBody().BodyType = BodyType.Dynamic;
            getBody().SetRestitution(0.0f);
            getBody().SetFriction(0.0f);
            getBody().LinearVelocity = prevVel;
            getBody().FixedRotation = true;
            getBody().OnCollision += Collision;
            getBody().SetCollisionGroup(0);
            setTextureSize(new Vector2(getTexture().Width, getTexture().Height));
            setTextureOrigin(getTextureSize() / 2f);
            isCrouching = true;
            crouchTimer = 0;
            numOfCrouches++;
        }

        //CREATE STANDING HITBOX
        private void Uncrouch()
        {
            Vector2 playerPosition = getBody().Position;
            Vector2 prevVel = getBody().LinearVelocity;
            getWorld().Remove(getBody());
            setBodySize(new Vector2(1.65f, 3f));
            setBody(getWorld().CreateRectangle(getBodySize().X, getBodySize().Y, 1f, playerPosition));
            getBody().BodyType = BodyType.Dynamic;
            getBody().SetRestitution(0.0f);
            getBody().SetFriction(0.0f);
            getBody().LinearVelocity = prevVel;
            getBody().FixedRotation = true;
            getBody().OnCollision += Collision;
            getBody().SetCollisionGroup(0);
            setTextureSize(new Vector2(getTexture().Width, getTexture().Height));
            setTextureOrigin(getTextureSize() / 2f);
            isCrouching = false;
        }

        public void keyboardMove()
        {
            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Right) && !crashed && !isMoving)
            {
                getBody().LinearVelocity = new Vector2(max_vel, 0);
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
                    this.getBody().LinearVelocity = new Vector2(25f, 0);
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
                    this.getBody().LinearVelocity = new Vector2(25f, 0);
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
                    this.getBody().LinearVelocity = new Vector2(25f, 0);
                    isMoving = true;
                }
            }
        }
         
        private void getCurrentRoad(List<Road> _road1, List<Road> _road2, List<Road> _road3)
        {
            if (getBody().Position.Y > _road1[0].getBody().Position.Y && getBody().Position.Y < _road2[0].getBody().Position.Y)
                currentRoad = (int)playerPosition.MIDDLE;
            else if (getBody().Position.Y > _road2[0].getBody().Position.Y && getBody().Position.Y < 40f)
                currentRoad = (int)playerPosition.TOP;
            else if (getBody().Position.Y > _road3[0].getBody().Position.Y && getBody().Position.Y < _road1[0].getBody().Position.Y)
                currentRoad = (int)playerPosition.BOTTOM;
            else if (getBody().Position.Y < -40f || getBody().Position.Y > 40f)
                currentRoad = (int)playerPosition.OUTOFBOUNDS;
        }

        public bool Collision(Fixture a, Fixture b, Contact c)
        {
           //obstacle collision
            if (b.CollisionGroup == 1)
            {
                crashed = true;
                a.Body.FixedRotation = false;
            }

           return true;
        }
    }
}
