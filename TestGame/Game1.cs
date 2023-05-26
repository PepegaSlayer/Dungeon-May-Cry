using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography.X509Certificates;

namespace TestGame
{
    public class Game1 : Game
    {
        private bool start = false; 
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        List<Texture2D> temp = new List<Texture2D>();

        List<Texture2D> EnemyAMove = new List<Texture2D>();
        List<Texture2D> EnemyAattackA = new List<Texture2D>();
        List<Texture2D> EnemyAdies = new List<Texture2D>();

        List<Texture2D> EnemyBMove = new List<Texture2D>();
        List<Texture2D> EnemyBattackA = new List<Texture2D>();
        List<Texture2D> EnemyBidle = new List<Texture2D>();
        List<Texture2D> EnemyBdies = new List<Texture2D>();

        List<Bullet> bullets = new List<Bullet>();
        List<Texture2D> bulletTexture = new List<Texture2D>();

        List<Blood> blood = new List<Blood>();
        List<Texture2D> bloodframes = new List<Texture2D>();

        private Texture2D Controls;

        public SoundEffect[] Blades;
        public SoundEffect[] EnemySounds;
        public Song Soundtrack;
        public SoundEffect BloodSplash;

     

        private Player player;
        private ValueBar healthBar;
        private ValueBar killsBar;
        private Map map;
        public Dictionary<EnemyState, Attack> EnemyAAttacks = new Dictionary<EnemyState, Attack>()
        {
            { EnemyState.attackA, new Attack("slash", 30, 200) }
        };

        public Dictionary<EnemyState, Attack> EnemyBAttacks = new Dictionary<EnemyState, Attack>()
        {
            { EnemyState.attackA, new Attack("shoot", 15, 600) }
        };
        private List<Enemy> enemies = new List<Enemy>();


        public SpriteFont arial;

        public KeyboardState previousKeyboardState;
        private Random rand = new Random();
        private double spawnTime = 0;
        public static int ScreenWidth = 1000;
        public static int ScreenHeight = 1000;

        public Camera camera = new Camera();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.ApplyChanges();

            _graphics.GraphicsDevice.Clear(Color.Black);


            base.Initialize();
        }

        protected override void LoadContent()
        {
          
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            for (var i = 1; i < 5; i++) temp.Add(Content.Load<Texture2D>($"TileSet/{i}"));
            map = new Map(temp.ToArray());
            temp.Clear();

            player = new Player(500, 500, 5, 100, map.mapSize, map.TileSize);

            healthBar = new ValueBar(Content.Load<Texture2D>("UI/Health Bar/hpBar"), Content.Load<Texture2D>("UI/Health Bar/hpBar_bg"), player.Health, new(10,10));
            killsBar = new ValueBar(Content.Load<Texture2D>("UI/Health Bar/killsbar"), Content.Load<Texture2D>("UI/Health Bar/hpBar_bg"), 30, new(50, 10));

            for (var i = 1; i < 9; i++) temp.Add(Content.Load<Texture2D>($"Player/run/{i}"));
            player.Animations.Add(Status.run, new AnimatedSprite(temp.ToArray(), 0.1f, true, new(-15, -20)));
            temp.Clear();
            for (var i = 1; i < 5; i++) temp.Add(Content.Load<Texture2D>($"Player/idle/{i}"));
            player.Animations.Add(Status.idle, new AnimatedSprite(temp.ToArray(), 0.2f, true, new(-15,-20)));
            temp.Clear();
            for (var i = 1; i < 4; i++) temp.Add(Content.Load<Texture2D>($"Player/SplashA/{i}"));
            player.Animations.Add(Status.slash, new AnimatedSprite(temp.ToArray(), 0.08f, false, new(-15, -20)));
            temp.Clear();
            for (var i = 5; i < 8; i++) temp.Add(Content.Load<Texture2D>($"Player/SplashA/{i}"));
            player.Animations.Add(Status.slashWait, new AnimatedSprite(temp.ToArray(), 0.1f, false, new(-15, -20)));
            temp.Clear();
            for (var i = 1; i < 6; i++) temp.Add(Content.Load<Texture2D>($"Player/SplashB/{i}"));
            player.Animations.Add(Status.doubleSlash, new AnimatedSprite(temp.ToArray(), 0.08f, false, new(-15, -20)));
            temp.Clear();
            for (var i = 1; i < 8; i++) temp.Add(Content.Load<Texture2D>($"Player/die/{i}"));
            player.Animations.Add(Status.die, new AnimatedSprite(temp.ToArray(), 0.08f, false, Vector2.Zero));
            temp.Clear();
            for (var i = 1; i < 3; i++) temp.Add(Content.Load<Texture2D>($"Player/die/{i}"));
            player.Animations.Add(Status.stun, new AnimatedSprite(temp.ToArray(), 0.08f, true, Vector2.Zero));
            temp.Clear();
            for (var i = 1; i < 5; i++) temp.Add(Content.Load<Texture2D>($"Player/dash/{i}"));
            player.Animations.Add(Status.dash, new AnimatedSprite(temp.ToArray(), 0.07f, false, Vector2.Zero));
            temp.Clear();
            for (var i = 1; i < 4; i++) temp.Add(Content.Load<Texture2D>($"Player/slam/{i}"));
            player.Animations.Add(Status.slam, new AnimatedSprite(temp.ToArray(), 0.08f, false,new(-130, -75)));
            temp.Clear();
            for (var i = 4; i < 6; i++) temp.Add(Content.Load<Texture2D>($"Player/slam/{i}"));
            player.Animations.Add(Status.slamWait, new AnimatedSprite(temp.ToArray(), 0.1f, false, new(-130, -75)));
            temp.Clear();
            for (var i = 1; i < 7; i++) temp.Add(Content.Load<Texture2D>($"Player/spin/{i}"));
            player.Animations.Add(Status.spin, new AnimatedSprite(temp.ToArray(), 0.08f, false, new(-130, -75)));
            temp.Clear();

            for (var i = 1; i < 8; i++) EnemyAMove.Add(Content.Load<Texture2D>($"EnemyA/move/{i}"));
            for (var i = 1; i < 11; i++) EnemyAattackA.Add(Content.Load<Texture2D>($"EnemyA/AttackA/{i}"));
            for (var i = 1; i < 9; i++) EnemyAdies.Add(Content.Load<Texture2D>($"EnemyA/die/{i}"));

            for (var i = 1; i < 9; i++) EnemyBMove.Add(Content.Load<Texture2D>($"EnemyB/move/{i}"));
            for (var i = 1; i < 12; i++) EnemyBattackA.Add(Content.Load<Texture2D>($"EnemyB/AttackA/{i}"));
            for (var i = 1; i < 8; i++) EnemyBidle.Add(Content.Load<Texture2D>($"EnemyB/Idle/{i}"));
            for (var i = 1; i < 6; i++) EnemyBdies.Add(Content.Load<Texture2D>($"EnemyB/die/{i}"));

            for (var i = 1; i < 5; i++) bulletTexture.Add(Content.Load<Texture2D>($"bullet/{i}"));
            
            for (var i = 1; i < 8; i++) bloodframes.Add(Content.Load<Texture2D>($"Blood FX/1/{i}"));

            Controls = Content.Load<Texture2D>("UI/controlls");

            Blades = new SoundEffect[] {
               Content.Load<SoundEffect>("sounds/blade1"),
                Content.Load<SoundEffect>("sounds/blade")
                };
            EnemySounds = new SoundEffect[]
            {
                 Content.Load<SoundEffect>("sounds/blast"),
                  Content.Load<SoundEffect>("sounds/AxeSmash")
            };
            BloodSplash = Content.Load<SoundEffect>("sounds/BloodSplash");
            Soundtrack = Content.Load<Song>("The Darkest Nights");
            MediaPlayer.Play(Soundtrack);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.1f;
           

        }

        protected override void Update(GameTime gameTime)
        {
            camera.follow(player.CollideBox, map);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.GetPressedKeys().Length != 0) start = true;
            if (start)
            {
                // обновление игрока

                player.Update(keyboardState, previousKeyboardState, enemies, blood, bloodframes.ToArray(), Blades, BloodSplash);
                previousKeyboardState = keyboardState;

                if (player.status == Status.idle || player.status == Status.run)
                    player.Move(keyboardState);
                if (player.Animations[Status.die].currentFrameIndex != player.Animations[Status.die].framesCount - 1)
                    player.Animations[player.status].Update(gameTime);

                // обновление противников

                for (var i = 0; i < enemies.Count; i++)
                {
                    enemies[i].Update(player, bullets, bulletTexture.ToArray(), EnemySounds, enemies.Where(x => x != enemies[i]).ToArray());
                    if (enemies[i].Animations[EnemyState.die].currentFrameIndex != enemies[i].Animations[EnemyState.die].framesCount - 1)
                        enemies[i].Animations[enemies[i].status].Update(gameTime);
                    if ((enemies[i].Health <= 0) && (enemies[i].status != EnemyState.die))
                    {
                        enemies[i].status = EnemyState.die;
                        player.kills += player.kills < 30 ? 1 : 0;
                    }
                    if (enemies[i].existingTime == 0) enemies.RemoveAt(i);

                }

                // Обновление снарядов колдуна и крови

                foreach (var bl in blood) bl.sprite.Update(gameTime);

                for (var i = 0; i < bullets.Count(); i++)
                {
                    bullets[i].update(player, gameTime);
                    if (bullets[i].box.Intersects(player.CollideBox) && (player.status != Status.dash))
                    {
                        player.Health -= bullets[i].damage;
                        player.status = Status.stun;
                        player.stanTime = 60;
                        bullets.RemoveAt(i);
                    }
                }

                // Спавн противников

                spawnTime -= 0.5;

                if (spawnTime <= 0)
                {
                    for (var i = 0; i < rand.Next(1, 2); i++)
                    {
                        enemies.Add(new EnemySlamer(rand.Next(-50, map.mapSize.X + 50), -50, 2, 200, new Dictionary<EnemyState, AnimatedSprite>() {
                        { EnemyState.move, new AnimatedSprite(EnemyAMove.ToArray(), 0.15f, true, new Vector2(-120,-20)) },
                        { EnemyState.attackA, new AnimatedSprite(EnemyAattackA.ToArray(),0.15f, false, new Vector2(-120,-78)) },
                        { EnemyState.die, new AnimatedSprite(EnemyAdies.ToArray(),0.05f, false, new Vector2(-120,-20)) }
                    }, EnemyAAttacks, 80, 120));
                        enemies.Add(new EnemyWizzard(rand.Next(-50, map.mapSize.X + 50), -50, 2, 100, new Dictionary<EnemyState, AnimatedSprite>() {
                        { EnemyState.move, new AnimatedSprite(EnemyBMove.ToArray(), 0.10f, true, new Vector2(0,-40)) },
                        { EnemyState.attackA, new AnimatedSprite(EnemyBattackA.ToArray(), 0.10f, false, new Vector2(-30,-30)) },
                        { EnemyState.idle, new AnimatedSprite(EnemyBidle.ToArray(), 0.20f, true, new Vector2(0,-25)) },
                        { EnemyState.die, new AnimatedSprite(EnemyBdies.ToArray(), 0.10f, false, new Vector2(-18,-40)) }
                    }, EnemyBAttacks, 70, 140));
                    enemies.Add(new EnemySlamer(rand.Next(-50, map.mapSize.X + 50), map.mapSize.Y + 50, 2, 200, new Dictionary<EnemyState, AnimatedSprite>() {
                        { EnemyState.move, new AnimatedSprite(EnemyAMove.ToArray(), 0.15f, true, new Vector2(-120,-20)) },
                        { EnemyState.attackA, new AnimatedSprite(EnemyAattackA.ToArray(),0.15f, false, new Vector2(-120,-78)) },
                        { EnemyState.die, new AnimatedSprite(EnemyAdies.ToArray(),0.05f, false, new Vector2(-120,-20)) }
                    }, EnemyAAttacks, 80, 120));
                    enemies.Add(new EnemyWizzard(rand.Next(-50, map.mapSize.X + 50), map.mapSize.Y + 50,2, 100, new Dictionary<EnemyState, AnimatedSprite>() {
                        { EnemyState.move, new AnimatedSprite(EnemyBMove.ToArray(), 0.10f, true, new Vector2(0,-40)) },
                        { EnemyState.attackA, new AnimatedSprite(EnemyBattackA.ToArray(), 0.10f, false, new Vector2(-30,-30)) },
                        { EnemyState.idle, new AnimatedSprite(EnemyBidle.ToArray(), 0.20f, true, new Vector2(0,-25)) },
                        { EnemyState.die, new AnimatedSprite(EnemyBdies.ToArray(), 0.10f, false, new Vector2(-18,-40)) }
                    }, EnemyBAttacks, 70, 140));
                    }
                spawnTime = 700;
                }

            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            _spriteBatch.Begin(transformMatrix: camera.Transform);
            map.Draw(_spriteBatch);
            foreach (Enemy enemy in enemies.OrderBy(x => x.CollideBox.y))   
                enemy.Animations[enemy.status].Draw(_spriteBatch, enemy.IsFlipped, enemy.CollideBox);
            player.Animations[player.status].Draw(_spriteBatch, player.isFlipped, player.CollideBox);
            
            foreach (var bullet in bullets) bullet.draw(_spriteBatch);
            foreach (var bl in blood) bl.draw(_spriteBatch);
            _spriteBatch.End();

            _spriteBatch.Begin();
            healthBar.Draw(_spriteBatch, player.Health);
            killsBar.Draw(_spriteBatch, player.kills);
            if (!start) 
                _spriteBatch.Draw(Controls, new Vector2(100, 100), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}