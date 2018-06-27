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


        float distance = 75;

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
            
            
        }

        private void UpdateInput(float deltaTime)
        {
            /*bool wasMovingLeft = velocity.X < 0;
            bool wasMovingRight = velocity.X > 0;
            bool wasMovingUp = velocity.Y < 0;
            bool wasMovingDown = velocity.Y > 0;*/
            
            

            //velocity.X = MathHelper.Clamp(velocity.X, -Game1.maxVelocity.X, Game1.maxVelocity.X);
           // velocity.Y = MathHelper.Clamp(velocity.Y, -Game1.maxVelocity.Y, Game1.maxVelocity.Y);

            

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
            //spriteBatch.DrawRectangle(zombieSprite.Bounds, Color.White, 1);
        }
    }
}
