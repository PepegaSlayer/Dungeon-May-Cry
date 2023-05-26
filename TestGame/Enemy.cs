using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static TestGame.Enemy;

namespace TestGame
{
    public enum EnemyState
    {
        move,
        attackA,
        attackB,
        idle,
        die
    }

    public class Enemy
    {
        public int existingTime = 1000;
        public int Speed { get; set; }
        public int Health { get; set; }
        public bool IsFlipped { get; set; }
     
        public CollideBox CollideBox = new CollideBox(0, 0, 0, 0);

        public Dictionary<EnemyState, AnimatedSprite> Animations = new Dictionary<EnemyState, AnimatedSprite>();
        public EnemyState status { get; set; }
        public Dictionary<EnemyState, Attack> attacks = new Dictionary<EnemyState, Attack>();

        public Enemy(int x0, int y0, int Speed0, int hp, Dictionary<EnemyState, AnimatedSprite> Animations0, Dictionary<EnemyState, Attack> attacks0, int w0, int h0)
        {
            Speed = Speed0;
            Health = hp;
            Animations = Animations0;
            attacks = attacks0;
            CollideBox.width = w0;
            CollideBox.height = h0;
            CollideBox.x = x0;
            CollideBox.y = y0;
        }

       
        public void Move(int pX, int pY, Enemy[] enemies)
        {
            bool collideFlag = false;
            foreach (Enemy enemy in enemies) 
                if ((Math.Abs(enemy.CollideBox.x - CollideBox.x) <= 30) && (Math.Abs(enemy.CollideBox.y - CollideBox.y) <= 30))
                    CollideBox.y = enemy.CollideBox.y > CollideBox.y ? CollideBox.y - Speed : CollideBox.y + Speed;
            if (!collideFlag) {
                if (CollideBox.x < pX)
                {
                    CollideBox.x += Speed;
                    IsFlipped = false;
                }
                else if (CollideBox.x > pX)
                {
                    CollideBox.x -= Speed;
                    IsFlipped = true;
                }
                CollideBox.y = pY > CollideBox.y ? CollideBox.y + Speed : CollideBox.y - Speed;
            }
            //else 
        }

        public virtual void Update(Player player, List<Bullet> bullets, Texture2D[] bullet, SoundEffect[] sound, Enemy[] enemies) { }
    }

    // Противник - танк, рыцарь с секирой

    public class EnemySlamer : Enemy
    {
        private bool flag = false;
        public EnemySlamer(int x0, int y0, int Speed0, int hp, Dictionary<EnemyState, AnimatedSprite> Animations0, Dictionary<EnemyState, Attack> attacks0, int w0, int h0) : base(x0, y0, Speed0, hp, Animations0, attacks0, w0, h0)
        {

        }

        public override void Update(Player player, List<Bullet> bullets, Texture2D[] bullet, SoundEffect[] sounds, Enemy[] enemies)
        {
            if (status != EnemyState.die)
            {
                var px = player.CollideBox.x + player.CollideBox.width / 2;
                var py = player.CollideBox.y;
                var x = IsFlipped ? CollideBox.x + CollideBox.width : CollideBox.x;

                if (((py - CollideBox.y) * (py - CollideBox.y) < 50 * 50) && ((attacks[EnemyState.attackA].distance) * (attacks[EnemyState.attackA].distance) / 2 > (px - x) * (px - x)) && (status != EnemyState.attackA))
                    status = EnemyState.attackA;




                foreach (var animation in Animations.Values.Where(x => x.isEndless == false))
                {
                    if ((animation.currentFrameIndex == animation.framesCount / 2) && (flag == false))
                    {
                        PerformAttack(attacks[EnemyState.attackA], player, sounds);
                        flag = true;
                       
                    }
                    if (animation.isOver())
                    {
                        status = EnemyState.move;
                        flag = false;
                        animation.currentFrameIndex = 0;
                    }
                }

                if (status == EnemyState.move) Move(px, py, enemies);
            }
            else existingTime--;
        }

        public void PerformAttack(Attack attack, Player player, SoundEffect[] sounds)
        {
            CollideBox attackBox = new CollideBox(attack.distance, CollideBox.height - 40, IsFlipped ? CollideBox.x - attack.distance : CollideBox.x + CollideBox.width, CollideBox.y + 20);
            sounds[1].Play();
            if (attackBox.Intersects(player.CollideBox)&&(player.status != Status.dash))
                player.Health -= attack.damage;
        }
    }

    // противник маг

    public class EnemyWizzard : Enemy
    {
        public double reloadTime = 0; 
        public EnemyWizzard(int x0, int y0, int Speed0, int hp, Dictionary<EnemyState, AnimatedSprite> Animations0, Dictionary<EnemyState, Attack> attacks0, int w0, int h0) : base(x0, y0, Speed0, hp, Animations0, attacks0, w0, h0)
        {

        }

        public override void Update(Player player, List<Bullet> bullets, Texture2D[] bullet, SoundEffect[] sounds, Enemy[] enemies)
        {
            if (status != EnemyState.die)
            {
                var px = player.CollideBox.x + player.CollideBox.width / 2;
                var py = player.CollideBox.y + player.CollideBox.height / 2;
                var x = IsFlipped ? CollideBox.x + CollideBox.width : CollideBox.x;
                var y = CollideBox.y + CollideBox.height / 2;
                var dist = Math.Sqrt(Math.Pow(px - x, 2) + Math.Pow(py - y, 2));

                reloadTime = reloadTime > 0 ? reloadTime - 1 : 0;
                foreach (var animation in Animations.Values.Where(x => !x.isEndless))
                {
                    // if (animation.currentFrameIndex == animation.framesCount / 2) PerformAttack(attacks[EnemyState.attackA], player);
                    if (animation.isOver())
                    {
                        status = EnemyState.idle;
                        PerformAttack(attacks[EnemyState.attackA], player, bullets, bullet, sounds);
                        animation.currentFrameIndex = 0;
                    }
                }
                if ((dist < attacks[EnemyState.attackA].distance) && (reloadTime == 0))
                {
                    status = EnemyState.attackA;
                    reloadTime = 350;
                }
                if (status != EnemyState.attackA)
                {
                    if (dist < 500)
                    {
                        status = EnemyState.move;
                        double dx = px - x;
                        double dy = py - y;
                        dx = -dx;
                        dy = -dy;

                        int newX = (int)(x + dx * 500 / Math.Sqrt(dx * dx + dy * dy));
                        int newY = (int)(y + dy * 500 / Math.Sqrt(dx * dx + dy * dy));

                        Move(newX, newY, enemies);
                    }
                    else if (dist > attacks[EnemyState.attackA].distance)
                    {
                        status = EnemyState.move;
                        Move(px, py, enemies);
                    }
                    else status = EnemyState.idle;
                }
            }
            else existingTime--;

        }

        public void PerformAttack(Attack attack, Player player, List<Bullet> bullets, Texture2D[] bullet, SoundEffect[] sounds)
        {
            var px = player.CollideBox.x + player.CollideBox.width / 2;
            var py = player.CollideBox.y + player.CollideBox.height / 2;
            var x = IsFlipped ? CollideBox.x + CollideBox.width : CollideBox.x;
            var y = CollideBox.y + CollideBox.height/2;
            sounds[0].Play();
            bullets.Add(new Bullet(x,y,px,py,bullet, attack.damage));
        }
    }
}
