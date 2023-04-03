using MG3_tiled.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using TiledCS;

namespace MG3_tiled
{

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // for bg
        Texture2D bgImage;
        // for tiled map
        private TiledMap map;
        private Dictionary<int, TiledTileset> tilesets;
        private Texture2D tilesetTexture;
        private TileMapManager mapManager;

        // for player
        private Player player;
        Texture2D runRight;
        Texture2D runLeft;
        Texture2D idleRight;
        Texture2D idleLeft;
        Texture2D jumpRight;
        Texture2D jumpLeft;
        Texture2D spaceRight;
        Texture2D spaceLeft;

        // for box
        private Box box;
        Texture2D boxTexture;

        // for door
        Texture2D doorTexture;
        private TiledLayer doorLayer;
        private Door door;

        // for flower
        Texture2D flowerTexture;
        private TiledLayer flowerLayer;
        private List<Flower> flowerObjects;

        // Map Layers
        // for collisions
        private TiledLayer collisionLayerBigPlat;
        private List<Rectangle> collisionBigPlatObjects;
        private TiledLayer collisionLayerSmallPlat;
        private List<Rectangle> collisionSmallPlatObjects;
        // player
        private TiledLayer playerStart;
        private Vector2 playerInitPos;
        // box
        private TiledLayer boxStart;
        private Vector2 boxInitPos;

        // font
        // cite for using fonts: http://rbwhitaker.wikidot.com/monogame-drawing-text-with-spritefonts
        private SpriteFont font;
                

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            player = new Player();
            box = new Box();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // LOAD BG IMAGE
            bgImage = Content.Load<Texture2D>("backgroundSky");

            // LOAD TILE MAP
            map = new TiledMap(Content.RootDirectory + "\\map3.tmx");
            tilesets = map.GetTiledTilesets(Content.RootDirectory + "/"); 
            tilesetTexture = Content.Load<Texture2D>("env");
            mapManager = new TileMapManager(_spriteBatch, map, tilesets, tilesetTexture);

            // collision layers
            // big platforms
            collisionLayerBigPlat = map.Layers.First(l => l.name == "CollisionsBigPlat");
            collisionBigPlatObjects = new List<Rectangle>();
            foreach (var o in collisionLayerBigPlat.objects)
            {
                collisionBigPlatObjects.Add(new Rectangle((int)o.x, (int)o.y, (int)o.width, (int)o.height));//Adding the rectangles to the list
            }
            // smaller platforms
            collisionLayerSmallPlat = map.Layers.First(l => l.name == "CollisionsSmallPlat");
            collisionSmallPlatObjects = new List<Rectangle>();
            foreach (var o in collisionLayerSmallPlat.objects)
            {
                collisionSmallPlatObjects.Add(new Rectangle((int)o.x, (int)o.y, (int)o.width, (int)o.height));//Adding the rectangles to the list
            }
            
            // player pos layer
            playerStart = map.Layers.First(l => l.name == "PlayerStart");
            foreach (var o in playerStart.objects)
            {
                playerInitPos = new Vector2(o.x, o.y);
                player.pos = playerInitPos;
            }
            
            // LOAD PLAYER SPRITE
            runRight = Content.Load<Texture2D>("Blue_witch/B_witch_runRight(32x384)");
            runLeft = Content.Load<Texture2D>("Blue_witch/B_witch_runLeft(32x384)");
            idleRight = Content.Load<Texture2D>("Blue_witch/B_witch_idleRight(32x288)");
            idleLeft = Content.Load<Texture2D>("Blue_witch/B_witch_idleLeft(32x288)");
            jumpRight = Content.Load<Texture2D>("Blue_witch/B_witch_chargeRight(48x240)");
            jumpLeft = Content.Load<Texture2D>("Blue_witch/B_witch_chargeLeft(48x240)");
            spaceRight = Content.Load<Texture2D>("Blue_witch/B_witch_attackRight(104x414)");
            spaceLeft = Content.Load<Texture2D>("Blue_witch/B_witch_attackLeft(104x414)");
            player.Load(runRight, runLeft, idleRight, idleLeft, jumpRight, jumpLeft, spaceRight, spaceLeft);

            // box pos layer
            boxStart = map.Layers.First(l => l.name == "BoxStart");
            foreach (var o in boxStart.objects)
            {
                boxInitPos = new Vector2(o.x, o.y);
                box.pos = boxInitPos;
            }

            // LOAD BOX SPRITE
            boxTexture = Content.Load<Texture2D>("Platform/boxSmall");
            box.Load(boxTexture);

            // LOAD DOOR SPRITE
            doorLayer = map.Layers.First(l => l.name == "Door");
            Vector2 doorPos = new Vector2(0,0);
            foreach (var o in doorLayer.objects)
            {
                doorPos = new Vector2(o.x, o.y);
            }
            door = new Door(doorPos);
            doorTexture = Content.Load<Texture2D>("Platform/door");
            door.Load(doorTexture);

            // LOAD FLOWER SPRITE
            flowerLayer = map.Layers.First(l => l.name == "Flowers");
            flowerObjects = new List<Flower>();
            foreach (var o in flowerLayer.objects)
            {
                flowerObjects.Add(new Flower(new Vector2(o.x, o.y)));
            }
            flowerTexture = Content.Load<Texture2D>("Platform/flower");
            foreach (var f in flowerObjects)
            {
                f.Load(flowerTexture);
            }

            // load font
            font = Content.Load<SpriteFont>("Score");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // exit from game if "enter" opend door
            if (player.playerBounds.Intersects(door.bounds) && door.isOpen)
                Exit();

            // TODO: Add your update logic here
            player.Update(gameTime);
            player.UpdatePhysicsAndColl(gameTime, _graphics, collisionBigPlatObjects, collisionSmallPlatObjects, flowerObjects);



            box.Update(gameTime, _graphics, collisionBigPlatObjects, collisionSmallPlatObjects, player);

            door.Update(player.flowerCount);

            base.Update(gameTime);
        }

        

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSeaGreen); // CornflowerBlue);

            //_spriteBatch.Begin();
            //_spriteBatch.Draw(bgImage, new Vector2(0, 0), Color.White);
            //_spriteBatch.End();

            mapManager.Draw();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(font, "flower count: " + player.flowerCount, new Vector2(10, 10), Color.White);
            _spriteBatch.End();

            door.Draw(_spriteBatch);
                      
            player.Draw(_spriteBatch);

            foreach (var f in flowerObjects)
            {
                f.Draw(_spriteBatch);
            }

            box.Draw(_spriteBatch);

            

            base.Draw(gameTime);
        }
    }
}