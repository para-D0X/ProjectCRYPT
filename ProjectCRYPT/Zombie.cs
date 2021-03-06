﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCRYPT
{
    class Zombie 
    {
        GraphicsDeviceManager graphics;
        public Sprite zombieSprite = new Sprite();
        

        Game1 game = null;


        float distance = 75;

        Vector2 position = Vector2.Zero;
        Vector2 velocity = Vector2.Zero;

        public Player GetPlayer { get; set; }
        public Fireball GetFireball { get; set; }
        public bool isAlive;
        public float zombieRotation = 0f;
        float zombieSpeed = 25f;
        
        Texture2D zombie;

        AnimatedTexture zombieAnimation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);

        public Rectangle Bounds
        {
            get { return zombieSprite.Bounds; }
        }

        public Vector2 Position
        {
            get
            {
                return zombieSprite.position;
            }
            set
            {
                zombieSprite.position = value;
            }
        }

        float RotateTo(Vector2 pointTo)
        {
            float rot = 0;
            
            Vector2 direction = position - pointTo;
            direction.Normalize();

            rot = (float)Math.Atan2((double)direction.Y,(double)direction.X);

            rot += MathHelper.ToRadians(180);

            return rot;
        }

        public Zombie(Game1 game)
        {
            this.game = game;
            Position = new Vector2(100,0);
            zombieRotation = 0;
        }
        
        public void Load(ContentManager content)
        {
            zombie = content.Load<Texture2D>("zombie");

            zombieAnimation.Load(content, "zombie", 1, 1);

            zombieAnimation.Origin = new Vector2(zombie.Width / 2, zombie.Height / 2);

            zombieSprite.Add(zombieAnimation, zombie.Width / 2, zombie.Height / 2);
            zombieSprite.Pause();

            

        }

        public void Update(float deltaTime)
        {
            zombieSprite.Update(deltaTime);
            UpdateInput(deltaTime);
            zombieRotation = RotateTo(GetPlayer.Position);
            zombieAnimation.Rotation = zombieRotation;
            position = zombieSprite.position;
        }

        private void UpdateInput(float deltaTime)
        {
            int tx = game.PixelToTile(zombieSprite.position.X);
            int ty = game.PixelToTile(zombieSprite.position.Y);

            bool nx = (zombieSprite.position.X) % Game1.tile != 0;

            bool ny = (zombieSprite.position.Y) % Game1.tile != 0;

            bool cell = game.CellAtTileCoord(tx, ty) != 0;
            bool cellright = game.CellAtTileCoord(tx + 1, ty) != 0;
            bool celldown = game.CellAtTileCoord(tx, ty + 1) != 0;
            bool celldiag = game.CellAtTileCoord(tx + 1, ty + 1) != 0;

            Vector2.Distance(GetPlayer.Position, zombieSprite.position);

            if(Vector2.Distance(GetPlayer.Position, zombieSprite.position) <= distance)
            {
                Vector2 direction;

                direction = GetPlayer.Position - Position;
                direction.Normalize();

                velocity = direction * (Game1.maxVelocity * deltaTime);

                zombieSprite.position += velocity * zombieSpeed * deltaTime;
            }
                      
            if (this.velocity.Y > 0)
            {
                if ((celldown && !cell) || (celldiag && !cellright && nx))
                {
                    zombieSprite.position.Y = game.TileToPixel(ty);
                    this.velocity.Y = 0;
                    ny = false;
                }
            }

            else if (this.velocity.Y < 0)
            {
                if ((cell && !celldown) || (cellright && !celldiag && nx))
                {
                    zombieSprite.position.Y = game.TileToPixel(ty + 1);
                    this.velocity.Y = 0;
                    cell = celldown;
                    cellright = celldiag;
                    ny = false;
                }
            }

            if (this.velocity.X > 0)
            {
                if ((cellright && !cell) || (celldiag && !celldown && ny))
                {
                    zombieSprite.position.X = game.TileToPixel(tx);
                    this.velocity.X = 0;
                    zombieSprite.Pause();
                }
            }
            else if (this.velocity.X < 0)
            {
                if ((cell && !cellright) || (celldown && !celldiag && ny))
                {
                    zombieSprite.position.X = game.TileToPixel(tx + 1);
                    this.velocity.X = 0;
                    zombieSprite.Pause();
                }
            }                   

        }
        
        
        public void Draw(SpriteBatch spriteBatch)
        {          
            zombieSprite.Draw(spriteBatch);
            zombieAnimation.Rotation = zombieRotation;
            //spriteBatch.DrawRectangle(zombieSprite.Bounds, Color.White, 1);
        }
                
    }
}
