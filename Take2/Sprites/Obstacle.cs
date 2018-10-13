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
        //public List<Vector2> obstacles;
        public bool isVisible;

        public Obstacle(Texture2D texture) : base(texture) { }

        protected void AddObstacle(List<Obstacle> o, Vector2 pos, World world)
        {
            Obstacle obs = new Obstacle(texture)
            {
                color = Color.Red
            };
            obs.world = world;
            obs.bodySize = new Vector2(1f, 4f);
            obs.body = world.CreateRectangle(obs.bodySize.X, obs.bodySize.Y, 2f, pos);
            obs.body.BodyType = BodyType.Static;
            obs.body.SetRestitution(0.0f);
            obs.body.SetFriction(0.0f);
            obs.textureSize = new Vector2(obs.texture.Width, obs.texture.Height);
            obs.textureOrigin = obs.textureSize / 2f;
            obs.body.SetCollisionGroup(1);
            o.Add(obs);
        }

        public List<Obstacle> obstacleUpdate(List<Obstacle> obs, List<Road> road, Player _player, int roadNum, bool isJumpingObs, World world)
        {
                if (obs.Count == 0 && _player.isOnRoad && _player.currentRoad == roadNum)
                {
                    if (isJumpingObs)
                        AddObstacle(obs, new Vector2(_player.body.Position.X + 60f, road[0].body.Position.Y + 2.5f), world);
                    else
                        AddObstacle(obs, new Vector2(_player.body.Position.X + 120f, road[0].body.Position.Y + 3.5f), world);
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
                }
            return obs;

            //CREATING OBSTACLES USING ROAD WIP
            /*
            for (int i = 0; i < _road2.Count; i++)
            {
                if (i % 2 == 0)
                {
                    if ((i == 2 || i == 6) && isJumpingObs)
                        AddObstacle(obs, new Vector2(_road2[i].body.Position.X / 2, _road2[0].body.Position.Y + 2.5f));
                    else if ((i == 4 || i == 8) && !isJumpingObs)
                        AddObstacle(obs, new Vector2(_road2[i].body.Position.X / 2, _road2[0].body.Position.Y + 3.5f));
                }
                //else if ( i % 2 != 0 && !isJumpingObs)

            }*/
            //MIDDLE ROAD
        }

        public void Draw(SpriteBatch sb, List<Obstacle> jObs1, List<Obstacle> jObs2, List<Obstacle> jObs3, List<Obstacle> cObs1, List<Obstacle> cObs2, List<Obstacle> cObs3)
        {
            foreach (Obstacle obs in jObs1)
                sb.Draw(obs.texture, obs.body.Position, null, Color.White, obs.body.Rotation, obs.textureOrigin, obs.bodySize / obs.textureSize, SpriteEffects.None, 0f);

            foreach (Obstacle obs in jObs2)
                sb.Draw(obs.texture, obs.body.Position, null, Color.White, obs.body.Rotation, obs.textureOrigin, obs.bodySize / obs.textureSize, SpriteEffects.None, 0f);

            foreach (Obstacle obs in jObs3)
                sb.Draw(obs.texture, obs.body.Position, null, Color.White, obs.body.Rotation, obs.textureOrigin, obs.bodySize / obs.textureSize, SpriteEffects.None, 0f);

            foreach (Obstacle obs in cObs1)
                sb.Draw(obs.texture, obs.body.Position, null, Color.White, obs.body.Rotation, obs.textureOrigin, obs.bodySize / obs.textureSize, SpriteEffects.None, 0f);

            foreach (Obstacle obs in cObs2)
                sb.Draw(obs.texture, obs.body.Position, null, Color.White, obs.body.Rotation, obs.textureOrigin, obs.bodySize / obs.textureSize, SpriteEffects.None, 0f);

            foreach (Obstacle obs in cObs3)
                sb.Draw(obs.texture, obs.body.Position, null, Color.White, obs.body.Rotation, obs.textureOrigin, obs.bodySize / obs.textureSize, SpriteEffects.None, 0f);
            //base.Draw(sb);
        }
    }

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
