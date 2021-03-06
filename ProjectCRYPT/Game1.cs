﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.ViewportAdapters;

namespace ProjectCRYPT
{

    public class Game1 : Game 
    {
        #region VARIABLES

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player = null;
        
        List<Zombie> zombies = new List<Zombie>();
        List<Skeleton> skeletons = new List<Skeleton>();
        List<Coin> coins = new List<Coin>();
        List<Turret> turrets = new List<Turret>();
        List<Bluefireball> blueFireballs = new List<Bluefireball>();

        Camera2D camera = null;
        TiledMap map = null;
        TiledMapRenderer mapRenderer = null;
        TiledMapTileLayer collisionLayer;

        SpriteFont arial;

        MouseState mouse = Mouse.GetState();    
        Vector2 mousePosition = Vector2.Zero;

        Texture2D splash = null;
        Texture2D healthBar = null;
        Texture2D graveyard = null;
        Texture2D deathText = null;
        Texture2D treasure = null;
        Texture2D treasureText = null;

        SoundEffect zombieDeathSound;
        SoundEffectInstance zombieDeathSoundInstance;

        SoundEffect skeletonDeathSound;
        SoundEffectInstance skeletonDeathSoundInstance;

        SoundEffect turretDeathSound;
        SoundEffectInstance turretDeathSoundInstance;

        SoundEffect playerHurtSound;
        SoundEffectInstance playerHurtSoundInstance;

        SoundEffect playerDeathSound;
        SoundEffectInstance playerDeathSoundInstance;

        SoundEffect coinPickupSound;
        SoundEffectInstance coinPickupSoundInstance;

        Song backgroundMusic;

        Rectangle Endzone = new Rectangle(978, 958, 16, 16);
        bool levelClear = false;

        bool PlayOnce = false;
        float Timer = 3;
        float DamageTimer = 1;

        int health = 15;
        int score = 0;
        int playOnce = 1;

        public static int tile = 16;
        public static float meter = tile;
        public static Vector2 maxVelocity = new Vector2(meter * 7, meter * 7);
        public static float xAcceleration = maxVelocity.X * 5;
        public static float yAcceleration = maxVelocity.Y * 5;
        public static float xFriction = maxVelocity.X * 6;
        public static float yFriction = maxVelocity.Y * 6;

        #endregion

        #region STATES

        const int STATE_SPLASH = 0;
        const int STATE_MENU = 1;
        const int STATE_GAME = 2;
        const int STATE_WIN = 3;
        const int STATE_LOSE = 4;

        int GameState = STATE_SPLASH;

        #endregion

        public int ScreenWidth
        {
            get
            {
                return graphics.GraphicsDevice.Viewport.Width;
            }
        }

        public int ScreenHeight
        {
            get
            {
                return graphics.GraphicsDevice.Viewport.Height;
            }
        }

        public Vector2 MousePos
        {
            get
            {
                return camera.ScreenToWorld(mousePosition);
            }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            GameState = STATE_SPLASH;

            player = new Player(this);
            player.Position = new Vector2(95, 60);         

            base.Initialize();
            this.IsMouseVisible = true;
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player.Load(Content);

            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice,  
                ScreenWidth, ScreenHeight);

            camera = new Camera2D(viewportAdapter);
            camera.Position = new Vector2(-100, -100);

            splash = Content.Load<Texture2D>("splashscreen");
            healthBar = Content.Load<Texture2D>("healthbar");

            map = Content.Load<TiledMap>("dungeon1");
            mapRenderer = new TiledMapRenderer(GraphicsDevice);

            arial = Content.Load<SpriteFont>("Arial");

            graveyard = Content.Load<Texture2D>("graveyard");
            deathText = Content.Load<Texture2D>("deathtext");
            treasure = Content.Load<Texture2D>("treasurepile");
            treasureText = Content.Load<Texture2D>("treasuretext");

            foreach (TiledMapTileLayer layer in map.TileLayers)
            {
                if (layer.Name == "Collisions");
                collisionLayer = layer;
            }
            foreach(TiledMapObjectLayer layer in map.ObjectLayers)
            {
                if(layer.Name == "Zombies")
                {
                    foreach(TiledMapObject obj in layer.Objects)
                    {
                        Zombie zombie = new Zombie(this);
                        zombie.Load(Content);
                        zombie.GetPlayer = player;
                        zombie.Position = new Vector2(obj.Position.X, obj.Position.Y);
                        zombies.Add(zombie);                        
                    }
                }
            }
            foreach (TiledMapObjectLayer layer in map.ObjectLayers)
            {
                if (layer.Name == "Coins")
                {
                    foreach (TiledMapObject obj in layer.Objects)
                    {
                        Coin coin = new Coin(this);
                        coin.Load(Content);
                        coin.Position = new Vector2(obj.Position.X, obj.Position.Y);
                        coins.Add(coin);
                    }
                }
            }
            foreach (TiledMapObjectLayer layer in map.ObjectLayers)
            {
                if (layer.Name == "Skeletons")
                {
                    foreach (TiledMapObject obj in layer.Objects)
                    {
                        Skeleton skeleton = new Skeleton(this);
                        skeleton.Load(Content);
                        skeleton.GetPlayer = player;
                        skeleton.Position = new Vector2(obj.Position.X, obj.Position.Y);
                        skeletons.Add(skeleton);
                    }
                }
            }
            foreach (TiledMapObjectLayer layer in map.ObjectLayers)
            {
                if (layer.Name == "Turrets")
                {
                    foreach (TiledMapObject obj in layer.Objects)
                    {
                        Turret turret = new Turret(this);
                        turret.Load(Content);
                        turret.GetPlayer = player;
                        turret.Position = new Vector2(obj.Position.X, obj.Position.Y);
                        turrets.Add(turret);
                    }
                }
            }



            zombieDeathSound = Content.Load<SoundEffect>("zombiedeath");
            zombieDeathSoundInstance = zombieDeathSound.CreateInstance();

            skeletonDeathSound = Content.Load<SoundEffect>("skeletondeath");
            skeletonDeathSoundInstance = skeletonDeathSound.CreateInstance();

            playerHurtSound = Content.Load<SoundEffect>("playerhurt");
            playerHurtSoundInstance = playerHurtSound.CreateInstance();

            playerDeathSound = Content.Load<SoundEffect>("playerdeath");
            playerDeathSoundInstance = playerDeathSound.CreateInstance();

            turretDeathSound = Content.Load<SoundEffect>("turretDeath");
            turretDeathSoundInstance = turretDeathSound.CreateInstance();

            coinPickupSound = Content.Load<SoundEffect>("coinSound");
            coinPickupSoundInstance = coinPickupSound.CreateInstance();

            backgroundMusic = Content.Load<Song>("Metaruka - The End");

        }


        protected override void UnloadContent()
        {

        }

        private bool IsColliding(Rectangle rect1, Rectangle rect2)
        {
            if (rect1.X + rect1.Width < rect2.X ||
                rect1.X > rect2.X + rect2.Width ||
                rect1.Y + rect1.Height < rect2.Y ||
                rect1.Y > rect2.Y + rect2.Height)
            {
                // these two rectangles are not colliding;
                return false;
            }
            return true;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            player.Update(deltaTime);

            CheckHealth();
            DamageTimer -= deltaTime;

            switch (GameState)
            {
                case STATE_SPLASH:
                    UpdateSplashState(deltaTime);
                    break;
                case STATE_MENU:
                    UpdateMenuState(deltaTime);
                    break;
                case STATE_GAME:
                    UpdateGameState(deltaTime);
                    break;
                case STATE_WIN:
                    UpdateWinState(deltaTime);
                    break;
                case STATE_LOSE:
                    UpdateLoseState(deltaTime);
                    break;
            }

            zombieDeathSoundInstance.Volume = 0.3f;
            skeletonDeathSoundInstance.Volume = 0.3f;
            turretDeathSoundInstance.Volume = 0.3f;
            coinPickupSoundInstance.Volume = 0.2f;
            playerHurtSoundInstance.Volume = 0.2f;

            base.Update(gameTime);
        }

        //GAME STATES

        #region SplashState
        private void UpdateSplashState(float deltaTime)
        {

            Timer -= deltaTime;

            if (Timer <= 0)
            {
                GameState = STATE_MENU;
            }

            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.Volume = 0.1f;
        }

        private void DrawSplashState(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(splash, new Vector2(0, 0), Color.White);
            spriteBatch.End();
        }
        #endregion

        #region MenuState
        private void UpdateMenuState(float deltaTime)
        {

            if (PlayOnce != false)
            {
                PlayOnce = false;
            }

            KeyboardState state = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (state.IsKeyDown(Keys.Space) == true)
            {
                GameState = STATE_GAME;
                if (PlayOnce != true)
                {
                    PlayOnce = true;
                }
            }

        }

        private void DrawMenuState(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(arial, "Press Space to Start", new Vector2( 200, ScreenHeight / 2.5f), Color.White, 0f, new Vector2(0, 0), 3, SpriteEffects.None, 1);

            spriteBatch.End();
        }
        #endregion

        #region GameState
        private void UpdateGameState(float deltaTime)
        {

            PlayOnce = false;

            foreach (Fireball fireball in player.fireballs)
            {

                if (Vector2.Distance(fireball.position, player.playerSprite.position) > 500)
                {
                    fireball.isAlive = false;

                }

                foreach (Zombie zombie in zombies)
                {
                    if (IsColliding(fireball.fireballSprite.Bounds, zombie.zombieSprite.Bounds) == true)
                    {
                        zombies.Remove(zombie);
                        fireball.isAlive = false;
                        zombieDeathSoundInstance.Play();
                        score += 1;
                        break;
                    }
                }

                foreach (Skeleton skeleton in skeletons)
                {
                    if (IsColliding(fireball.fireballSprite.Bounds, skeleton.skeletonSprite.Bounds) == true)
                    {
                        skeletons.Remove(skeleton);
                        fireball.isAlive = false;
                        skeletonDeathSoundInstance.Play();
                        score += 1;
                        break;
                    }
                }

                foreach (Turret turret in turrets)
                {
                    if (IsColliding(fireball.fireballSprite.Bounds, turret.turretSprite.Bounds) == true)
                    {
                        turrets.Remove(turret);
                        turret.isAlive = false;
                        fireball.isAlive = false;
                        turretDeathSoundInstance.Play();
                        score += 1;
                        break;
                    }
                }
            }

            foreach (Coin coin in coins)
            {
                if (IsColliding(coin.coinSprite.Bounds, player.playerSprite.Bounds) == true)
                {
                    coins.Remove(coin);
                    coin.isAlive = false;
                    coinPickupSoundInstance.Play();
                    score += 1;
                    break;
                }
            }

            foreach (Zombie zombie in zombies)
            {
                if (IsColliding(player.Bounds, zombie.Bounds) == true && DamageTimer < 0)
                {
                    playerHurtSoundInstance.Play();
                    DamageTimer = 1;
                    health -= 1;
                    break;
                }
            }

            foreach (Skeleton skeleton in skeletons)
            {
                if (IsColliding(player.Bounds, skeleton.Bounds) == true && DamageTimer < 0) 
                {
                    playerHurtSoundInstance.Play();
                    DamageTimer = 1;
                    health -= 1;
                    break;
                }
            }
               
            foreach (Turret turret in turrets)
            {
                foreach (Bluefireball bluefireball in turret.bluefireballs)
                {
                    if (IsColliding(bluefireball.Bounds, player.Bounds) == true && DamageTimer < 0) 
                    {
                        Console.WriteLine("fireball is colliding with player");
                        blueFireballs.Remove(bluefireball);
                        bluefireball.isAlive = false;
                        playerHurtSoundInstance.Play();
                        DamageTimer = 1;
                        health -= 1;
                        return;
                    }
                }
            }

            foreach (Zombie zombie in zombies)
            {
                zombie.Update(deltaTime);
            }
            foreach (Turret turret in turrets)
            {
                turret.Update(deltaTime);
            }
            foreach (Skeleton skeleton in skeletons)
            {
                skeleton.Update(deltaTime);
            }
            foreach (Coin coin in coins)
            {
                coin.Update(deltaTime);
            }

            camera.Zoom = 2f;

            camera.Position = player.Position - new Vector2(ScreenWidth / 2, ScreenHeight / 2);


            mouse = Mouse.GetState();
            mousePosition = new Vector2(mouse.X, mouse.Y);


            Console.WriteLine(ScreenWidth);
            Console.WriteLine(ScreenHeight);






            if (IsColliding(player.Bounds, Endzone) == true && levelClear == true)
            {
                GameState = STATE_WIN;
            }


        }

        private void DrawGameState(SpriteBatch spriteBatch)
        {

            var viewMatrix = camera.GetViewMatrix();
            var projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0f, -1f);

            spriteBatch.Begin(transformMatrix: viewMatrix, samplerState: SamplerState.PointClamp);

            mapRenderer.Draw(map, ref viewMatrix, ref projectionMatrix);
            player.Draw(spriteBatch);


            foreach (Zombie zombie in zombies)
            {
                zombie.Draw(spriteBatch);
            }
            foreach (Skeleton skeleton in skeletons)
            {
                skeleton.Draw(spriteBatch);
            }
            foreach (Coin coin in coins)
            {
                coin.Draw(spriteBatch);
            }
            foreach (Turret turret in turrets)
            {
                turret.Draw(spriteBatch);
            }

            spriteBatch.DrawRectangle(Endzone, Color.Red, 4f);

            spriteBatch.End();

            spriteBatch.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            for (int i = 0; i < health; i++)
            {
                spriteBatch.Draw(healthBar, new Vector2(20 + healthBar.Width * i, 20), Color.White);
            }

            spriteBatch.DrawString(arial, "Score:", new Vector2(90, ScreenHeight - 40), Color.White, 0f, new Vector2(0, 0), 1, SpriteEffects.None, 1);

            spriteBatch.DrawString(arial, score.ToString(), new Vector2(150, ScreenHeight - 40), Color.White);



            if (IsColliding(player.Bounds, Endzone) == true && levelClear == false)
            {
                spriteBatch.DrawString(arial, "You cannot leave until the level is clear", new Vector2(250, ScreenHeight - 40), Color.White, 0f, new Vector2(0, 0), 1.5f, SpriteEffects.None, 1);         
            }

            if (zombies.Count == 0 && skeletons.Count == 0 && turrets.Count == 0)
            {
                spriteBatch.DrawString(arial, "All Enemies are Dead!! Proceed to the EXIT!", new Vector2(200, 50), Color.White, 0f, new Vector2(0, 0), 3, SpriteEffects.None, 1);
                levelClear = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.P) == true)
            {
                //spriteBatch.DrawString(arial, "All Enemies are Dead!! Proceed to the EXIT!", new Vector2(150, 50), Color.White, 0f, new Vector2(0, 0), 1.5f, SpriteEffects.None, 1);
                //levelClear = true;
                GameState = STATE_WIN;

            }

            if (Keyboard.GetState().IsKeyDown(Keys.O) == true)
            {
                spriteBatch.DrawString(arial, "OOF", new Vector2(ScreenWidth / 2 - 100, ScreenHeight / 2), Color.White, 0f, new Vector2(0, 0), 4, SpriteEffects.None, 1);
                health -= 1;
            }

            spriteBatch.End();

        }
        #endregion

        #region WinState
        private void UpdateWinState(float deltaTime)
        {

        }

        private void DrawWinState(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(treasureText, new Vector2(115, 30), Color.White);        
            spriteBatch.Draw(treasure, new Vector2(275, 150), Color.White);

            spriteBatch.End();
        }
        #endregion

        #region LoseState
        private void UpdateLoseState(float deltaTime)
        {

        }

        private void DrawLoseState(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(graveyard, new Vector2(-100, -100), Color.White);
            spriteBatch.Draw(deathText, new Vector2(200, 75), Color.White); 

            spriteBatch.End();
        }
        #endregion

        private void CheckHealth()
        {
            if (health <= 0 && playOnce == 1)
            {
                playerDeathSoundInstance.Play();
                playOnce = 0;
                GameState = STATE_LOSE;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (GameState)
            {
                case STATE_SPLASH:
                    DrawSplashState(spriteBatch);
                    break;
                case STATE_MENU:
                    DrawMenuState(spriteBatch);
                    break;
                case STATE_GAME:
                    DrawGameState(spriteBatch);
                    break;
                case STATE_WIN:
                    DrawWinState(spriteBatch);
                    break;
                case STATE_LOSE:
                    DrawLoseState(spriteBatch);
                    break;
            }

            base.Draw(gameTime);
        }

        #region Tiled Code
        public int PixelToTile(float pixelCoord)
        {
            return (int)Math.Floor(pixelCoord / tile);
        }

        public int TileToPixel(int tileCoord)
        {
            return tile * tileCoord;
        }

        public int CellAtPixelCoord(Vector2 pixelCoords)
        {
            if (pixelCoords.X < 0 || pixelCoords.X > map.WidthInPixels || pixelCoords.Y < 0)
            {
                return 1;
            }

            if (pixelCoords.Y > map.HeightInPixels)
            {
                return 0;
            } 
            return CellAtTileCoord(PixelToTile(pixelCoords.X), PixelToTile(pixelCoords.Y));

        }

        public int CellAtTileCoord(int tx, int ty)
        {
            if (tx < 0 || tx >= map.Width || ty < 0)
            {
                return 1;
            }
                

            if (ty >= map.Height)
            {
                return 0;
            }
                

            TiledMapTile? tile;
            collisionLayer.TryGetTile(tx, ty, out tile);
            return tile.Value.GlobalIdentifier;

        }

        #endregion
    }
}
