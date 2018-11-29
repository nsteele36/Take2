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
                        Vector2 pos = new Vector2(road[i - 1].body.Position.X + roadTextureSize, road[0].body.Position.Y);
                        AddRoad(road, pos, world);
                    }
                }

                else
                {
                    Vector2 pos = new Vector2(road[i - 1].body.Position.X + roadTextureSize + roadTextureSize / 2, road[0].body.Position.Y);
                    AddRoad(road, pos, world);
                }
            }

            return road;
        }

        public void AddRoad(List<Road> pieces, Vector2 pos, World world)
        {
            Road r = new Road(texture);
            r.bodySize = new Vector2(roadTextureSize, 1f);
            r.body = world.CreateRectangle(r.bodySize.X, r.bodySize.Y, 1f, pos);
            Console.WriteLine("Road Piece created! pos = " + pos);
            r.body.BodyType = BodyType.Static;
            r.body.SetRestitution(0.0f);
            r.body.SetFriction(0.0f);
            r.textureSize = new Vector2(texture.Width, texture.Height);
            r.textureOrigin = r.textureSize / 2f;
            r.world = world;
            r.body.SetCollisionGroup(2);
            pieces.Add(r);
        }

        public List<Road> MoveRoad(List<Road> road, Player _player, World world)
        {
            if (_player.body.Position.X > road[road.Count - 2].body.Position.X)
            {
                Console.WriteLine("new road1 piece created!");
                world.Remove(road[0].body);
                road.RemoveAt(0);
                world.Remove(road[0].body);
                road.RemoveAt(0);
                Vector2 new_pos1 = new Vector2(road[road.Count - 1].body.Position.X + roadTextureSize, road[0].body.Position.Y);
                AddRoad(road, new_pos1, world);
                Vector2 new_pos2 = new Vector2(road[road.Count - 1].body.Position.X + roadTextureSize + roadTextureSize / 2, road[0].body.Position.Y);
                AddRoad(road, new_pos2, world);
            }
            return road;
        }

        public void Draw(SpriteBatch sb, List<Road> r1, List<Road> r2, List<Road> r3)
        {
            //MIDDLE
            foreach (Road piece in r1)
                sb.Draw(piece.texture, piece.body.Position, null, Color.White, piece.body.Rotation, piece.textureOrigin, piece.bodySize / piece.textureSize, SpriteEffects.FlipVertically, 0f);

            //TOP
            foreach (Road piece in r2)
                sb.Draw(piece.texture, piece.body.Position, null, Color.White, piece.body.Rotation, piece.textureOrigin, piece.bodySize / piece.textureSize, SpriteEffects.FlipVertically, 0f);

            //BOTTOM
            foreach (Road piece in r3)
                sb.Draw(piece.texture, piece.body.Position, null, Color.White, piece.body.Rotation, piece.textureOrigin, piece.bodySize / piece.textureSize, SpriteEffects.FlipVertically, 0f);
        }
    }
}




