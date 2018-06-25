﻿using Microsoft.Xna.Framework;
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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player = null;
        //Zombie zombie = null;

        List<Zombie> zombies = new List<Zombie>();

        Camera2D camera = null;
        TiledMap map = null;
        TiledMapRenderer mapRenderer = null;
        TiledMapTileLayer collisionLayer;

        SpriteFont arial;

        MouseState mouse = Mouse.GetState();

        Vector2 mousePosition = Vector2.Zero;

        float cameraZoomAmount = 3f;




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
            //zombie = new Zombie(this);
            //zombie.Position = new Vector2(100, 0);

            player = new Player(this);
            player.Position = new Vector2(0, 0);

            base.Initialize();
            this.IsMouseVisible = true;

        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player.Load(Content);
            //zombie.Load(Content);
            //zombie.GetPlayer = player;

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
            

            foreach(Zombie zombie in zombies)
            {
                zombie.Update(deltaTime);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up) == true)
            {
                cameraZoomAmount += 1f * deltaTime;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down) == true)
            {
                cameraZoomAmount -= 1f * deltaTime;
            }

            camera.Zoom = 3f;
            camera.Zoom = cameraZoomAmount;

            camera.Position = player.Position - new Vector2(ScreenWidth / 2, ScreenHeight / 2);


            mouse = Mouse.GetState();
            mousePosition = new Vector2(mouse.X, mouse.Y);

            Console.WriteLine(mousePosition);

            base.Update(gameTime);
        }      




        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var viewMatrix = camera.GetViewMatrix();
            var projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0f, -1f);

            spriteBatch.Begin(transformMatrix : viewMatrix, samplerState : SamplerState.PointClamp);

            mapRenderer.Draw(map, ref viewMatrix, ref projectionMatrix);
            player.Draw(spriteBatch);
            //spriteBatch.DrawRectangle(player.Bounds, Color.Red, 1);


            foreach (Zombie zombie in zombies)
            {
                zombie.Draw(spriteBatch);
            }

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
        /*private void CheckCollsions()
        {
            foreach(Zombie zombie in zombies)
            {
                if(IsColliding(player.Bounds, zombie.Bounds) == true)
                {
                    
                }
            }
        }

        public bool IsColliding(Rectangle rect1, Rectangle rect2)
        {
            if (rect1.X + rect1.Width < rect2.X ||
                rect1.X > rect2.X + rect2.Width ||
                rect1.Y + rect1.Height < rect2.Y ||
                rect1.Y > rect2.Y + rect2.Height)
            {
                // these two rectangles are not colliding
                return false;
            }
            else
            {
                // else, the two AABB rectangles overlap, therefore collision
                return true;
            }
        }*/
    }
}
