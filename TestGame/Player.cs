using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    // Состояния игрока

    public enum Status
    {
        idle,
        run,
        slash,
        doubleSlash,
        slashWait,
        die,
        dash,
        slam,
        slamWait,
        spin,
        stun
    }

    public class Player
    {
        public int Speed { get; set; }
        public float Health { get; set; }
        public int stanTime;
        private Vector2 minPos, maxPos;

        public CollideBox CollideBox;

        public Dictionary<Status, AnimatedSprite> Animations = new Dictionary<Status, AnimatedSprite>();
        public Status status { get; set; }
        public Queue<Status> nextAttack = new Queue<Status>();
        public bool isFlipped;
        public int kills;

        // список атак игорка

        public Dictionary<Status, Attack> attacks = new Dictionary<Status, Attack>()
        {
            {Status.slash, new Attack("Slash", 20, 300)},
            {Status.doubleSlash, new Attack("doubleSlash", 30, 320)},
            {Status.slam, new Attack("Slam", 40, 200)},
            {Status.spin, new Attack("Spin", 25, 300)}
        };

        public Player(int x, int y, int speed, int health, Point mapSize, Point tileSize)
        {
            CollideBox = new CollideBox(170, 145, x, y);
            Speed = speed;
            minPos = new((-tileSize.X / 2), (-tileSize.Y / 2) );
            maxPos = new(mapSize.X - CollideBox.width - (tileSize.X / 2), mapSize.Y - CollideBox.height - (tileSize.Y / 2));
            Health = health;
        }

        // Метод Атаки игрока

        private void PerformAttack(Attack attack, List<Enemy> enemies, List<Blood> Blood, Texture2D[] bloodTexture, SoundEffect bloodSplash)
        {   
            List<CollideBox> attackBoxes = new List<CollideBox> { new CollideBox(attack.distance, CollideBox.height - 40, isFlipped ? CollideBox.x - attack.distance : CollideBox.x + CollideBox.width, CollideBox.y + 20) };
            if (attack.name == "Spin")
                attackBoxes.Add(new CollideBox(attack.distance, CollideBox.height - 40, isFlipped ? CollideBox.x + CollideBox.width : CollideBox.x - attack.distance / 2, CollideBox.y + 20));
            foreach (Enemy enemy in enemies)
            {
                foreach (var attackbox in attackBoxes)
                if (attackbox.Intersects(enemy.CollideBox)&&(enemy.status!= EnemyState.die))
                {
                    Random rnd = new Random();
                    enemy.Health -= attack.damage * (kills*2/100 + 1);
                    enemy.CollideBox.x -= enemy.CollideBox.x > CollideBox.x ? -rnd.Next(20, 50) : rnd.Next(20, 50);
                    Blood.Add(new Blood (enemy.CollideBox,new AnimatedSprite(bloodTexture, 0.05f, false, new (0,0)), enemy.IsFlipped));
                    bloodSplash.Play();
                }
            }
        }

        public void Update(KeyboardState currentKeyboardState, KeyboardState previousKeyboardState, List<Enemy> enemies, List<Blood> Blood, Texture2D[] bloodTexture, SoundEffect[] blades, SoundEffect bloodSplash)
        {
            
            if ((status != Status.die)&&(status != Status.stun))
            {
                if (currentKeyboardState.IsKeyDown(Keys.H) && previousKeyboardState.IsKeyUp(Keys.H))
                {
                    
                    if (status == Status.slashWait)
                    {
                        status = Status.doubleSlash;
                        blades[1].Play();
                        PerformAttack(attacks[status], enemies, Blood, bloodTexture, bloodSplash);
                    }
                    else if (nextAttack.Count() == 0)
                    {
                        blades[1].Play();
                        status = Status.slash;
                        Animations[Status.slashWait].currentFrameIndex = 0;
                        Animations[Status.doubleSlash].currentFrameIndex = 0;
                        nextAttack.Enqueue(Status.slashWait);
                        PerformAttack(attacks[status], enemies, Blood, bloodTexture, bloodSplash);
                    }
                }

                if (currentKeyboardState.IsKeyDown(Keys.J) && previousKeyboardState.IsKeyUp(Keys.J))
                {
                  
                    if (status == Status.slamWait)
                    {
                        status = Status.spin;
                        blades[1].Play();
                        PerformAttack(attacks[status], enemies, Blood, bloodTexture, bloodSplash);
                    }
                    else if (nextAttack.Count() == 0)
                    {
                        blades[1].Play();
                        status = Status.slam;
                        Animations[Status.slamWait].currentFrameIndex = 0;
                        Animations[Status.spin].currentFrameIndex = 0;
                        nextAttack.Enqueue(Status.slamWait);
                        PerformAttack(attacks[status], enemies, Blood, bloodTexture, bloodSplash);
                    }
                }

                if (currentKeyboardState.IsKeyDown(Keys.Space) && previousKeyboardState.IsKeyUp(Keys.Space))
                {
                    status = Status.dash;
                    CollideBox.x -= isFlipped ? 150 : -150;
                    if (currentKeyboardState.IsKeyDown(Keys.W))
                        CollideBox.y -= 100;
                    else if (currentKeyboardState.IsKeyDown(Keys.S))
                        CollideBox.y += 100;
                }

                if (currentKeyboardState.IsKeyDown(Keys.R) && previousKeyboardState.IsKeyUp(Keys.R) && (kills > 5))
                {
                    kills = 0;
                    Health += 50;
                }
            }
            else if (status == Status.stun) stanTime--;

            if ((stanTime == 0)&&(status == Status.stun))
                status = Status.idle;

            if (Health < 0) status = Status.die;
           
            // Проверка кончились ли конечные анимации (удары)

            foreach (var animation in Animations.Values.Where(x => !x.isEndless))
            {
                if (animation.isOver())
                {
                    status = nextAttack.Count() > 0 ? nextAttack.Dequeue() : Status.idle;
                    animation.currentFrameIndex = 0;
                   
                }
               
            }
            
        }

        public void Move(KeyboardState keyboardState)
        {
            if ((keyboardState.GetPressedKeys().Length == 0)&&(status != Status.stun))
                status = Status.idle;
            else if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.D))
                status = Status.run;
            if (keyboardState.IsKeyDown(Keys.W))
            {
                CollideBox.y -= Speed;
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                CollideBox.y += Speed;
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                CollideBox.x -= Speed;
                isFlipped = true;
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                CollideBox.x += Speed;
                isFlipped = false;
            }

            // Игрок не должен выходить за карту

            if (CollideBox.x<minPos.X) CollideBox.x = (int)minPos.X;
            if (CollideBox.x > maxPos.X) CollideBox.x = (int)maxPos.X;
            if (CollideBox.y < minPos.Y) CollideBox.y = (int)minPos.Y;
            if (CollideBox.y > maxPos.Y) CollideBox.y = (int)maxPos.Y;

        }

    }
}
