using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class AnimatedSprite
    {
        private Texture2D[] frames;
        public int currentFrameIndex;
        private float frameTimer; // таймер для переключения кадров
        private float frameDuration; // длительность кадра в секундах
        private Vector2 PositionDif;
        public int framesCount;
        public bool isEndless;

        private SpriteEffects effects;


        public AnimatedSprite(Texture2D[] frames, float frameDuration, bool end, Vector2 positionDif)
        {
            this.frames = frames;
            this.frameDuration = frameDuration;
            this.isEndless = end;
            PositionDif = positionDif;
            framesCount = frames.Length;
        }

        public void Update(GameTime gameTime)
        {
            // обновляем таймер
            frameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            // если прошло достаточно времени для переключения кадра
            if (frameTimer >= frameDuration)
            {
                // переключаемся на следующий кадр
                currentFrameIndex++;
                if (currentFrameIndex >= frames.Length)
                {
                    if (isEndless)
                        currentFrameIndex = 0;
                }

                // сбрасываем таймер
                frameTimer = 0f;
            }
        }

        public bool isOver()
        {
            return (currentFrameIndex >= frames.Length);
        }

        public void Draw(SpriteBatch spriteBatch, bool isFlipped,  CollideBox collideBox)
        {
            // отрисовываем текущий кадр
            Vector2 Position = new Vector2 (collideBox.x + PositionDif.X, collideBox.y+PositionDif.Y);
            if (isFlipped)
            {
                var rightDif = frames[1].Width - collideBox.width + PositionDif.X;
                effects = SpriteEffects.FlipHorizontally;
                Position.X -= PositionDif.X >= 0 ? frames[1].Width - collideBox.width : PositionDif.X + rightDif;
            }
            else effects = SpriteEffects.None;
            if (currentFrameIndex < frames.Length)
                spriteBatch.Draw(frames[currentFrameIndex], Position, new Rectangle(0, 0, frames[currentFrameIndex].Width, frames[currentFrameIndex].Height), Color.White, 0f, Vector2.Zero, 1f, effects, 0f);
        }


    }
}
