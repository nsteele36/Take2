using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Take2.Sprites;


namespace Take2
{
    public class Camera
    {
        /*
        public Matrix transform;
        Viewport view;
        Vector2 center;

        public Camera(Viewport newView)
        {
            view = newView;
        }

        public void Update(GameTime gameTime, Game1 take2)
        {
            center = new Vector2(take2.player.pos.X + (take2.player.Rectangle.Width / 2) - 400, 0);
            transform = Matrix.CreateScale(new Vector3(1, 1, 0)) *
                Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0));
        }*/
        
        public Matrix Transform { get; private set; }

        public void Follow(Sprite target)
        {
            float pos_x = target.body.Position.X;
            float pos_y = target.body.Position.Y;
            var position = Transform = Matrix.CreateTranslation(
                -pos_x -(target.Rectangle.Width / 2),
                -pos_y -(target.Rectangle.Height / 2), 0);
            var offset = Matrix.CreateTranslation(
                    Game1.ScreenWidth / 2,
                    Game1.ScreenHeight / 2, 0);
            Transform = position * offset;



        }
    }
}
