using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Take2.Sprites
{
    public class Road : Sprite
    {

        public Road(Texture2D texture) : base(texture)
        {
        }
        /*
public override void Draw(SpriteBatch sb)
{
    DrawRoad(sb);
}


public void DrawRoad(SpriteBatch sb)
{
    sb.Begin();
    for (int i = 0; i < road_pieces.Count; i++)
    {

        sb.Draw(texture, road_pieces[i], null, Color.White, 0.0f,
            new Vector2(0, texture.Height / 2), Vector2.One, SpriteEffects.None, 0.0f);
    }
    sb.End();
}*/
    }
}
