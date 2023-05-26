using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public struct Sprite
    {
        private readonly Texture2D texture;
        private Vector2 Position { get; set; }
        private Vector2 Origin { get; set;}

        public Sprite(Texture2D t, Vector2 pos) 
        { 
            Position = pos;
            texture = t;
            Origin = new(t.Width / 2, t.Height / 2);
        }
        public void Draw(SpriteBatch sp) 
        {
            sp.Draw(texture, Position, null, Color.White, 0f, Origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
