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
            

            foreach(Fireball fireball in player.fireballs)
            {
                foreach(Zombie zombie in zombies)
                {
                    if (IsColliding(fireball.fireballSprite.Bounds, zombie.zombieSprite.Bounds) == true)
                    {
                        zombies.Remove(zombie);
                        break;
                    }
                }

                foreach(Skeleton skeletons in skeletons)
                {

                }
            }

            foreach(Zombie zombie in zombies)
            {
                zombie.Update(deltaTime);
                
            }

            foreach(Skeleton skeleton in skeletons)
            {
                skeleton.Update(deltaTime);
            }
            foreach(Coin coin in coins)
            {
                coin.Update(deltaTime);
            }

            camera.Zoom = 3f;

            camera.Position = player.Position - new Vector2(ScreenWidth / 2, ScreenHeight / 2);


            mouse = Mouse.GetState();
            mousePosition = new Vector2(mouse.X, mouse.Y);

            Console.WriteLine(mousePosition);

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

        }

        private void DrawSplashState(float deltaTime)
        {

        }
        #endregion

        #region MenuState
        private void UpdateMenuState(float deltaTime)
        {

        }

        private void DrawMenuState(float deltaTime)
        {

        }
        #endregion

        #region GameState
        private void UpdateGameState(float deltaTime)
        {

        }
  
        private void DrawGameState(float deltaTime)
        {

        }
        #endregion

        #region WinState
        private void UpdateWinState(float deltaTime)
        {

        }

        private void DrawWinState(float deltaTime)
        {

        }
        #endregion

        #region LoseState
        private void UpdateLoseState(float deltaTime)
        {

        }

        private void DrawLoseState(float deltaTime)
        {

        }
        #endregion


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var viewMatrix = camera.GetViewMatrix();
            var projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0f, -1f);

            spriteBatch.Begin(transformMatrix : viewMatrix, samplerState : SamplerState.PointClamp);

            mapRenderer.Draw(map, ref viewMatrix, ref projectionMatrix);
            player.Draw(spriteBatch);


            foreach (Zombie zombie in zombies)
            {
                zombie.Draw(spriteBatch);
            }
            foreach(Skeleton skeleton in skeletons)
            {
                skeleton.Draw(spriteBatch);
            }
            foreach(Coin coin in coins)
            {
                coin.Draw(spriteBatch);
            }

            switch (GameState)
            {
                case STATE_SPLASH:
                    DrawSplashState(spriteBatch);
                    break;


            }

            spriteBatch.End();

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
