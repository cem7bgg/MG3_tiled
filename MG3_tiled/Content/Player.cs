using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
//using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TiledCS;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MG3_tiled.Content
{
    public class Player
    {
        public Vector2 pos;
        public Vector2 vel;
        private float speed = 150;
        public Rectangle playerBounds;

        // for jumping
        public int grav = 240;
        public int speedJump = -250;
        public bool isJumping = false;
        public bool inAir = true;

        // initialize witch
        Texture2D runRight;
        Texture2D runLeft;
        Texture2D idleRight;
        Texture2D idleLeft;
        Texture2D jumpRight;
        Texture2D jumpLeft;
        Texture2D spaceRight;
        Texture2D spaceLeft;
        public int witchWidth = 25;
        public int witchHeight = 48;


        // for animation
        Rectangle[] runRightAnim;
        Rectangle[] runLeftAnim;
        Rectangle[] idleRightAnim;
        Rectangle[] idleLeftAnim;
        Rectangle[] jumpRightAnim;
        Rectangle[] jumpLeftAnim;
        Rectangle[] spaceRightAnim;
        Rectangle[] spaceLeftAnim;

        float timerRun;
        float timerIdle;
        float timerJump;
        float timerSpace;

        int animSpeed = 100;
        int animationFrameRun;
        int animationFrameIdle;
        int animationFrameJump;
        public int animationFrameSpace;

        public int playerDirection;    // 1 = Left, 2 = Right, 3 = Up, 4 = Space
        public int previousDirection;

        //public Texture2D texture;

        // for flower counting
        public int flowerCount = 0;


        public Player() 
        { 
            pos = new Vector2(200, 350);
            vel = new Vector2(0, 0);
            playerBounds = new Rectangle((int)pos.X, (int)pos.Y, witchWidth, witchHeight);
        }
        public Player(Vector2 _pos)
        {
            pos = _pos;
            vel = new Vector2(0, 0);
            playerBounds = new Rectangle((int)pos.X, (int)pos.Y, witchWidth, witchHeight);
        }

        public void Load(Texture2D _runRight, Texture2D _runLeft, Texture2D _idleRight, Texture2D _idleLeft, Texture2D _jumpRight, Texture2D _jumpLeft, Texture2D _spaceRight, Texture2D _spaceLeft)
        {
            runRight = _runRight;
            runLeft = _runLeft;
            idleRight = _idleRight;
            idleLeft = _idleLeft;
            jumpRight = _jumpRight;
            jumpLeft = _jumpLeft;
            spaceRight = _spaceRight;
            spaceLeft = _spaceLeft;

            // ---
            // LOADING IN ANIMATIONS
            // ---
            // credit animation from: https://9e0.itch.io/witches-pack

            // run right & left
            runRightAnim = new Rectangle[8];
            runLeftAnim = new Rectangle[8];

            for (int j = 0; j < runRightAnim.Length; j++)
            {
                runRightAnim[j] = new Rectangle(0, 48 * j, 32, 48);
                runLeftAnim[j] = new Rectangle(0, 48 * j, 32, 48);
            }

            // idle right & left
            idleRightAnim = new Rectangle[6];
            idleLeftAnim = new Rectangle[6];

            for (int j = 0; j < idleRightAnim.Length; j++)
            {
                idleRightAnim[j] = new Rectangle(0, 48 * j, 32, 48);
                idleLeftAnim[j] = new Rectangle(0, 48 * j, 32, 48);
            }


            // jump right & left
            jumpRightAnim = new Rectangle[5];
            jumpLeftAnim = new Rectangle[5];

            for (int j = 0; j < jumpRightAnim.Length; j++)
            {
                jumpRightAnim[j] = new Rectangle(0, 48 * j, 48, 48);
                jumpLeftAnim[j] = new Rectangle(0, 48 * j, 48, 48);
            }


            // space right & left
            spaceRightAnim = new Rectangle[9];
            spaceLeftAnim = new Rectangle[9];

            for (int j = 0; j < spaceRightAnim.Length; j++)
            {
                spaceRightAnim[j] = new Rectangle(0, 46 * j, 104, 46);
                spaceLeftAnim[j] = new Rectangle(0, 46 * j, 104, 46);
            }

            // ---
            // INITIALIZE VARIABLES
            // ---

            // initalizing animation timers
            timerRun = 0;
            timerIdle = 0;
            timerJump = 0;
            timerSpace = 0;

            // initalizing initial animation frame
            animationFrameRun = 0;
            animationFrameIdle = 0;
            animationFrameJump = 0;
            animationFrameSpace = 0;

            // initializing to face right
            previousDirection = 2;
        }
        

        public void Update(GameTime gameTime)
        {
            // default to idle
            playerDirection = 0;

            // ---
            // USING KEYS TO MOVE
            // ---

            // A = left, D = right, W = jump
            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.W) && (!inAir))// || isCollPlatTop))
            {
                playerDirection = 3;

                isJumping = true;
                inAir = true;
            }
            if (kstate.IsKeyDown(Keys.A))
            {
                pos.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                playerDirection = 1;
                previousDirection = 1;
            }
            if (kstate.IsKeyDown(Keys.D))
            {
                pos.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                playerDirection = 2;
                previousDirection = 2;
            }
            if (kstate.IsKeyDown(Keys.Space))
            {
                playerDirection = 4;
            }
            // update bounds rectangle
            playerBounds.X = (int)pos.X;
            playerBounds.Y = (int)pos.Y;
        }

        public void UpdatePhysicsAndColl(GameTime gameTime, GraphicsDeviceManager _graphics, List<Rectangle> collisionSolidPlatObjects, List<Rectangle> collisionPassablePlatObjects, List<Flower> flowerObjects)
        {
            // ---
            // GRAVITY/JUMPING PHYSICS
            // ---
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // applying gravity
            vel.Y = vel.Y + grav * elapsed;


            // ---
            // COLLISIONS
            // ---
            // collision with big/solid platform
            // code reference: https://lioncatdevstudio.blogspot.com/2021/01/tile-map-collisions.html
            foreach (var rect in collisionSolidPlatObjects)
            {
                if (rect.Intersects(playerBounds))
                {
                    DealWithCollSolid(rect);
                    break;
                }
            }

            // collision with small/passable platform
            foreach (var rect in collisionPassablePlatObjects)
            {
                if (rect.Intersects(playerBounds))
                {
                    DealWithCollPassable(rect);
                    break;
                }
            }

            // collision with flowers
            foreach (var f in flowerObjects)
            {
                if (f.bounds.Intersects(playerBounds) && f.isPicked == false)
                {
                    f.isPicked= true;
                    flowerCount++;
                }
            }

            if (isJumping)
            {
                vel.Y = speedJump;
            }
            pos.Y += vel.Y * elapsed;

            // reset variables 
            if (vel.Y != 0)
            {
                inAir = true;
            }
            isJumping = false;

            // ---
            // WINDOW LIMITS
            // ---
            CheckLimits(_graphics);


            UpdateAnimation(gameTime);
        }

        private void CheckLimits(GraphicsDeviceManager _graphics)
        {
            // Check Y limits
            if (pos.Y >= _graphics.PreferredBackBufferHeight - witchHeight)
            {
                pos.Y = _graphics.PreferredBackBufferHeight - witchHeight;
                inAir = false;
                vel.Y = 0;
            }
            else if (pos.Y < 0)
            {
                pos.Y = 0;
                vel.Y = 0;
            }

            // Check X limits
            if (pos.X > _graphics.PreferredBackBufferWidth - witchWidth)
            {
                pos.X = _graphics.PreferredBackBufferWidth - witchWidth;
            }
            else if (pos.X < 0)
            {
                pos.X = 0;
            }
        }

        private void DealWithCollSolid(Rectangle rect)
        {
            int wiggleRoom = 5;
            // landed on top of platform
            if ((rect.Y - wiggleRoom <= pos.Y + 40 && pos.Y + 40 <= rect.Y + wiggleRoom))
            {
                vel.Y = 0;
                inAir = false;
            }
            // hitting bottom of platform 
            else if ((rect.Bottom - wiggleRoom >= pos.Y)) //} && playerPosition.Y  <= platformJumpTopPos.Y + platJTopHeight + 5))
            {
                vel.Y = grav - 100;
            }
            // hitting left side of platform
            if ((rect.X >= pos.X + witchWidth - wiggleRoom))
            {
                pos.X = rect.X - witchWidth;
            }
            else if ((rect.Right - wiggleRoom <= pos.X))
            {
                pos.X = rect.Right;
            }
        }

        private void DealWithCollPassable(Rectangle rect)
        {
            int wiggleRoom = 5;
            if ((rect.X - witchWidth) < pos.X && pos.X < (rect.Right)
                && (rect.Y - wiggleRoom <= pos.Y + 40 && pos.Y + 40 <= rect.Y + wiggleRoom)
                && vel.Y >= 0)
            {
                //playerPosition.Y = platformJumpThruPos.Y - 40;
                vel.Y = 0;
                inAir = false;
            }
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            // ---
            // UPDATE ANIMATION FRAMES
            // ---

            // Run frames
            if (timerRun > animSpeed)
            {
                if (animationFrameRun < runRightAnim.Length - 1)
                {
                    animationFrameRun++;
                }
                else
                {
                    animationFrameRun = 0;
                }
                // reset timer
                timerRun = 0;
            }
            else
            {
                timerRun += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            // Idle frames
            if (timerIdle > animSpeed)
            {
                if (animationFrameIdle < idleRightAnim.Length - 1)
                {
                    animationFrameIdle++;
                }
                else
                {
                    animationFrameIdle = 0;
                }
                // reset timer
                timerIdle = 0;
            }
            else
            {
                timerIdle += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            // Jump frames
            if (timerJump > animSpeed)
            {
                if (animationFrameJump < jumpRightAnim.Length - 1)
                {
                    animationFrameJump++;
                }
                else
                {
                    animationFrameJump = 0;
                }
                // reset timer
                timerJump = 0;
            }
            else
            {
                timerJump += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            // Space frames
            if (playerDirection == 4)
            {

                if (timerSpace > animSpeed)
                {
                    if (animationFrameSpace < spaceRightAnim.Length - 1)
                    {
                        animationFrameSpace++;
                    }
                    else
                    {
                        animationFrameSpace = 0;
                    }
                    // reset timer
                    timerSpace = 0;
                }
                else
                {
                    timerSpace += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }
            // reset space animation everytime let go of space
            if (playerDirection != 4)
            {
                animationFrameSpace = 0;
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Begin();
            // ---
            // DRAWING WITCH
            // ---
            //_spriteBatch.Draw(runRight, playerPosition, runRightAnim[animationFrameRun], Color.White);
            // 1 = Left, 2 = Right, 3 = Up, 4 = Space

            // jumping
            if ((playerDirection == 3 || inAir == true) && previousDirection == 1)
            {
                // shift origin 
                _spriteBatch.Draw(jumpLeft, pos, jumpLeftAnim[animationFrameJump], Color.White, 0f, new Vector2(9, 0), Vector2.One, SpriteEffects.None, 0f);
            }
            else if ((playerDirection == 3 || inAir == true) && previousDirection == 2)
            {
                // shift origin 
                _spriteBatch.Draw(jumpRight, pos, jumpRightAnim[animationFrameJump], Color.White, 0f, new Vector2(9, 0), Vector2.One, SpriteEffects.None, 0f);
            }
            // running
            else if (playerDirection == 1)
            {
                _spriteBatch.Draw(runLeft, pos, runLeftAnim[animationFrameRun], Color.White);
            }
            else if (playerDirection == 2)
            {
                _spriteBatch.Draw(runRight, pos, runRightAnim[animationFrameRun], Color.White);
            }
            // space
            else if (playerDirection == 4 && previousDirection == 1)
            {
                // have to adjust origin for left space effect because when flipped, witch is shifted to the right
                _spriteBatch.Draw(spaceLeft, pos, spaceLeftAnim[animationFrameSpace], Color.White, 0f, new Vector2(72, 0), Vector2.One, SpriteEffects.None, 0f);
            }
            else if (playerDirection == 4 && previousDirection == 2)
            {
                _spriteBatch.Draw(spaceRight, pos, spaceRightAnim[animationFrameSpace], Color.White);
            }
            // idle
            //playerDirection == 0 (idle)
            else if (previousDirection == 1)
            {
                _spriteBatch.Draw(idleLeft, pos, idleLeftAnim[animationFrameIdle], Color.White);
            }
            else if (previousDirection == 2)
            {
                _spriteBatch.Draw(idleRight, pos, idleRightAnim[animationFrameIdle], Color.White);
            }

            _spriteBatch.End();
        }
    
    }
}
