using MG3_tiled.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MG3_tiled
{
    internal class Box
    {
        public Vector2 pos;
        public Vector2 vel;
        int height = 14;
        int width = 14;
        float boxBoostSpeed = 120;
        int grav = 170;
        Texture2D boxTexture;
        Rectangle bounds;
        bool isCollSpace = false;

        public Box() 
        {
            pos = new Vector2(130, 10);
            vel = new Vector2(0, 0);
            bounds = new Rectangle((int)pos.X, (int)pos.Y, width, height);
        }
        public Box(Vector2 _pos)
        {
            pos = _pos;
            vel = new Vector2(0, 0);
            bounds = new Rectangle((int)pos.X, (int)pos.Y, width, height);
        }


        public void Load(Texture2D _boxTexture)
        {
            boxTexture = _boxTexture;
        }

        public void Update(GameTime gameTime, GraphicsDeviceManager _graphics, List<Rectangle> collisionSolidPlatObjects, List<Rectangle> collisionPassablePlatObjects, Player player)
        {
            // update bounds rectangle
            bounds.X = (int)pos.X;
            bounds.Y = (int)pos.Y;

            // ---
            // COLLISION
            // ---

            // ---
            // GRAVITY/JUMPING PHYSICS
            // ---
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // gravity
            vel.Y = vel.Y + grav * elapsed;

            // box & platform interaction
            // collision with solid platform
            foreach (var rect in collisionSolidPlatObjects)
            {
                if (rect.Intersects(bounds))
                {
                    DealWithCollSolid(rect);
                }
            }

            // collision with small/passable platform
            foreach (var rect in collisionPassablePlatObjects)
            {
                if (rect.Intersects(bounds))
                {
                    DealWithCollPassable(rect);
                }
            }

            // box & witch interaction
            if (player.playerBounds.Right > pos.X && player.playerBounds.X < pos.X + width / 2
                    && player.playerBounds.Bottom > pos.Y && player.playerBounds.Y < pos.Y + height)
            {
                DealWithCollWitch(player.playerBounds, player.playerDirection);
            }


            // box & space bar interaction
            CheckCollSpace(player.playerBounds, player.previousDirection);
            if (isCollSpace && player.playerDirection == 4 && player.animationFrameSpace > 4)
                DealWithCollWitchSpace(player.previousDirection);

            // update box position
            pos.X += vel.X * elapsed;
            pos.Y += vel.Y * elapsed;


            // ---
            // WINDOW LIMITS
            // ---
            CheckLimits(_graphics);

            isCollSpace = false;
        }
        private void CheckCollSpace(Rectangle playerBounds, int previousDirection)
        {
            int w = 104;
            // facing right
            if (previousDirection == 2)
            {
                if (playerBounds.X + w > pos.X && playerBounds.X < pos.X + width
                    && playerBounds.Bottom > pos.Y && playerBounds.Y < pos.Y + height)
                {
                    isCollSpace = true;
                }
            }
            // facing left
            if (previousDirection == 1)
            {
                if (pos.X < playerBounds.Right && pos.X - width > playerBounds.X - w + 5
                && playerBounds.Bottom > pos.Y && playerBounds.Y < pos.Y + height)
                {
                    isCollSpace = true;
                }
            }                
        }
        private void DealWithCollSolid(Rectangle rect)
        {
            // landed on top of platform
            int wiggleRoom = 10;
            if (rect.Y - wiggleRoom <= bounds.Bottom && bounds.Bottom <= rect.Y + wiggleRoom)
            {
                vel.Y = 0;
                //pos.Y = rect.Y - height+wiggleRoom;
            }
            // hit side of platform
            if(bounds.Bottom - 5 > rect.Y) 
            {
                vel.X = 0;
            }
        }

        private void DealWithCollWitch(Rectangle playerBounds, int previousDirection)
        {
            // to left
            if (previousDirection == 1)
            {
                pos.X = playerBounds.X - width / 2;
                vel.X = 0;
            }
            // to right
            if (previousDirection == 2)
            {
                pos.X = playerBounds.Right;
                vel.X = 0;
            }
        }
        private void DealWithCollWitchSpace(int playerDirection)
        {
            // to left
            if (playerDirection == 1)
                vel.X = -boxBoostSpeed;
            // to right
            if (playerDirection == 2)
                vel.X = boxBoostSpeed;
            vel.Y = 0;
        }

        private void DealWithCollPassable(Rectangle rect)
        {
            vel.Y = 0;
            if (bounds.Bottom - 5 > rect.Y)
            {
                vel.X = 0;
            }
        }

        private void CheckLimits(GraphicsDeviceManager _graphics)
        {
            // Check Y limits
            if (pos.Y >= _graphics.PreferredBackBufferHeight - height)
            {
                pos.Y = _graphics.PreferredBackBufferHeight - height;
                vel.Y = 0;
            }
            else if (pos.Y < 0)
            {
                pos.Y = 0;
            }

            // Check X limits
            if (pos.X > _graphics.PreferredBackBufferWidth - width)
            {
                pos.X = _graphics.PreferredBackBufferWidth - width;
            }
            else if (pos.X < 0)
            {
                pos.X = 0;
            }
        }

        public void Draw(SpriteBatch _spriteBatch) 
        {
            _spriteBatch.Begin();

            _spriteBatch.Draw(boxTexture, new Vector2(pos.X, pos.Y - 5), Color.White);

            _spriteBatch.End();
        }
    }
}
