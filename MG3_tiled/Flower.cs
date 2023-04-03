using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MG3_tiled
{
    public class Flower
    {
        public Vector2 pos;
        public Rectangle bounds;
        public int width = 14;
        public int height = 16;

        Texture2D flowerTexture;
        Rectangle flowerNotPicked;
        Rectangle flowerPicked;

        public bool isPicked = false;

        public Flower(Vector2 _pos)
        {
            pos = _pos;
            bounds = new Rectangle((int)(pos.X + Math.Floor((float)width / 3)), (int)pos.Y, (int)Math.Floor((float)width / 3), height);
        }

        public void Load(Texture2D texture)
        {
            flowerTexture = texture;
            flowerNotPicked = new Rectangle(0, 0, width, height);
            flowerPicked = new Rectangle(width, 0, width, height);
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Begin();

            if (isPicked)
            {
                _spriteBatch.Draw(flowerTexture, pos, flowerPicked, Color.White);
            }
            else
            {
                _spriteBatch.Draw(flowerTexture, pos, flowerNotPicked, Color.White);
            }

            _spriteBatch.End();
        }

    }
}
