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
            Obstacle obs = new Obstacle(getTexture());
            obs.setWorld(world);
            obs.setBodySize(new Vector2(4f, 4f));
            obs.setBody( world.CreateRectangle(obs.getBodySize().X, obs.getBodySize().Y, 2f, pos));
            obs.getBody().BodyType = BodyType.Static;
            obs.getBody().SetRestitution(0.0f);
            obs.getBody().SetFriction(0.0f);
            obs.setTextureSize(new Vector2(obs.getTexture().Width, obs.getTexture().Height));
            obs.setTextureOrigin(obs.getTextureSize() / 2f);
            obs.getBody().SetCollisionGroup(1);
            o.Add(obs);
        }

        public List<Obstacle> createObstacles(List<Obstacle> obs, List<Road> road, Player _player, int roadNum, bool isJumpingObs, World world)
        {
            if (obs.Count == 0  && _player.getCurrentRoad() == roadNum)
            {   
                if (isJumpingObs)
                    AddObstacle(obs, new Vector2(road[road.Count - 2].getBody().Position.X, road[0].getBody().Position.Y + 2.5f), world);
                else
                    AddObstacle(obs, new Vector2(road[road.Count - 1].getBody().Position.X, road[0].getBody().Position.Y + 4.5f), world);
            }
            return obs;
        }

        public List<Obstacle> deleteObstacles(List<Obstacle> obs, Player _player, int roadNum, World world, GameTime gameTime)
        {
            if (obs.Count != 0)
            {
                for (int i = 0; i < obs.Count; i++)
                {
                    if (_player.getBody().Position.X - 10 > obs[i].getBody().Position.X)
                    {
                        world.Remove(obs[i].getBody());
                        obs.RemoveAt(i);

                        if (_player.getCurrentRoad() == roadNum && !_player.getIsCrashed())
                        {
                            //_player.obstaclesPassed++;
                            //_player.score += 500f;
                            //_player.passed = true;
                            //_player.passedTime = (float)gameTime.TotalGameTime.TotalSeconds;
                            _player.setObstaclesPassed(_player.getObstaclesPassed() + 1);
                            _player.setScore(_player.getScore() + 500f);
                            _player.setIsPassed(true);
                            _player.setPassedTime((float)gameTime.TotalGameTime.TotalSeconds);
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
                sb.Draw(obs.getTexture(), obs.getBody().Position, null, Color.White, obs.getBody().Rotation, obs.getTextureOrigin(), obs.getBodySize() / obs.getTextureSize(), SpriteEffects.FlipVertically, 0f);

            foreach (Obstacle obs in obs2)
                sb.Draw(obs.getTexture(), obs.getBody().Position, null, Color.White, obs.getBody().Rotation, obs.getTextureOrigin(), obs.getBodySize() / obs.getTextureSize(), SpriteEffects.FlipVertically, 0f);

            foreach (Obstacle obs in obs3)
                sb.Draw(obs.getTexture(), obs.getBody().Position, null, Color.White, obs.getBody().Rotation, obs.getTextureOrigin(), obs.getBodySize() / obs.getTextureSize(), SpriteEffects.FlipVertically, 0f);
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
                        AddObstacle(obs, new Vector2(road[i].getBody().Position.X / 2 + 300f, road[0].getBody().Position.Y + 4.5f), world);
                }
                else
                {
                    //if ((i == 3 || i == 7) && !isJumpingObs)
                    //    AddObstacle(obs, new Vector2(road[i].body.Position.X / 2, road[0].body.Position.Y + 4.5f), world);
                    if((i == 5 || i == 9) && isJumpingObs)
                        AddObstacle(obs, new Vector2(road[i].getBody().Position.X / 2 + 100f , road[0].getBody().Position.Y + 2.5f), world);
                }
            }
            return obs;
        }
    }
}

