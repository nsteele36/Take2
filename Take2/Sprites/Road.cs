using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using tainicom.Aether.Physics2D.Dynamics;

namespace Take2.Sprites
{
    public class Road : Sprite
    {
        private readonly float roadTextureSize = 60f;
        public Road(Texture2D texture) : base(texture) { }

        public List<Road> CreateRoad(List<Road> road, int roadNum, World world)
        {
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    if (i == 0)
                    {
                        if (roadNum == 1)
                        {
                            Vector2 pos = new Vector2(0, 0f);
                            AddRoad(road, pos, world);
                        }
                        else if (roadNum == 2)
                        {
                            Vector2 pos = new Vector2(45, 13f);
                            AddRoad(road, pos, world);
                        }
                        else if (roadNum == 3)
                        {
                            Vector2 pos = new Vector2(-45, -13f);
                            AddRoad(road, pos, world);
                        }
                    }
                    else
                    {
                        Vector2 pos = new Vector2(road[i - 1].getBody().Position.X + roadTextureSize, road[0].getBody().Position.Y);
                        AddRoad(road, pos, world);
                    }
                }

                else
                {
                    Vector2 pos = new Vector2(road[i - 1].getBody().Position.X + roadTextureSize + roadTextureSize / 2, road[0].getBody().Position.Y);
                    AddRoad(road, pos, world);
                }
            }

            return road;
        }

        public void AddRoad(List<Road> pieces, Vector2 pos, World world)
        {
            Road r = new Road(getTexture());
            r.setBodySize(new Vector2(roadTextureSize, 1f));
            r.setBody(world.CreateRectangle(r.getBodySize().X, r.getBodySize().Y, 1f, pos));
            Console.WriteLine("Road Piece created! pos = " + pos);
            r.getBody().BodyType = BodyType.Static;
            r.getBody().SetRestitution(0.0f);
            r.getBody().SetFriction(0.0f);
            r.setTextureSize(new Vector2(getTexture().Width, getTexture().Height));
            r.setTextureOrigin(r.getTextureSize() / 2f);
            r.setWorld(world);
            r.getBody().SetCollisionGroup(2);
            pieces.Add(r);
        }

        public List<Road> MoveRoad(List<Road> road, Player _player, World world)
        {
            if (_player.getBody().Position.X > road[road.Count - 2].getBody().Position.X)
            {
                Console.WriteLine("new road1 piece created!");
                world.Remove(road[0].getBody());
                road.RemoveAt(0);
                world.Remove(road[0].getBody());
                road.RemoveAt(0);
                Vector2 new_pos1 = new Vector2(road[road.Count - 1].getBody().Position.X + roadTextureSize, road[0].getBody().Position.Y);
                AddRoad(road, new_pos1, world);
                Vector2 new_pos2 = new Vector2(road[road.Count - 1].getBody().Position.X + roadTextureSize + roadTextureSize / 2, road[0].getBody().Position.Y);
                AddRoad(road, new_pos2, world);
            }
            return road;
        }

        public void Draw(SpriteBatch sb, List<Road> r1, List<Road> r2, List<Road> r3)
        {
            //MIDDLE
            foreach (Road piece in r1)
                sb.Draw(piece.getTexture(), piece.getBody().Position, null, Color.White, piece.getBody().Rotation, piece.getTextureOrigin(), piece.getBodySize() / piece.getTextureSize(), SpriteEffects.FlipVertically, 0f);

            //TOP
            foreach (Road piece in r2)
                sb.Draw(piece.getTexture(), piece.getBody().Position, null, Color.White, piece.getBody().Rotation, piece.getTextureOrigin(), piece.getBodySize() / piece.getTextureSize(), SpriteEffects.FlipVertically, 0f);

            //BOTTOM
            foreach (Road piece in r3)
                sb.Draw(piece.getTexture(), piece.getBody().Position, null, Color.White, piece.getBody().Rotation, piece.getTextureOrigin(), piece.getBodySize() / piece.getTextureSize(), SpriteEffects.FlipVertically, 0f);
        }
    }
}
