using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.ViewportAdapters;


namespace ProjectCRYPT
{

    public class Game1 : Game 
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player = null;
        Zombie zombie = null;

        Camera2D camera = null;
        TiledMap map = null;
        TiledMapRenderer mapRenderer = null;
        TiledMapTileLayer collisionLayer;

        SpriteFont arial;





        public static int tile = 16;
        public static float meter = tile;
        public static Vector2 maxVelocity = new Vector2(meter * 7, meter * 7);
        public static float xAcceleration = maxVelocity.X * 5;
        public static float yAcceleration = maxVelocity.Y * 5;
        public static float xFriction = maxVelocity.X * 6;
        public static float yFriction = maxVelocity.Y * 6;


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


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


        }

        protected override void Initialize()
        {
            zombie = new Zombie(this);
            zombie.Position = new Vector2(100, 0);

            player = new Player(this);
            player.Position = new Vector2(0, 0);

            base.Initialize();
            this.IsMouseVisible = true;
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player.Load(Content);
            zombie.Load(Content);
            zombie.GetPlayer = player;

            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice,  
                ScreenWidth, ScreenHeight);

            camera = new Camera2D(viewportAdapter);
            camera.Position = new Vector2(-100, -100);

            map = Content.Load<TiledMap>("cryptoverworld");
            mapRenderer = new TiledMapRenderer(GraphicsDevice);

            arial = Content.Load<SpriteFont>("Arial");

            foreach (TiledMapTileLayer layer in map.TileLayers)
            {
                if (layer.Name == "Collisions");
                collisionLayer = layer;
            }

        }

        protected override void UnloadContent()
        {

        }

        

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            player.Update(deltaTime);
            zombie.Update(deltaTime);

            MouseState mouse = Mouse.GetState();

            int mouseX = mouse.X;
            int mouseY = mouse.Y;

            Vector2 mousePosition = new Vector2(mouse.X, mouse.Y);

            //camera.Move(new Vector2(0, -50) * deltaTime);
            camera.Zoom = 4f;

            camera.Position = player.Position - new Vector2(ScreenWidth / 2, ScreenHeight / 2);

            Console.WriteLine(mousePosition);

            
            base.Update(gameTime);
        }
        

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var viewMatrix = camera.GetViewMatrix();
            var projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0f, -1f);

            spriteBatch.Begin(transformMatrix : viewMatrix, samplerState : SamplerState.PointClamp);

            mapRenderer.Draw(map, ref viewMatrix, ref projectionMatrix);
            player.Draw(spriteBatch);
            zombie.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

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
                return 1;

            if (pixelCoords.Y > map.HeightInPixels)
                return 0;
            return CellAtTileCoord(PixelToTile(pixelCoords.X), PixelToTile(pixelCoords.Y));

        }

        public int CellAtTileCoord(int tx, int ty)
        {
            if (tx < 0 || tx >= map.Width || ty < 0)
                return 1;

            if (ty >= map.Height)
                return 0;

            TiledMapTile? tile;
            collisionLayer.TryGetTile(tx, ty, out tile);
            return tile.Value.GlobalIdentifier;

        }
    }
}
