using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Take2.Sprites;
using Take2.Models;
using tainicom.Aether.Physics2D.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using System;

//TO DO:
//DYNAMIC OBSTACLES
//

namespace Take2
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private BasicEffect _spriteBatchEffect;

        //display dimensions
        private static int ScreenWidth;
        private static int ScreenHeight;

        //OBJECTS
        private Player _player;
        private List<Road> _road1;
        private List<Road> _road2;
        private List<Road> _road3;
        private List<Obstacle> _obstacles1;
        private List<Obstacle> _obstacles2;
        private List<Obstacle> _obstacles3;

        //TEXTURES
        private Texture2D roadTexture;
        private Texture2D playerTexture;
        public Texture2D obstacleTexture;

        //PHYSICS
        private World world;
        private DebugView debugView;
        private Boolean debuggerSwitch = false;

        //CAMERA
        private Vector3 _cameraPosition = new Vector3(0, 1.70f, 0);       
        private float cameraViewWidth = 50.5f;

        //TEXT
        private SpriteFont font;
        private float totalTime = 0;

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
            //CREATE GRAVITY 
            world = new World(new Vector2(0, -9.8f));

            spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteBatchEffect = new BasicEffect(graphics.GraphicsDevice);
            _spriteBatchEffect.TextureEnabled = true;

            //LOAD TEXTURES
            roadTexture = Content.Load<Texture2D>("road6");
            playerTexture = Content.Load<Texture2D>("square2");
            obstacleTexture = Content.Load<Texture2D>("rectangle");
            font = Content.Load<SpriteFont>("Time");

            //CREATE ROAD
            _road1 = new List<Road>();
            CreateRoad(_road1, 1);

            _road2 = new List<Road>();
            CreateRoad(_road2, 2);

            _road3 = new List<Road>();
            CreateRoad(_road3, 3);

            //CREATE PLAYER
            _player = new Player(playerTexture)
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

            Vector2 playerPosition = new Vector2(-99f, 1f);
            _player.bodySize = new Vector2(1f, 1f);
            _player.body = world.CreateRectangle(_player.bodySize.X, _player.bodySize.Y, 1f, playerPosition);
            _player.body.BodyType = BodyType.Dynamic;
            _player.body.SetRestitution(0.0f);
            _player.body.SetFriction(0.0f);
            _player.textureSize = new Vector2(playerTexture.Width, playerTexture.Height);
            _player.textureOrigin = _player.textureSize / 2f;
            _player.world = world;
            _player.puckEnabled = _player.initializePuck();
            
            /*
            Console.Write("working1!");
            Vertices shapePos = new Vertices();
            shapePos.Add(new Vector2(0, 0f));
            PolygonShape standingShape = new PolygonShape(shapePos, 1f);
            Fixture standingPosition = _player.body.CreateFixture(standingShape);
            Console.WriteLine("working2!");*/
            //CREATE OBSTACLES

            _obstacles1 = new List<Obstacle>();
            _obstacles2 = new List<Obstacle>();
            _obstacles3 = new List<Obstacle>();
            
            //IntializeObstacles(_obstacles1, _road1);

            //CREATE DEBUGGER
            debugView = new DebugView(world);
            debugView.AppendFlags(DebugViewFlags.DebugPanel | DebugViewFlags.PolygonPoints);
            debugView.LoadContent(GraphicsDevice, Content);

            base.Initialize();
        }

        protected override void LoadContent()
        {
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            totalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                //display players last position
                Console.WriteLine("players last position = " + _player.body.Position);
                Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F10))
                debuggerSwitch = !debuggerSwitch;

            //UPDATE ROAD
            MoveRoad();

            //UPDATE CAMERA
            updateCamera();

            //UPDATE PLAYER
            _player.Update(gameTime);

            //UPDATE OBSTACLE
            obstacleUpdate(_obstacles1, 1);
            obstacleUpdate(_obstacles2, 2);
            obstacleUpdate(_obstacles3, 3);

            //UPDATE WORLD
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        private void updateCamera()
        {
            //fixes camera onto player while adusting them to the left
            _cameraPosition = new Vector3(_player.body.Position.X + 20f, _player.body.Position.Y, 0);
        }
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //CREATE VIEW POINT FROM CAMERA
            var vp = GraphicsDevice.Viewport;
            _spriteBatchEffect.View = Matrix.CreateLookAt(_cameraPosition, _cameraPosition + Vector3.Forward, Vector3.Up);
            _spriteBatchEffect.Projection = Matrix.CreateOrthographic(cameraViewWidth, cameraViewWidth / vp.AspectRatio, 0f, -1f);

            //BEGIN DRAWING SPRITES FROM CAMERA VIEW
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, RasterizerState.CullClockwise, _spriteBatchEffect);

            //DRAW PLAYER
            spriteBatch.Draw(playerTexture, _player.body.Position, null, Color.White, _player.body.Rotation, _player.textureOrigin, _player.bodySize / _player.textureSize, SpriteEffects.None, 0f);

            //DRAW ROAD
            foreach (Road piece in _road1)
                spriteBatch.Draw(piece.texture, piece.body.Position, null, Color.White, piece.body.Rotation, piece.textureOrigin, piece.bodySize / piece.textureSize, SpriteEffects.None, 0f);

           // spriteBatch.Draw(piece.texture, piece.body.Position * (piece.bodySize / piece.textureSize), null, Color.White, piece.body.Rotation, piece.textureOrigin, piece.bodySize / piece.textureSize, SpriteEffects.None, 0f);

            foreach (Road piece in _road2)
                //* (piece.bodySize / piece.textureSize)
                spriteBatch.Draw(piece.texture, piece.body.Position , null, Color.White, piece.body.Rotation, piece.textureOrigin, piece.bodySize / piece.textureSize, SpriteEffects.None, 0f);

            foreach (Road piece in _road3)
                //* (piece.bodySize / piece.textureSize)
                spriteBatch.Draw(piece.texture, piece.body.Position, null, Color.White, piece.body.Rotation, piece.textureOrigin, piece.bodySize / piece.textureSize, SpriteEffects.None, 0f);

            //DRAW OBSTACLES
            foreach (Obstacle obs in _obstacles1)
                spriteBatch.Draw(obstacleTexture, obs.body.Position, null, Color.White, obs.body.Rotation, obs.textureOrigin, obs.bodySize / obs.textureSize, SpriteEffects.None, 0f);

            foreach (Obstacle obs in _obstacles2)
                spriteBatch.Draw(obstacleTexture, obs.body.Position, null, Color.White, obs.body.Rotation, obs.textureOrigin, obs.bodySize / obs.textureSize, SpriteEffects.None, 0f);

            foreach (Obstacle obs in _obstacles3)
                spriteBatch.Draw(obstacleTexture, obs.body.Position, null, Color.White, obs.body.Rotation, obs.textureOrigin, obs.bodySize / obs.textureSize, SpriteEffects.None, 0f);

            //END
            spriteBatch.End();

            spriteBatch.Begin();

            spriteBatch.DrawString(font, "Time: " + (int)totalTime + " seconds", new Vector2(100, 50), Color.Black);

            spriteBatch.End();


            //DRAW DEBUGGER
            if (debuggerSwitch)
                debugView.RenderDebugData(_spriteBatchEffect.Projection, _spriteBatchEffect.View, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, 0.8f);

            base.Draw(gameTime);
        }

        //INTIALIZE OBSTACLES
        /*
        public void IntializeObstacles(List<Obstacle> obs, List<Road> road)
        {
            //iterate road pieces
            for(int i = 0; i < road.Count; i++)
            {
                //create 5 objects on each road piece every 100 meters
                for (int j = 0; j < 5; j++)
                {
                    //variables holding positions
                    Vector2 pos;
                    float y_pos_offset = 3f;
                    float x_pos_offset;

                    switch (j)
                    {
                        //NOTE: i == 0 cases are to account for intial starting position

                        case 0:
                            x_pos_offset = -100f;
                            
                            if (i == 0)
                                pos = new Vector2(x_pos_offset, road[i].body.Position.Y + y_pos_offset);
                            else
                                pos = new Vector2(road[i].body.Position.X, road[i].body.Position.Y + y_pos_offset);

                            AddObstacle(obs, pos);
                            break;
                        case 1:
                            x_pos_offset = 100f;
                            if (i == 0)
                                pos = new Vector2(100f, road[i].body.Position.Y + 3f);
                            else
                                //original (road[i].body.Position.X + (road[i].bodySize.X * 0.25f))
                                pos = new Vector2((road[i].body.Position.X + x_pos_offset), road[i].body.Position.Y + y_pos_offset);

                            AddObstacle(obs, pos);
                            break;

                        case 2:
                            x_pos_offset = 200f;
                            if (i == 0)
                                pos = new Vector2(x_pos_offset, road[i].body.Position.Y + 3f);
                            else
                                pos = new Vector2((road[i].body.Position.X + x_pos_offset), road[i].body.Position.Y + y_pos_offset);

                            AddObstacle(obs, pos);
                            break;

                        case 3:
                            x_pos_offset = 300f;
                            if (i == 0)
                                pos = new Vector2(x_pos_offset, road[i].body.Position.Y + 3f);
                            else
                                pos = new Vector2((road[i].body.Position.X + x_pos_offset), road[i].body.Position.Y + y_pos_offset);

                            AddObstacle(obs, pos);
                            break;
                        case 4:
                            x_pos_offset = 400f;
                            if (i == 0)
                                pos = new Vector2(x_pos_offset, road[i].body.Position.Y + 3f);
                            else
                                pos = new Vector2((road[i].body.Position.X + x_pos_offset), road[i].body.Position.Y + y_pos_offset);

                            AddObstacle(obs, pos);
                            break;
                    }
                }
            }
        }*/

        protected void AddObstacle(List<Obstacle> o, Vector2 pos)
        {
            Obstacle obs = new Obstacle(obstacleTexture)
            {
                color = Color.Red
            };
            obs.bodySize = new Vector2(1f, 4f);
            obs.body = world.CreateRectangle(obs.bodySize.X, obs.bodySize.Y, 2f, pos);
            obs.body.BodyType = BodyType.Static;
            obs.body.SetRestitution(0.0f);
            obs.body.SetFriction(0.0f);
            obs.textureSize = new Vector2(obs.texture.Width, obs.texture.Height);
            obs.textureOrigin = obs.textureSize / 2f;
            obs.world = world;
            o.Add(obs);
        }

        public void obstacleUpdate(List<Obstacle> obs, int roadNum)
        {
            if(roadNum == 1)
            {
                if (obs.Count == 0)
                {
                    AddObstacle(obs, new Vector2(_player.body.Position.X + 50f, _road1[0].body.Position.Y + 2.5f));
                    AddObstacle(obs, new Vector2(_player.body.Position.X + 175f, _road1[0].body.Position.Y + 3.5f));

                }
                for (int i = 0; i < obs.Count; i++)
                {
                    if (_player.body.Position.X - 100 > obs[i].body.Position.X)
                    {
                        world.Remove(obs[i].body);
                        obs.RemoveAt(0);
                    }
                }
            }
            if (roadNum == 2)
            {
                if (obs.Count == 0)
                {
                    AddObstacle(obs, new Vector2(_player.body.Position.X + 75f, _road2[0].body.Position.Y + 2.5f));
                    AddObstacle(obs, new Vector2(_player.body.Position.X + 160f, _road2[0].body.Position.Y + 3.5f));

                }
                for (int i = 0; i < obs.Count; i++)
                {
                    if (_player.body.Position.X - 100 > obs[i].body.Position.X)
                    {
                        world.Remove(obs[i].body);
                        obs.RemoveAt(0);
                    }
                }
            }

            if (roadNum == 3)
            {
                if (obs.Count == 0)
                {
                    AddObstacle(obs, new Vector2(_player.body.Position.X + 30f, _road3[0].body.Position.Y + 2.5f));
                    AddObstacle(obs, new Vector2(_player.body.Position.X + 140, _road3[0].body.Position.Y + 3.5f));

                }
                for (int i = 0; i < obs.Count; i++)
                {
                    if (_player.body.Position.X - 100 > obs[i].body.Position.X)
                    {
                        world.Remove(obs[i].body);
                        obs.RemoveAt(0);
                    }
                }
            }

        }

        public void CreateRoad(List<Road> road, int roadNum)
        {
            if(roadNum == 1)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (i % 2 == 0)
                    {
                        Vector2 pos = new Vector2(roadTexture.Width * i, 0f);
                        AddRoad(road, pos);
                    }
                    else
                    {
                        Vector2 pos = new Vector2(roadTexture.Width * i + 60f, 0f);
                        AddRoad(road, pos);
                    }
                }
            }

            else if(roadNum == 2)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (i % 2 == 0)
                    {
                        Vector2 pos = new Vector2(roadTexture.Width * i + 30f, 10f);
                        AddRoad(road, pos);
                    }
                    else
                    {
                        Vector2 pos = new Vector2(roadTexture.Width * i, 10f);
                        AddRoad(road, pos);
                    }
                }
            }

            else if (roadNum == 3)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (i % 2 == 0)
                    {
                        Vector2 pos = new Vector2(roadTexture.Width * i -30f, -10f);
                        AddRoad(road, pos);
                    }
                    else
                    {
                        Vector2 pos = new Vector2(roadTexture.Width * i, -10f);
                        AddRoad(road, pos);
                    }
                }
            }
        }

        public void AddRoad(List<Road> pieces, Vector2 pos)
        {
            Road r = new Road(roadTexture);
            // Create the ground fixture
            r.bodySize = new Vector2(200f,1f);

            r.body = world.CreateRectangle(r.bodySize.X , r.bodySize.Y, 1f, pos);
            Console.WriteLine("Road Piece created! pos = " + pos);
 
            //r.body = world.CreateRectangle(r.texture.Width, 2f, 2f, pos);
            r.body.BodyType = BodyType.Static;
            r.body.SetRestitution(0.0f);
            r.body.SetFriction(0.0f);
            r.textureSize = new Vector2(roadTexture.Width, roadTexture.Height);
            r.textureOrigin = r.textureSize / 2f;
            r.world = world;

            pieces.Add(r);
        }

        private void MoveRoad()
        {
            if (_player.body.Position.X > _road1[_road1.Count - 1].body.Position.X)
            {
                Console.WriteLine("new road1 piece created!");
                world.Remove(_road1[0].body);
                _road1.RemoveAt(0);
                world.Remove(_road1[0].body);
                _road1.RemoveAt(0);
                Vector2 new_pos1 = new Vector2(_road1[_road1.Count - 1].body.Position.X + roadTexture.Width, _road1[0].body.Position.Y);
                AddRoad(_road1, new_pos1);
                Vector2 new_pos2 = new Vector2(_road1[_road1.Count - 1].body.Position.X + roadTexture.Width + 20f, _road1[0].body.Position.Y);
                AddRoad(_road1, new_pos2);
            }

            if (_player.body.Position.X > _road2[_road2.Count - 1].body.Position.X)
            {
                Console.WriteLine("new road2 piece created!");
                world.Remove(_road2[0].body);
                _road2.RemoveAt(0);
                world.Remove(_road2[0].body);
                _road2.RemoveAt(0);
                Vector2 new_pos1 = new Vector2(_road2[_road2.Count - 1].body.Position.X + roadTexture.Width, _road2[0].body.Position.Y);
                AddRoad(_road2, new_pos1);
                Vector2 new_pos2 = new Vector2(_road2[_road2.Count - 1].body.Position.X + roadTexture.Width + 20f, _road2[0].body.Position.Y);
                AddRoad(_road2, new_pos2);
            }

            if (_player.body.Position.X > _road3[_road3.Count - 1].body.Position.X)
            {
                Console.WriteLine("new road3 piece created!");
                world.Remove(_road3[0].body);
                _road3.RemoveAt(0);
                world.Remove(_road3[0].body);
                _road3.RemoveAt(0);
                Vector2 new_pos1 = new Vector2(_road3[_road3.Count - 1].body.Position.X + roadTexture.Width, _road3[0].body.Position.Y);
                AddRoad(_road3, new_pos1);
                Vector2 new_pos2 = new Vector2(_road3[_road3.Count - 1].body.Position.X + roadTexture.Width + 20f, _road3[0].body.Position.Y);
                AddRoad(_road3, new_pos2);
            }
        }

    }
}
