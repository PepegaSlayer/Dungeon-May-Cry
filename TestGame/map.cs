using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class Map
    {
        private readonly Point _mapTileSize = new(10, 10);
        private readonly Sprite[,] tileSet;
        public Point TileSize { get; set; }
        public Point mapSize { get; set; }

        public Map(Texture2D[] tiles)
        {
            tileSet = new Sprite[_mapTileSize.X, _mapTileSize.Y];

            List<Texture2D> textures = new(5);
            textures.AddRange(tiles);

            TileSize = new(textures[0].Width, textures[0].Height);
            mapSize = new(TileSize.X * _mapTileSize.X, TileSize.Y * _mapTileSize.Y);

            Random random = new();

            for (int y = 0; y < _mapTileSize.Y; y++)
            {
                for (int x = 0; x < _mapTileSize.X; x++)
                {
                    int r = random.Next(0, textures.Count);
                    tileSet[x, y] = new(textures[r], new(x * TileSize.X, y * TileSize.Y));
                }
            }
        }

        public void Draw(SpriteBatch sp)
        {
            for (int y = 0; y < _mapTileSize.Y; y++)
            {
                for (int x = 0; x < _mapTileSize.X; x++) tileSet[x, y].Draw(sp);
            }
        }
    }
}
