using Microsoft.Xna.Framework.Content;
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
        Sprite zombieSprite = new Sprite();
        

        Game1 game = null;

        
        Vector2 position = Vector2.Zero;
        Vector2 velocity = Vector2.Zero;

        public Player GetPlayer { get; set; }

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

        public Zombie(Game1 game)
        {
            this.game = game;
            Position = new Vector2(100,0);
        }
        
        public void Load(ContentManager content)
        {
            zombie = content.Load<Texture2D>("zombie2");

            zombieAnimation.Load(content, "zombie2", 1, 1);

            zombieAnimation.Origin = new Vector2(zombie.Width / 2, zombie.Height / 2);

            zombieSprite.Add(zombieAnimation, 0, 0);
            zombieSprite.Pause();

            

        }

        public void Update(float deltaTime)
        {
            zombieSprite.Update(deltaTime);
            UpdateInput(deltaTime);
            //CollisionDetection();
            
        }

        private void UpdateInput(float deltaTime)
        {
            /*bool wasMovingLeft = velocity.X < 0;
            bool wasMovingRight = velocity.X > 0;
            bool wasMovingUp = velocity.Y < 0;
            bool wasMovingDown = velocity.Y > 0;*/
            
            Vector2 direction;

            direction = GetPlayer.Position - Position;
            direction.Normalize();

            velocity = direction * (Game1.maxVelocity * deltaTime);

            //velocity.X = MathHelper.Clamp(velocity.X, -Game1.maxVelocity.X, Game1.maxVelocity.X);
           // velocity.Y = MathHelper.Clamp(velocity.Y, -Game1.maxVelocity.Y, Game1.maxVelocity.Y);

            zombieSprite.position += velocity * zombieSpeed * deltaTime;

            /*if ((wasMovingLeft && (velocity.X > 0)) || (wasMovingRight && (velocity.X < 0)))
            {
                velocity.X = 0;
                zombieSprite.Pause();
            }
            if ((wasMovingUp && (velocity.Y > 0)) || (wasMovingDown && (velocity.Y < 0)))
            {
                velocity.Y = 0;
                zombieSprite.Pause();
            }*/


        }

       /* void CollisionDetection()
        {
            //Console.WriteLine("IS RUNNING?");
            // collision detection
            // Our collision detection logic is greatly simplified by the fact that 
            // the player is a rectangle and is exactly the same size as a single tile.
            // So we know that the player can only ever occupy 1, 2 or 4 cells.
            // This means we can short-circuit and avoid building a general purpose 
            // collision detection engine by simply looking at the 1 to 4 cells that 
            // the player occupies:
            int tx = game.PixelToTile(zombieSprite.position.X);
            int ty = game.PixelToTile(zombieSprite.position.Y);
            // nx = true if player overlaps right
            bool nx = (zombieSprite.position.X) % Game1.tile == 0;
            // ny = true if player overlaps below
            bool ny = (zombieSprite.position.Y) % Game1.tile == 0;
            bool cell = game.CellAtTileCoord(tx, ty) == 0;
            bool cellright = game.CellAtTileCoord(tx + 1, ty) == 0;
            bool celldown = game.CellAtTileCoord(tx, ty + 1) == 0;
            bool celldiag = game.CellAtTileCoord(tx + 1, ty + 1) != 0;

            //Console.WriteLine(game.CellAtTileCoord(tx + 1, ty));

            // If the player has vertical velocity, then check to see if they have hit
            // a platform below or above, in which case, stop their vertical velocity, 
            // and clamp their y position:
            if (this.velocity.Y > 0)
            {
                if ((celldown && !cell) || (celldiag && !cellright && nx))
                {
                    // clamp the y position to avoid falling into platform below
                    zombieSprite.position.Y = game.TileToPixel(ty);
                    this.velocity.Y = 0;        // stop downward velocity
                                                //this.isFalling = false;     // no longer falling
                                                //this.isJumping = false;     // (or jumping)
                    ny = false;                 // - no longer overlaps the cells below
                }
            }
            else if (this.velocity.Y < 0)
            {
                if ((cell && !celldown) || (cellright && !celldiag && nx))
                {
                    // clamp the y position to avoid jumping into platform above
                    zombieSprite.position.Y = game.TileToPixel(ty + 1);
                    this.velocity.Y = 0;   // stop upward velocity
                                           // player is no longer really in that cell, we clamped them 
                                           // to the cell below
                    cell = celldown;
                    cellright = celldiag;  // (ditto)
                    ny = false;            // player no longer overlaps the cells below
                }
            }

            if (this.velocity.X > 0)
            {
                if ((cellright && !cell) || (celldiag && !celldown && ny))
                {
                    // clamp the x position to avoid moving into the platform 
                    // we just hit
                    zombieSprite.position.X = game.TileToPixel(tx);
                    this.velocity.X = 0;      // stop horizontal velocity
                    zombieSprite.Pause();
                }
            }
            else if (this.velocity.X < 0)
            {
                if ((cell && !cellright) || (celldown && !celldiag && ny))
                {
                    // clamp the x position to avoid moving into the platform 
                    // we just hit
                    zombieSprite.position.X = game.TileToPixel(tx + 1);
                    this.velocity.X = 0;      // stop horizontal velocity
                    zombieSprite.Pause();
                }
            }
        }*/
        public void Draw(SpriteBatch spriteBatch)
        {          
            zombieSprite.Draw(spriteBatch);
            //spriteBatch.DrawRectangle(zombieSprite.Bounds, Color.White, 1);
        }
    }
}
