using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class Camera
    {
        public Matrix Transform { get; set; }
        
        // камера будет следить за координатой переданной в follow // CollideBox.x CollideBox.y

        public void follow(CollideBox box, Map map)
        {
            var x = -box.x - box.width / 2;
            var y = -box.y - box.height / 2;
            x = MathHelper.Clamp(x, -map.mapSize.X + Game1.ScreenHeight / 2 + (map.TileSize.X / 2), -Game1.ScreenHeight / 2 + map.TileSize.X / 2);
            y = MathHelper.Clamp(y, -map.mapSize.Y + Game1.ScreenHeight / 2 + (map.TileSize.Y / 2), -Game1.ScreenHeight/2 + map.TileSize.Y/2);
            Transform = Matrix.CreateTranslation(x, y, 0) * Matrix.CreateTranslation(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2, 0);
        }
    }
}
