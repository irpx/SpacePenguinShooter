using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Platformer3D
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public static Game1 Instance;

        public Camera camera;
        public Player player;
        public static Physics physics;
        public static Sounds sounds;
        public static Background bg;
        public static Random random;
        public static float bulletTime;
        public static float dTime;
        GraphicsDeviceManager graphics;
        public SpriteBatch overlaySprites;
        public SpriteBatch backgroundSprites;
        SpriteBatch debugSprites;
        public List<String> debugTexts;
        public List<Bullet1> bullets;
        public List<GameObject3D> particles;
        public List<Platform> platforms;
        public List<Enemy> enemies;
        public List<Enemy> deadEnemies;
        SpriteFont debugFont;
        SpriteFont gameFont;
        public static Dictionary<string, Texture2D> textures;
        public static GameObject2D FaderWhite;
        public static GameObject2D backgroundPlate;

        public Game1()
        {
            Instance = this;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            physics = new Physics(new Vector2(0, 200));

            new Shaders();

            random = new Random();
            sounds = new Sounds();

            camera = new Camera();
            camera.position = new Vector3(0.0f, 100.0f, 500.0f);
            
            player = new Player();
            player.position = new Vector3(100.0f, 400.0f, 0.0f);
            
            bg = new Background();
            
            textures = new Dictionary<string, Texture2D>();
            debugTexts = new List<string>();
            bullets = new List<Bullet1>();
            particles = new List<GameObject3D>();
            enemies = new List<Enemy>();
            deadEnemies = new List<Enemy>();
            platforms = new List<Platform>();
            
            platforms.Add(new Platform(new Vector2(200,-520)));
            platforms.Add(new Platform(new Vector2(310, -490)));
            platforms.Add(new Platform(new Vector2(420, -460)));
            platforms.Add(new Platform(new Vector2(530, -430)));
            platforms.Add(new Platform(new Vector2(420, -400)));
            platforms.Add(new Platform(new Vector2(310, -370)));
            platforms.Add(new Platform(new Vector2(200, -340)));

            for (int i = 0; i < 10; i++)
            {
                enemies.Add(new Enemy(new Vector3(150 + random.Next(450), -100, 0)));
            }

            // Vertex declaration for rendering our 3D model.
            graphics.GraphicsDevice.VertexDeclaration = new VertexDeclaration(graphics.GraphicsDevice, VertexPositionNormalTexture.VertexElements);

            graphics.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            graphics.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha; // source rgb * source alpha
            graphics.GraphicsDevice.RenderState.AlphaSourceBlend = Blend.One; // don't modify source alpha
            graphics.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha; // dest rgb * (255 - source alpha)
            graphics.GraphicsDevice.RenderState.AlphaDestinationBlend = Blend.InverseSourceAlpha; // dest alpha * (255 - source alpha)
            graphics.GraphicsDevice.RenderState.BlendFunction = BlendFunction.Add; // add source and dest results

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            backgroundSprites = new SpriteBatch(GraphicsDevice);
            overlaySprites = new SpriteBatch(GraphicsDevice);
            debugSprites = new SpriteBatch(GraphicsDevice);

            debugFont = Content.Load<SpriteFont>(@"Fonts/diagnosticFont");
            gameFont = Content.Load<SpriteFont>(@"Fonts/gameFont");

            textures.Add("white", Content.Load<Texture2D>(@"Textures/white"));
            textures.Add("space", Content.Load<Texture2D>(@"Textures/space_bg"));

            backgroundPlate = new GameObject2D(new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2), "space");
            backgroundPlate.alpha = 1;
            backgroundPlate.scale = 3;

            FaderWhite = new GameObject2D(new Vector2(GraphicsDevice.Viewport.Width/2,GraphicsDevice.Viewport.Height/2),"white");
            FaderWhite.alpha = 0.0f;

            Sounds.LoadSounds();

            Sounds.PlayMusic("music2");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Input.Update();

            if (debugTexts.Count > 0) debugTexts.Clear();

            if (Input.pad.IsButtonDown(Buttons.Back) || Input.key.IsKeyDown(Keys.Escape))
            {
                Sounds.PlaySound("pickup");
                this.Exit();
            }

            if (Input.keyPressedDown(Keys.F))
            {
                Sounds.PlaySound("blip");
                graphics.ToggleFullScreen();
            }

            bulletTime = Input.pad.Triggers.Left;
            bulletTime = 1f - bulletTime;

            dTime = gameTime.ElapsedGameTime.Milliseconds * bulletTime;
            
            physics.Update(dTime*0.001f);
            player.Update(dTime);
            camera.Update(dTime);
            UpdateBullets(dTime);
            UpdateEnemies(dTime);
            UpdateParticles(dTime);

            backgroundPlate.position.X = 400 + (-Input.pad.ThumbSticks.Right.X * 200);
            backgroundPlate.position.Y = 300 + (Input.pad.ThumbSticks.Right.Y * 100);

            FaderWhite.Update(dTime);

            base.Update(gameTime);
        }

        private void UpdateEnemies(float dTime)
        {
            if (enemies.Count < 10 && random.Next(100) > 97)
            {
                if (deadEnemies.Count > 0)
                {
                    Enemy enm = deadEnemies.First();
                    if (enm == null) throw new Exception("Enemies messed up!");
                    deadEnemies.Remove(enm);
                    enm.Reset(new Vector3(250 + random.Next(450), -100, 0));
                    enemies.Add(enm);
                }
                else
                {
                    throw new Exception("NO ENEMIES!");
                }
            }

            Enemy[] removes = new Enemy[enemies.Count];
            foreach (Enemy enm in enemies)
            {
                if (!enm.Update(dTime))
                {
                    removes[enemies.IndexOf(enm)] = enm;
                }
            }

            foreach (Enemy enemy in removes)
            {
                if (enemy != null)
                {
                    enemy.Destroy();
                    deadEnemies.Add(enemy);
                }
            }
        }

        private void UpdateBullets(float dTime)
        {
            Bullet1[] removes = new Bullet1[bullets.Count];
            foreach (Bullet1 bullet in bullets)
            {
                if (!bullet.Update(dTime))
                {
                    removes[bullets.IndexOf(bullet)] = bullet;
                }
            }

            foreach (Bullet1 bullet in removes)
            {
                if (bullet != null) bullet.Destroy();
            }
        }
        private void UpdateParticles(float dTime)
        {
            GameObject3D[] removes = new GameObject3D[particles.Count];
            foreach (GameObject3D particle in particles)
            {
                if (!particle.Update(dTime))
                {
                    removes[particles.IndexOf(particle)] = particle;
                }
            }

            foreach (GameObject3D particle in removes)
            {
                if (particle != null) particle.Destroy();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            backgroundSprites.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.SaveState);
            backgroundPlate.Draw(backgroundSprites);
            backgroundSprites.End();
            
            bg.Draw(dTime);
            DrawPlatforms(dTime);

            player.Draw(dTime);
            DrawEnemies(dTime);
            
            DrawBullets(dTime);

            DrawParticles(dTime);

            overlaySprites.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.SaveState);
            FaderWhite.Draw(overlaySprites);

            overlaySprites.DrawString(gameFont, "Player1 Health:"+player.health, new Vector2(10, 10), Color.White);
            overlaySprites.DrawString(gameFont, "Enemies:" + enemies.Count, new Vector2(10, 50), Color.White);

            overlaySprites.End();

            if (Input.pad.IsButtonDown(Buttons.LeftStick) || Input.pad.IsButtonDown(Buttons.RightStick))
            {
                debugSprites.Begin(SpriteBlendMode.AlphaBlend,SpriteSortMode.BackToFront,SaveStateMode.SaveState);
                if (Input.pad.IsButtonDown(Buttons.LeftStick)) drawDebug();
                if (Input.pad.IsButtonDown(Buttons.RightStick)) drawPhysDebug();
                debugSprites.End();
            }
            base.Draw(gameTime);
        }

        private void DrawPlatforms(float dTime)
        {
            foreach (Platform plat in platforms)
            {
                plat.Draw(dTime);
            }
        }

        private void DrawEnemies(float dTime)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(dTime);
            }
        }
        private void DrawParticles(float dTime)
        {
            foreach (GameObject3D particle in particles)
            {
                particle.Draw(dTime);
            }
        }

        private void DrawBullets(float dTime)
        {
            foreach (Bullet1 bullet in bullets)
            {
                bullet.Draw(dTime);
            }
        }

        private void drawPhysDebug()
        {
            physics.Draw(debugSprites);

        }
        private void drawDebug()
        {
            int textPos = 5;
            foreach (String str in debugTexts)
            {
                debugSprites.DrawString(debugFont, str, new Vector2(10, textPos), Color.White);
                textPos += (int)debugFont.MeasureString(str).Y + 5;
            }
        }
    }
}
