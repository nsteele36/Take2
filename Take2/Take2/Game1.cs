using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Take2.Sprites;
using Take2.Models;
using tainicom.Aether.Physics2D.Dynamics;
using System;

namespace Take2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private BasicEffect _spriteBatchEffect;
        public static int  ScreenWidth;
        public static int ScreenHeight;
        public Player player;
        public List<Road> _road;
        private List<Obstacle> obstacles = new List<Obstacle>();
        private Texture2D roadTexture;
        private Texture2D playerTexture;
        public Texture2D obstacleTexture;
        //private Texture2D backgroundTexture;
        //Vector2 backgroundPosition;
        public Vector2 SpritePosition;
        public Rectangle SpriteRectangle;

        //PHYSICS
        public World world;

        public Vector2 obsBodySize = new Vector2(1.5f, 2f);
        public Vector2 roadBodySize = new Vector2(1.5f, 2f);
        //public Camera camera;

        private Vector3 _cameraPosition = new Vector3(0, 1.70f, 0);
        //private float cameraViewWidth = 12.5f;
        private float cameraViewWidth = 50.5f;

        float spawnTimeValue;
        TimeSpan previousSpanTime;
        TimeSpan spawnTime;
        float bgSpeed;

        public KeyboardState oldKeyState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1920,
                PreferredBackBufferHeight = 1080
            };
            ScreenWidth = graphics.PreferredBackBufferWidth;
            ScreenHeight = graphics.PreferredBackBufferHeight;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //camera = new Camera(GraphicsDevice.Viewport);
            //camera = new Camera();
            world = new World(new Vector2(0, -9.8f));

            obstacles = new List<Obstacle>();
           
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteBatchEffect = new BasicEffect(graphics.GraphicsDevice);
            _spriteBatchEffect.TextureEnabled = true;

            roadTexture = Content.Load<Texture2D>("road");
            playerTexture = Content.Load<Texture2D>("square");
            obstacleTexture = Content.Load<Texture2D>("square");

            player = new Player(playerTexture)
            {
                Input = new Input()
                {
                    Left = Keys.Left,
                    Right = Keys.Right,
                    Up = Keys.Up,
                    Down = Keys.Down,
                    Jump = Keys.Space,
                },
                color = Color.White,
                texture = playerTexture,
                //pos = new Vector2(0, 1f),

            };

            Vector2 playerPosition = new Vector2(0, 1f);
            player.bodySize = new Vector2(2f, 2f);
            player.body = world.CreateRectangle(player.bodySize.X, player.bodySize.Y, 1f, playerPosition);
            player.body.BodyType = BodyType.Dynamic;
            player.body.SetRestitution(0.3f);
            player.body.SetFriction(0.5f);
            //player.pos = player.body.Position;
            player.textureSize = new Vector2(playerTexture.Width, playerTexture.Height);
            player.textureOrigin = player.textureSize / 2f;

            /* road */
            _road = new List<Road>();
            CreateRoad(_road);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            float totalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            /*
            foreach(Obstacle obs in obstacles)
            {
                player.Update(gameTime, obs);
            }*/
            //LoadObstacles();
            //_road.Update(gameTime, player);

            //obstacleUpdate(gameTime);
            MoveRoad();
            //MovePlayer();
            HandleKeyboard(gameTime, totalSeconds);
            //camera.Update(gameTime, this);
            //camera.Follow(player);
            player.Update(gameTime, player);
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            /*
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //DrawObstacles();
            _road.Draw(spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                null, null, null, null,
                camera.transform);
            spriteBatch.Draw(playerTexture, player.pos, null, Color.White, 0.0f,
                 new Vector2(0, playerTexture.Height / 2), Vector2.One, SpriteEffects.None, .0f);
            spriteBatch.End();

            spriteBatch.Begin();
            foreach (Obstacle obs in obstacles)
                obs.Draw(spriteBatch);
          
            spriteBatch.End();

            base.Draw(gameTime);*/
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var vp = GraphicsDevice.Viewport;
            _spriteBatchEffect.View = Matrix.CreateLookAt(_cameraPosition, _cameraPosition + Vector3.Forward, Vector3.Up);
            _spriteBatchEffect.Projection = Matrix.CreateOrthographic(cameraViewWidth, cameraViewWidth / vp.AspectRatio, 0f, -1f);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterizerState.CullClockwise, _spriteBatchEffect);
            //spriteBatch.Begin();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);
            // player.Draw(spriteBatch);
            //spriteBatch.Draw(roadTexture, _road.body.Position, Color.White);
            //spriteBatch.Draw(playerTexture, player.body.Position, Color.White) ;
            spriteBatch.Draw(playerTexture, player.body.Position, null, Color.White, player.body.Rotation, player.textureOrigin, player.bodySize / player.textureSize, SpriteEffects.FlipVertically, 0f);
            foreach(Road piece in _road)
                spriteBatch.Draw(roadTexture, piece.body.Position, null, Color.White, piece.body.Rotation, piece.textureOrigin, piece.bodySize / piece.textureSize, SpriteEffects.FlipVertically, 0f);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void MoveRoad()
        {
            for (int i = 0; i < _road.Count; i++)
                _road[i].body.LinearVelocity += -player.body.LinearVelocity;

            if (_road[1].body.Position.X < 0)
            {
                Vector2 new_pos = new Vector2(_road[9].body.Position.X + roadTexture.Width, _road[0].body.Position.Y);
                _road.RemoveAt(0);
                AddRoad(_road, 9);
            }
        }

        public void obstacleUpdate(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - previousSpanTime >= spawnTime)
            {
                previousSpanTime = gameTime.TotalGameTime;
                AddObstacle(obstacles, true);

                spawnTimeValue *= 0.98f;
                spawnTime = TimeSpan.FromSeconds(spawnTimeValue);
            }

            for (int i = 0; i < obstacles.Count; i++)
            {
                obstacles[i].Update(gameTime, obstacles[i]);
                
                if (!obstacles[i].isVisible)
                    obstacles.RemoveAt(i);
            }
        }

        protected void AddObstacle(List<Obstacle> things, bool isObstacle)
        {
            Obstacle obs = new Obstacle(obstacleTexture);

            obstacleTexture = Content.Load<Texture2D>("square");
            Vector2 obsPos = new Vector2(1920, 1000 / 2);

            obs.Initialize(obstacleTexture, obsPos, bgSpeed, isObstacle);
            things.Add(obs);
        }

        public void CreateRoad(List<Road> road)
        {
            for (int i = 0; i < 10; i++)
                AddRoad(road, i);            
        }

        public void AddRoad(List<Road> pieces, int i)
        {
            Road r = new Road(roadTexture);
            // Create the ground fixture
            r.bodySize = new Vector2(8f, 1f);
            Vector2 roadPosition = new Vector2(0, -r.bodySize.Y / 2f); 
            r.body = world.CreateRectangle(r.bodySize.X, r.bodySize.Y, 1f, roadPosition);
            r.body.BodyType = BodyType.Static;
            r.body.SetRestitution(0.3f);
            r.body.SetFriction(0.5f);
            r.textureSize = new Vector2(roadTexture.Width, roadTexture.Height);
            r.textureOrigin = r.textureSize / 2f;
            r.body.Position = new Vector2(roadTexture.Width * i, 1080 / 2);
            pieces.Add(r);


        }

        /*
        public void checkCollision(GameTime gametime, Sprite s)
        {
                        
            if (this.vel.X > 0 && this.IsTouchingLeft(s) || this.vel.X < 0 && this.IsTouchingRight(s))
                this.vel.X = 0;

            if (this.vel.Y > 0 && this.IsTouchingTop(s) || this.vel.Y < 0 && this.IsTouchingBottom(s))
                this.vel.Y = 0;
               
        }*/

        protected void HandleKeyboard(GameTime gameTime, float totalSeconds)
        {
            KeyboardState state = Keyboard.GetState();
            // Move camera
            if (state.IsKeyDown(Keys.Left))
            {
                player.body.ApplyForce(new Vector2(-500, 0), player.body.WorldCenter);
                _cameraPosition.X -= totalSeconds * cameraViewWidth;

            }


            if (state.IsKeyDown(Keys.Right))
            {
                player.body.ApplyForce(new Vector2(500, 0), player.body.WorldCenter);
                _cameraPosition.X += totalSeconds * cameraViewWidth;
            }


            if (state.IsKeyDown(Keys.Up))
            {
                player.body.ApplyForce(new Vector2(0, 500), player.body.WorldCenter);
                _cameraPosition.Y += totalSeconds * cameraViewWidth;
            }


            if (state.IsKeyDown(Keys.Down))
            {
                player.body.ApplyForce(new Vector2(0, -500), player.body.WorldCenter);
                _cameraPosition.Y -= totalSeconds * cameraViewWidth;
            }


            if (state.IsKeyDown(Keys.Space) && oldKeyState.IsKeyUp(Keys.Space))
            {
                float impulse = player.body.Mass * 10;
                player.body.ApplyLinearImpulse(new Vector2(0, impulse), player.body.WorldCenter);
            }


            if (state.IsKeyDown(Keys.Escape))
                Exit();


            oldKeyState = state;
        }




    }
}
