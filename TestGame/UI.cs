using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TestGame
{
    public class ValueBar
    {
        private Texture2D bar;
        private Texture2D Bg;
        float max;
        private Vector2 position; 

        protected Rectangle part;

        public ValueBar(Texture2D hpBar, Texture2D hpBarBg, float value, Vector2 position)
        {
            bar = hpBar;
            Bg = hpBarBg;

            part = new(0, 0, bar.Width, bar.Height);
            max = value;
            this.position = position;
        }

        public void Draw(SpriteBatch spriteBatch, float value)
        {
            part.Height = (int)(value / max * bar.Height);
            spriteBatch.Draw(Bg, position, Color.White);
            spriteBatch.Draw(bar, position, part, Color.White,0, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        }
    }
}
