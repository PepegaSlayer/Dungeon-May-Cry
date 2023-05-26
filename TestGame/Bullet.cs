using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class Bullet
    {
        public CollideBox box;
        public AnimatedSprite sprite;
        float deltaX;
        float deltaY;
        public int damage;
        public Bullet(int x, int y, int x1, int y1, Texture2D[] textures, int d0 ) 
        { 
            sprite = new AnimatedSprite(textures, 0.1f, true, Vector2.Zero);
            box = new CollideBox(textures[1].Width, textures[1].Height, x, y);
            deltaX = x1 - x;
            deltaY = y1 - y;
            damage = d0;
        }

        public void update(Player player, GameTime gameTime) 
        {
          
            float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            float speed = 10f;

            sprite.Update(gameTime);
            float moveX = speed * deltaX / distance;
            float moveY = speed * deltaY / distance;
            box.x += (int)moveX;
            box.y += (int)moveY;
         }

        public void draw(SpriteBatch sp) 
        {
            sprite.Draw(sp, false, box);
        }

    }
}
