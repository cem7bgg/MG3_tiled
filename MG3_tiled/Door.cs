using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MG3_tiled
{
    public class Door
    {
        public Vector2 pos;
        public Rectangle bounds;
        public int width = 32;
        public int height = 41;

        Texture2D doorTexture;
        Rectangle doorOpen;
        Rectangle doorClosed;

        public bool isOpen = false;

        int levelFlowerCount = 4;

        public Door(Vector2 _pos)
        {
            pos = _pos;
            bounds = new Rectangle((int)pos.X + width/2, (int)pos.Y, width/2, height);
        }

        public void Load(Texture2D texture)
        {
            doorTexture = texture;
            doorOpen = new Rectangle(0,0, width, height);
            doorClosed = new Rectangle(width,0, width, height);
        }

        public void Update(int flowerCount)
        {
            if (flowerCount >= levelFlowerCount)
            {
                isOpen = true;
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Begin();

            if (isOpen)
            {
                _spriteBatch.Draw(doorTexture, pos, doorOpen, Color.White);
            }
            else
            {
                _spriteBatch.Draw(doorTexture, pos, doorClosed, Color.White);
            }            

            _spriteBatch.End();
        }
    }
}
