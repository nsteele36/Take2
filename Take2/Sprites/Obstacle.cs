using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;

namespace Take2.Sprites
{
    public class Obstacle : Sprite
    {
        public bool isVisible;

        public Obstacle(Texture2D texture) : base(texture) { }

        protected void AddObstacle(List<Obstacle> o, Vector2 pos, World world)
        {
            Obstacle obs = new Obstacle(texture)
            {
                color = Color.Red
            };
            obs.world = world;
            obs.bodySize = new Vector2(4f, 4f);
            obs.body = world.CreateRectangle(obs.bodySize.X, obs.bodySize.Y, 2f, pos);
            obs.body.BodyType = BodyType.Static;
            obs.body.SetRestitution(0.0f);
            obs.body.SetFriction(0.0f);
            obs.textureSize = new Vector2(obs.texture.Width, obs.texture.Height);
            obs.textureOrigin = obs.textureSize / 2f;
            obs.body.SetCollisionGroup(1);
            o.Add(obs);
        }

        public List<Obstacle> createObstacles(List<Obstacle> obs, List<Road> road, Player _player, int roadNum, bool isJumpingObs, World world)
        {
            if (obs.Count == 0  && _player.currentRoad == roadNum)
            {   
                if (isJumpingObs)
                    AddObstacle(obs, new Vector2(road[road.Count - 2].body.Position.X, road[0].body.Position.Y + 2.5f), world);
                else
                    AddObstacle(obs, new Vector2(road[road.Count - 1].body.Position.X, road[0].body.Position.Y + 4.5f), world);
            }
            return obs;
        }

        public List<Obstacle> deleteObstacles(List<Obstacle> obs, Player _player, int roadNum, World world, GameTime gameTime)
        {
            if (obs.Count != 0)
            {
                for (int i = 0; i < obs.Count; i++)
                {
                    if (_player.body.Position.X - 10 > obs[i].body.Position.X)
                    {
                        world.Remove(obs[i].body);
                        obs.RemoveAt(i);

                        if (_player.currentRoad == roadNum && !_player.crashed)
                        {
                            _player.obstaclesPassed++;
                            _player.score += 500f;
                            _player.passed = true;
                            _player.passedTime = (float)gameTime.TotalGameTime.TotalSeconds;
                        }
                    }
                }
            }
            return obs;
        }
        public List<Obstacle> obstacleUpdate(List<Obstacle> obs, List<Road> road, Player _player, int roadNum, bool isJumpingObs, World world, GameTime gameTime)
        {
            if (obs.Count == 0)
                obs = createObstacles(obs, road, _player, roadNum, isJumpingObs, world);
            else
                deleteObstacles(obs, _player, roadNum, world, gameTime);

            return obs;
        }

        public void Draw(SpriteBatch sb, List<Obstacle> obs1, List<Obstacle> obs2, List<Obstacle> obs3)
        {
            foreach (Obstacle obs in obs1)
                sb.Draw(obs.texture, obs.body.Position, null, Color.White, obs.body.Rotation, obs.textureOrigin, obs.bodySize / obs.textureSize, SpriteEffects.FlipVertically, 0f);

            foreach (Obstacle obs in obs2)
                sb.Draw(obs.texture, obs.body.Position, null, Color.White, obs.body.Rotation, obs.textureOrigin, obs.bodySize / obs.textureSize, SpriteEffects.FlipVertically, 0f);

            foreach (Obstacle obs in obs3)
                sb.Draw(obs.texture, obs.body.Position, null, Color.White, obs.body.Rotation, obs.textureOrigin, obs.bodySize / obs.textureSize, SpriteEffects.FlipVertically, 0f);
        }

        public List<Obstacle> IntializeObstacles(List<Obstacle> obs, List<Road> road, bool isJumpingObs, World world)
        {
            for (int i = 0; i < road.Count; i++)
            {
                if(i % 2 == 0)
                {
                    //if ((i == 4 || i == 8) && isJumpingObs)
                    //    AddObstacle(obs, new Vector2(road[i].body.Position.X / 2, road[0].body.Position.Y + 2.5f), world);
                    if ((i == 2 || i == 6) && !isJumpingObs)
                        AddObstacle(obs, new Vector2(road[i].body.Position.X / 2 + 300f, road[0].body.Position.Y + 4.5f), world);
                }
                else
                {
                    //if ((i == 3 || i == 7) && !isJumpingObs)
                    //    AddObstacle(obs, new Vector2(road[i].body.Position.X / 2, road[0].body.Position.Y + 4.5f), world);
                    if((i == 5 || i == 9) && isJumpingObs)
                        AddObstacle(obs, new Vector2(road[i].body.Position.X / 2 + 100f , road[0].body.Position.Y + 2.5f), world);
                }
            }
            return obs;
        }
    }
}

