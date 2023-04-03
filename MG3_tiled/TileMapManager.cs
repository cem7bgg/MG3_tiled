using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiledCS;

namespace MG3_tiled
{
    // Class inspiration here: https://lioncatdevstudio.blogspot.com/2021/01/loading-tile-map-and-collision.html
    public class TileMapManager
    {
        private SpriteBatch spriteBatch;
        private TiledMap map;
        private Dictionary<int, TiledTileset> tilesets;
        private Texture2D tilesetTexture;
        //int tilesetTilesWide;
        //int tileWidth;
        //int tileHeight;

        public TileMapManager(SpriteBatch _spriteBatch, TiledMap _map, Dictionary<int, TiledTileset> _tilesets, Texture2D _tilesetTexture)//, int _tilesetTilesWide, int _tileWidth, int _tileHeight) 
        {
            spriteBatch = _spriteBatch;
            map = _map;
            tilesets = _tilesets;
            tilesetTexture = _tilesetTexture;
            //tilesetTilesWide = _tilesetTilesWide;
            //tileWidth = _tileWidth;
            //tileHeight = _tileHeight;
        }

        public void Draw()
        {
            // cite for drawing code: https://github.com/ironcutter24/TiledCS-example-MonoGame/blob/main/Game1.cs
            spriteBatch.Begin();

            var tileLayers = map.Layers.Where(x => x.type == TiledLayerType.TileLayer);

            foreach (var layer in tileLayers)
            {
                for (var y = 0; y < layer.height; y++)
                {
                    for (var x = 0; x < layer.width; x++)
                    {
                        var index = (y * layer.width) + x; // Assuming the default render order is used which is from right to bottom
                        var gid = layer.data[index]; // The tileset tile index
                        var tileX = x * map.TileWidth;
                        var tileY = y * map.TileHeight;

                        // Gid 0 is used to tell there is no tile set
                        if (gid == 0)
                        {
                            continue;
                        }

                        // Helper method to fetch the right TieldMapTileset instance
                        // This is a connection object Tiled uses for linking the correct tileset to the gid value using the firstgid property
                        var mapTileset = map.GetTiledMapTileset(gid);

                        // Retrieve the actual tileset based on the firstgid property of the connection object we retrieved just now
                        var tileset = tilesets[mapTileset.firstgid];

                        // Use the connection object as well as the tileset to figure out the source rectangle
                        var rect = map.GetSourceRect(mapTileset, tileset, gid);

                        // Create destination and source rectangles
                        var source = new Rectangle(rect.x, rect.y, rect.width, rect.height);
                        var destination = new Rectangle(tileX, tileY, map.TileWidth, map.TileHeight);


                        // Render sprite at position tileX, tileY using the rect
                        spriteBatch.Draw(tilesetTexture, destination, source, Color.White);
                    }
                }
            }

            spriteBatch.End();

        }
    }
}
