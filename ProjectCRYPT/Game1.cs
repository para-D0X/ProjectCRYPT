using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
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

        Camera2D camera = null;
        TiledMap map = null;
        TiledMapRenderer mapRenderer = null;
        TiledMapTileLayer collisionLayer;

        SpriteFont arial;

        MouseState mouse = Mouse.GetState();    
        Vector2 mousePosition = Vector2.Zero;

        Texture2D splash = null;
        Texture2D healthBar = null;

        bool PlayOnce = false;
        float Timer = 3;
        float DamageTimer = 1;

        int health = 15;

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

            spriteBatch.DrawString(arial, "Press Space to Start", new Vector2(ScreenWidth / 2, ScreenHeight / 2), Color.White);

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
                        break;
                    }
                }

                foreach (Skeleton skeleton in skeletons)
                {
                    if (IsColliding(fireball.fireballSprite.Bounds, skeleton.skeletonSprite.Bounds) == true)
                    {
                        skeletons.Remove(skeleton);
                        fireball.isAlive = false;
                        break;
                    }
                }
            }

            foreach (Zombie zombie in zombies)
            {
                zombie.Update(deltaTime);

            }

            foreach (Skeleton skeleton in skeletons)
            {
                skeleton.Update(deltaTime);
            }
            foreach (Coin coin in coins)
            {
                coin.Update(deltaTime);
            }

            camera.Zoom = 3f;

            camera.Position = player.Position - new Vector2(ScreenWidth / 2, ScreenHeight / 2);


            mouse = Mouse.GetState();
            mousePosition = new Vector2(mouse.X, mouse.Y);

            //Console.WriteLine(mousePosition);

            Console.WriteLine(ScreenWidth);
            Console.WriteLine(ScreenHeight);

            if(zombies.Count == 0 && skeletons.Count == 0)
            {
                //print "all enemies are dead, proceed to the exit"
            }

            foreach (Zombie zombie in zombies)
            {
                if (IsColliding(player.Bounds, zombie.Bounds) == true && DamageTimer < 0)
                {
                                      
                    //playerhurt.Play();
                    DamageTimer = 1;
                    health -= 1;
                    break;                                     
                }
            }
            /*if (IsColliding(player.Bounds, Endzone) == true)
            {
                GameState = STATE_WIN;
            }*/

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

            for (int i = 0; i < health; i++)
            {
                spriteBatch.Draw(healthBar, new Vector2(80 - i * 15, 20), Color.White);
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

        }
        #endregion

        #region LoseState
        private void UpdateLoseState(float deltaTime)
        {

        }

        private void DrawLoseState(SpriteBatch spriteBatch)
        {

        }
        #endregion

        private void CheckHealth()
        {
            if (health <= 0)
            {
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
