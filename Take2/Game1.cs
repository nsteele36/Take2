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
        private float playerBodySizeX = 60f;

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
            createGame();
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

            if ((Keyboard.GetState().IsKeyDown(Keys.R) || _player.puckData2 > 500) && _player.crashed)
            {
                resetGame();
            }

            if (!_player.crashed)
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

            GraphicsDevice.Clear(Color.Black);

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
                spriteBatch.Draw(piece.texture, piece.body.Position, null, Color.White, piece.body.Rotation, piece.textureOrigin, piece.bodySize / piece.textureSize, SpriteEffects.FlipVertically, 0f);

            foreach (Road piece in _road2)
                spriteBatch.Draw(piece.texture, piece.body.Position, null, Color.White, piece.body.Rotation, piece.textureOrigin, piece.bodySize / piece.textureSize, SpriteEffects.FlipVertically, 0f);

            foreach (Road piece in _road3)
                spriteBatch.Draw(piece.texture, piece.body.Position, null, Color.White, piece.body.Rotation, piece.textureOrigin, piece.bodySize / piece.textureSize, SpriteEffects.FlipVertically, 0f);

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

            spriteBatch.DrawString(font, "Time: " + (int)totalTime + " seconds", new Vector2(100, 20), Color.White);
            spriteBatch.DrawString(font, "Obstacles Passed: " + _player.obstaclesPassed, new Vector2(100, 40), Color.White);

            if (_player.crashed)
                spriteBatch.DrawString(font, "CRASHED! Press r to restart", new Vector2(100, 60), Color.White);

            spriteBatch.End();

            //DRAW DEBUGGER
            if (debuggerSwitch)
            {
                debugView.RenderDebugData(_spriteBatchEffect.Projection, _spriteBatchEffect.View, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, 0.8f);
                spriteBatch.Begin();
                if(_player.currentRoad == 1)
                    spriteBatch.DrawString(font, "Current Road: MIDDLE (road" + _player.currentRoad + ")", new Vector2(50, 80), Color.White);
                else if(_player.currentRoad == 2)
                    spriteBatch.DrawString(font, "Current Road: TOP (road" + _player.currentRoad + ")", new Vector2(50, 80), Color.White);
                else if(_player.currentRoad == 3)
                    spriteBatch.DrawString(font, "Current Road: BOTTOM (road" + _player.currentRoad + ")", new Vector2(50, 80), Color.White);
                spriteBatch.End();
            }


            base.Draw(gameTime);
        }

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
            //MIDDLE ROAD
            if(roadNum == 1)
            {
                if (obs.Count == 0)
                {
                    AddObstacle(obs, new Vector2(_player.body.Position.X + 60f, _road1[0].body.Position.Y + 2.5f));
                    AddObstacle(obs, new Vector2(_player.body.Position.X + 120f, _road1[0].body.Position.Y + 3.5f));

                }

                for (int i = 0; i < obs.Count; i++)
                {
                    if (_player.body.Position.X - 10 > obs[i].body.Position.X)
                    {
                        world.Remove(obs[i].body);
                        obs.RemoveAt(0);

                        if(_player.currentRoad == roadNum && !_player.crashed)
                            _player.obstaclesPassed++;
                    }

                    if ( _player.body.Position.Y > _road1[0].body.Position.Y && _player.body.Position.Y < _road2[0].body.Position.Y)
                        _player.currentRoad = roadNum;
                }
            }
            //TOP ROAD
            if (roadNum == 2)
            {
                if (obs.Count == 0)
                {
                    AddObstacle(obs, new Vector2(_player.body.Position.X + 70f, _road2[0].body.Position.Y + 2.5f));
                    AddObstacle(obs, new Vector2(_player.body.Position.X + 130f, _road2[0].body.Position.Y + 3.5f));
                }
                for (int i = 0; i < obs.Count; i++)
                {
                    if (_player.body.Position.X - 10 > obs[i].body.Position.X)
                    {
                        world.Remove(obs[i].body);
                        obs.RemoveAt(0);

                        if (_player.currentRoad == roadNum && !_player.crashed)
                            _player.obstaclesPassed++;
                    }

                    if (_player.body.Position.Y > _road2[0].body.Position.Y)
                        _player.currentRoad = roadNum;
                    
                }
            }
            //BOTTOM ROAD
            if (roadNum == 3)
            {
                if (obs.Count == 0)
                {
                    AddObstacle(obs, new Vector2(_player.body.Position.X + 40f, _road3[0].body.Position.Y + 2.5f));
                    AddObstacle(obs, new Vector2(_player.body.Position.X + 80f, _road3[0].body.Position.Y + 3.5f));
                }
                for (int i = 0; i < obs.Count; i++)
                {
                    if (_player.body.Position.X - 10 > obs[i].body.Position.X)
                    {
                        world.Remove(obs[i].body);
                        obs.RemoveAt(0);
                        if (_player.currentRoad == roadNum && !_player.crashed)
                            _player.obstaclesPassed++;
                    }

                    if (_player.body.Position.Y > _road3[0].body.Position.Y && _player.body.Position.Y < _road1[0].body.Position.Y)
                        _player.currentRoad = roadNum;
                    
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
                        Vector2 pos = new Vector2(playerBodySizeX * i, 0f);
                        AddRoad(road, pos);
                    }
                    else
                    {
                        Vector2 pos = new Vector2(playerBodySizeX * i + playerBodySizeX / 2, 0f);
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
                        Vector2 pos = new Vector2(playerBodySizeX * i + playerBodySizeX / 2, 10f);
                        AddRoad(road, pos);
                    }
                    else
                    {
                        Vector2 pos = new Vector2(playerBodySizeX * i, 10f);
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
                        Vector2 pos = new Vector2(60f * i - playerBodySizeX / 2, -10f);
                        AddRoad(road, pos);
                    }
                    else
                    {
                        Vector2 pos = new Vector2(60f * i, -10f);
                        AddRoad(road, pos);
                    }
                }
            }
        }

        public void AddRoad(List<Road> pieces, Vector2 pos)
        {
            Road r = new Road(roadTexture);
            r.bodySize = new Vector2(playerBodySizeX, 1f);
            r.body = world.CreateRectangle(r.bodySize.X , r.bodySize.Y, 1f, pos);
            Console.WriteLine("Road Piece created! pos = " + pos);
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
            if (_player.body.Position.X > _road1[_road1.Count - 2].body.Position.X)
            {
                Console.WriteLine("new road1 piece created!");
                world.Remove(_road1[0].body);
                _road1.RemoveAt(0);
                world.Remove(_road1[0].body);
                _road1.RemoveAt(0);
                Vector2 new_pos1 = new Vector2(_road1[_road1.Count - 1].body.Position.X + playerBodySizeX, _road1[0].body.Position.Y);
                AddRoad(_road1, new_pos1);
                Vector2 new_pos2 = new Vector2(_road1[_road1.Count - 1].body.Position.X + playerBodySizeX + playerBodySizeX / 2, _road1[0].body.Position.Y);
                AddRoad(_road1, new_pos2);
            }

            if (_player.body.Position.X > _road2[_road2.Count - 2].body.Position.X)
            {
                Console.WriteLine("new road2 piece created!");
                world.Remove(_road2[0].body);
                _road2.RemoveAt(0);
                world.Remove(_road2[0].body);
                _road2.RemoveAt(0);
                Vector2 new_pos1 = new Vector2(_road2[_road2.Count - 1].body.Position.X + playerBodySizeX, _road2[0].body.Position.Y);
                AddRoad(_road2, new_pos1);
                Vector2 new_pos2 = new Vector2(_road2[_road2.Count - 1].body.Position.X + playerBodySizeX + playerBodySizeX / 2, _road2[0].body.Position.Y);
                AddRoad(_road2, new_pos2);
            }

            if (_player.body.Position.X > _road3[_road3.Count - 2].body.Position.X)
            {
                Console.WriteLine("new road3 piece created!");
                world.Remove(_road3[0].body);
                _road3.RemoveAt(0);
                world.Remove(_road3[0].body);
                _road3.RemoveAt(0);
                Vector2 new_pos1 = new Vector2(_road3[_road3.Count - 1].body.Position.X + playerBodySizeX, _road3[0].body.Position.Y);
                AddRoad(_road3, new_pos1);
                Vector2 new_pos2 = new Vector2(_road3[_road3.Count - 1].body.Position.X + playerBodySizeX + playerBodySizeX / 2, _road3[0].body.Position.Y);
                AddRoad(_road3, new_pos2);
            }
        }

        public void createGame()
        {
            //CREATE GRAVITY 
            world = new World(new Vector2(0, -9.8f));

            spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteBatchEffect = new BasicEffect(graphics.GraphicsDevice);
            _spriteBatchEffect.TextureEnabled = true;

            //LOAD TEXTURES
            roadTexture = Content.Load<Texture2D>("spaceplatform");
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
            };

            Vector2 playerPosition = new Vector2(-29f, 1.5f);
            _player.bodySize = new Vector2(1f, 1f);
            _player.body = world.CreateRectangle(_player.bodySize.X, _player.bodySize.Y, 1f, playerPosition);
            _player.body.BodyType = BodyType.Dynamic;
            _player.body.SetRestitution(0.0f);
            _player.body.SetFriction(0.0f);
            _player.textureSize = new Vector2(playerTexture.Width, playerTexture.Height);
            _player.textureOrigin = _player.textureSize / 2f;
            _player.world = world;
            _player.puckEnabled = _player.initializePuck();
            _player.currentRoad = 2;

            //CREATE OBSTACLES
            _obstacles1 = new List<Obstacle>();
            _obstacles2 = new List<Obstacle>();
            _obstacles3 = new List<Obstacle>();

            //IntializeObstacles(_obstacles1, _road1);

            //CREATE DEBUGGER
            debugView = new DebugView(world);
            debugView.AppendFlags(DebugViewFlags.DebugPanel | DebugViewFlags.PolygonPoints);
            debugView.LoadContent(GraphicsDevice, Content);
        }
        public void resetGame()
        {
            createGame();
            totalTime = 0;
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

    }
}
