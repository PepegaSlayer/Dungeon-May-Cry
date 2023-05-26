using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class Blood
    {
        public CollideBox box;
        public AnimatedSprite sprite;
        public bool isFlipped;

        public Blood(CollideBox box0, AnimatedSprite sp, bool flip)
        { 
            sprite = sp;
            var x = !flip ? box0.x - (box0.width / 4) * 3 : box0.x + box0.width - (box0.width / 3) ;
            box = new CollideBox(123,135, x, box0.y-box0.height/2);
            isFlipped = flip;
        }

        public void draw(SpriteBatch sp) 
        {
            sprite.Draw(sp, isFlipped, box);
        }
    }
}
