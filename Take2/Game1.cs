﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Take2.Sprites;
using Take2.Models;
using tainicom.Aether.Physics2D.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

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

        private Road RoadManager;
        private List<Road> _road1;
        private List<Road> _road2;
        private List<Road> _road3;

        private Obstacle ObstacleManagerJ;
        private List<Obstacle> _jumpObstacles1;
        private List<Obstacle> _jumpObstacles2;
        private List<Obstacle> _jumpObstacles3;
        private Obstacle ObstacleManagerC;
        private List<Obstacle> _crouchObstacles1;
        private List<Obstacle> _crouchObstacles2;
        private List<Obstacle> _crouchObstacles3;

        //TEXTURES
        private Texture2D roadTexture;
        private Texture2D playerTexture;
        private Texture2D obstacleTextureJ;
        private Texture2D obstacleTextureC;

        //PHYSICS
        private World world;
        private DebugView debugView;
        private Boolean debuggerSwitch = false;

        //CAMERA
        private Vector3 _cameraPosition;
        private float cameraViewWidth = 50.5f;

        //TEXT
        private SpriteFont font;
        private float totalTime = 0;

        //BACKGROUND
        private Scrolling scrolling1;
        private Scrolling scrolling2;

        //SOUND
        private SoundEffect jumpSound;
        private SoundEffect crouchSound;
        private SoundEffect crashSound;
        private SoundEffect obstaclePassedSound;
        private SoundEffect levelUpSound;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1024,
                PreferredBackBufferHeight = 700,
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
            //SOUND INTIALIZATION
            jumpSound = Content.Load<SoundEffect>("jumpsound");
            crouchSound = Content.Load<SoundEffect>("crouchsound");
            crashSound = Content.Load<SoundEffect>("crashsound");
            obstaclePassedSound = Content.Load<SoundEffect>("obstaclepassedring");
            levelUpSound = Content.Load<SoundEffect>("levelupring");

            //SCROLLING
            scrolling1 = new Scrolling(Content.Load<Texture2D>("space-2"), new Rectangle(0, 0, 1024, 1024));
            scrolling2 = new Scrolling(Content.Load<Texture2D>("space-1"), new Rectangle(0, 0, 1024, 1024));
            scrolling2.rectangle = new Rectangle(scrolling1.texture.Width, 0, scrolling2.rectangle.Width, scrolling2.rectangle.Height);

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if ((Keyboard.GetState().IsKeyDown(Keys.R) || _player.getPuckData2() > 500) && _player.getIsCrashed())
                resetGame();

            //GET TIME AND SCORE
            //if (!_player.crashed && _player.isMoving)
            if (!_player.getIsCrashed() && _player.getIsMoving())
            {
                totalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _player.setScore(_player.getScore() + (float)gameTime.ElapsedGameTime.TotalSeconds * 1000);

                updateCamera();

                //BACKGROUND
                if (_player.getIsMoving())
                {
                    if (scrolling1.rectangle.X + scrolling1.texture.Width <= 0)
                        scrolling1.rectangle.X = scrolling2.rectangle.X + scrolling2.texture.Width;
                    if (scrolling2.rectangle.X + scrolling2.texture.Width <= 0)
                        scrolling2.rectangle.X = scrolling1.rectangle.X + scrolling1.texture.Width;
                    scrolling1.Update();
                    scrolling2.Update();
                }

                //UPDATE OBSTACLE
                _jumpObstacles1 = ObstacleManagerJ.obstacleUpdate(_jumpObstacles1, _road1, _player, 1, true, world, gameTime, obstaclePassedSound);
                _jumpObstacles2 = ObstacleManagerJ.obstacleUpdate(_jumpObstacles2, _road2, _player, 2, true, world, gameTime, obstaclePassedSound);
                _jumpObstacles3 = ObstacleManagerJ.obstacleUpdate(_jumpObstacles3, _road3, _player, 3, true, world, gameTime, obstaclePassedSound);
                _crouchObstacles1 = ObstacleManagerC.obstacleUpdate(_crouchObstacles1, _road1, _player, 1, false, world, gameTime, obstaclePassedSound);
                _crouchObstacles2 = ObstacleManagerC.obstacleUpdate(_crouchObstacles2, _road2, _player, 2, false, world, gameTime, obstaclePassedSound);
                _crouchObstacles3 = ObstacleManagerC.obstacleUpdate(_crouchObstacles3, _road3, _player, 3, false, world, gameTime, obstaclePassedSound);
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                //display players last position
                Console.WriteLine("players last position = " + _player.getBody().Position);
                Console.WriteLine("player passed = " + _player.getIsPassed());
                Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F10))
                debuggerSwitch = !debuggerSwitch;

            //UPDATE PLAYER
            _player.Update(gameTime, _road1, _road2, _road3, jumpSound, crouchSound, levelUpSound);

            //UPDATE ROAD
            _road1 = RoadManager.MoveRoad(_road1, _player, world);
            _road2 = RoadManager.MoveRoad(_road2, _player, world);
            _road3 = RoadManager.MoveRoad(_road3, _player, world);

            //MESSAGE FOR OBSTACLE PASSED
            if (_player.getIsPassed() && _player.getPassedTime() + 1f < (float)gameTime.TotalGameTime.TotalSeconds)
                _player.setIsPassed(false);

            if (_player.getIsCrashed() && !_player.getCrashSoundPlayed())
            {
                crashSound.Play();
                _player.setCrashSoundPlayed(true);
            }

            //UPDATE WORLD
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }

        private void updateCamera()
        {
            //fixes camera onto player while adusting them to the left
            _cameraPosition = new Vector3(_player.getBody().Position.X + 20f, _player.getBody().Position.Y, 0);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //BACK GROUND
            spriteBatch.Begin();
                scrolling1.Draw(spriteBatch);
                scrolling2.Draw(spriteBatch);
            spriteBatch.End();

            //CREATE VIEW POINT FROM CAMERA
            var vp = GraphicsDevice.Viewport;
            _spriteBatchEffect.View = Matrix.CreateLookAt(_cameraPosition, _cameraPosition + Vector3.Forward, Vector3.Up);
            _spriteBatchEffect.Projection = Matrix.CreateOrthographic(cameraViewWidth, cameraViewWidth / vp.AspectRatio, 0f, -1f);

            //BEGIN DRAWING SPRITES FROM CAMERA VIEW
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, RasterizerState.CullClockwise, _spriteBatchEffect);

                //DRAW PLAYER
                spriteBatch.Draw(playerTexture, _player.getBody().Position, null, Color.White, _player.getBody().Rotation, _player.getTextureOrigin(), _player.getBodySize() / _player.getTextureSize(), SpriteEffects.FlipVertically, 0f);

                //DRAW ROAD
                RoadManager.Draw(spriteBatch, _road1, _road2, _road3);

                //DRAW OBSTACLES
                ObstacleManagerJ.Draw(spriteBatch, _jumpObstacles1, _jumpObstacles2, _jumpObstacles3);
                ObstacleManagerC.Draw(spriteBatch, _crouchObstacles1, _crouchObstacles2, _crouchObstacles3);
            
            //END
            spriteBatch.End();

            //UI
            spriteBatch.Begin();

                spriteBatch.DrawString(font, "Score: " + (int)_player.getScore(), new Vector2(50, 40), Color.White);

                if (_player.getIsOutOfBounds())
                    spriteBatch.DrawString(font, "OUT OF BOUNDS! Press r to restart", new Vector2(670 / 2, 750 / 2), Color.Red);
                else if (_player.getIsCrashed())
                    spriteBatch.DrawString(font, "CRASHED! Press r to restart", new Vector2(670 / 2, 750 / 2), Color.Red);

                if (_player.getIsPassed())
                    spriteBatch.DrawString(font, "Enemy Passed! +500", new Vector2(50, 20), Color.Yellow);

                if(_player.getLevelUp() && _player.getLevelUptimer() < (float)gameTime.TotalGameTime.TotalSeconds)
                    spriteBatch.DrawString(font, "   LEVEL UP!\nSpeed Increased", new Vector2(425, 40), Color.Yellow);



            spriteBatch.End();

            //DEBUGGER
            if (debuggerSwitch)
            {
                debugView.RenderDebugData(_spriteBatchEffect.Projection, _spriteBatchEffect.View, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, 0.8f);
                spriteBatch.Begin();

                    spriteBatch.DrawString(font, "Time: " + (int)totalTime + " seconds", new Vector2(50, 60), Color.White);
                    spriteBatch.DrawString(font, "Obstacles Passed: " + _player.getObstaclesPassed(), new Vector2(50, 80), Color.White);
                    if(_player.getCurrentRoad() == 1)
                        spriteBatch.DrawString(font, "Current Road: MIDDLE (road" + _player.getCurrentRoad() + ")", new Vector2(50, 220), Color.White);
                    else if(_player.getCurrentRoad() == 2)
                        spriteBatch.DrawString(font, "Current Road: TOP (road" + _player.getCurrentRoad() + ")", new Vector2(50, 220), Color.White);
                    else if(_player.getCurrentRoad() == 3)
                        spriteBatch.DrawString(font, "Current Road: BOTTOM (road" + _player.getCurrentRoad() + ")", new Vector2(50, 220), Color.White);
                    else if (_player.getCurrentRoad() == 4)
                        spriteBatch.DrawString(font, "Current Road: OUT OF BOUNDS", new Vector2(50, 220), Color.White);

                    if (_player.getIsOnRoad())
                        spriteBatch.DrawString(font, "ON ROAD", new Vector2(50, 240), Color.White);
                    else
                        spriteBatch.DrawString(font, "OFF ROAD", new Vector2(50, 240), Color.White);
                spriteBatch.DrawString(font, "Player Pos: " + _player.getBody().Position, new Vector2(50, 260), Color.White);
                spriteBatch.End();
            }
            base.Draw(gameTime);
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
            playerTexture = Content.Load<Texture2D>("Astronaut");
            obstacleTextureJ = Content.Load<Texture2D>("drone");
            obstacleTextureC = Content.Load<Texture2D>("alien3");
            font = Content.Load<SpriteFont>("8bitletters");

            //CREATE ROADS
            RoadManager = new Road(roadTexture);
            _road1 = new List<Road>();
            _road1 = RoadManager.CreateRoad(_road1, 1, world);

            _road2 = new List<Road>();
            _road2 = RoadManager.CreateRoad(_road2, 2, world);

            _road3 = new List<Road>();
            _road3 = RoadManager.CreateRoad(_road3, 3, world);

            //CREATE PLAYER
            _player = new Player(playerTexture);
            _player.SetPlayerPhysics(world);

            _cameraPosition = new Vector3(_player.getBody().Position.X + 20f, _player.getBody().Position.Y, 0);

            //CREATE OBSTACLES
            ObstacleManagerJ = new Obstacle(obstacleTextureJ);
            _jumpObstacles1 = new List<Obstacle>();
            _jumpObstacles2 = new List<Obstacle>();
            _jumpObstacles3 = new List<Obstacle>();
            ObstacleManagerC = new Obstacle(obstacleTextureC);
            _crouchObstacles1 = new List<Obstacle>();
            _crouchObstacles2 = new List<Obstacle>();
            _crouchObstacles3 = new List<Obstacle>();

            _jumpObstacles1 = ObstacleManagerJ.IntializeObstacles(_jumpObstacles1, _road1, true, world);
            _crouchObstacles1 = ObstacleManagerC.IntializeObstacles(_crouchObstacles1, _road1, false, world);
            _jumpObstacles2 = ObstacleManagerJ.IntializeObstacles(_jumpObstacles2, _road2, true, world);
            _crouchObstacles2 = ObstacleManagerC.IntializeObstacles(_crouchObstacles2, _road2, false, world);
            _jumpObstacles3 = ObstacleManagerJ.IntializeObstacles(_jumpObstacles3, _road3, true, world);
            _crouchObstacles3 = ObstacleManagerC.IntializeObstacles(_crouchObstacles3, _road3, false, world);

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
    }
}
