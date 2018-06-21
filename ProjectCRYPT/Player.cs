using System;
using MonoGame.Extended;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ParticleEffects;

namespace ProjectCRYPT
{
    class Player
    {
        Game1 game = null;

        Vector2 velocity = Vector2.Zero;
        Vector2 position = Vector2.Zero;
        float rotation = 0f;

        Texture2D playerTexture = null;
        Texture2D crosshairTexture = null;
        Texture2D fireballTexture = null;


        List<Fireball> fireballs = new List<Fireball>();

        Sprite playerSprite = new Sprite();
        Sprite crosshair = new Sprite();
        
       /* Emitter dustEmitter = null;
        Texture2D dustParticle = null;
        Vector2 emitterOffset = new Vector2(8, 8); */


        AnimatedTexture playerAnimation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);
        AnimatedTexture crosshairAnimation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);

        float Deg2Rad(float Deg)
        {
            float Rad = Deg;
            Rad = Rad * (float)Math.PI / 180f;
            return Rad;
        }

        public Rectangle Bounds
        {
            get { return playerSprite.Bounds; }
        }

        public Vector2 Position
        {
            get
            {
                return playerSprite.position;
            }
            set
            {
                playerSprite.position = value;
            }
        }

        public Player(Game1 game)
        {
            this.game = game;
            velocity = Vector2.Zero;
            position = Vector2.Zero;
        }

        public void Load(ContentManager content)
        {
            playerTexture = content.Load<Texture2D>("player");
            playerAnimation.Load(content, "player", 1, 1);
            playerAnimation.Origin = new Vector2(playerTexture.Width / 2, playerTexture.Height / 2);
            playerSprite.Add(playerAnimation, 0, 0);
            playerSprite.Pause();

            crosshairTexture = content.Load<Texture2D>("crosshair");
            crosshairAnimation.Load(content, "crosshair", 1, 1);
            crosshairAnimation.Origin = new Vector2(crosshairTexture.Width / 2, crosshairTexture.Height / 2);
            crosshair.Add(crosshairAnimation, 0, 0);
            crosshair.Pause();

            fireballTexture = (content.Load<Texture2D>("fireball"));

            //dustParticle = content.Load<Texture2D>("dust");
            //dustEmitter = new Emitter(dustEmitter, playerSprite.position);
        }

        public void Update(float deltaTime)
        {
            playerSprite.Update(deltaTime);
            UpdateInput(deltaTime);
            

            crosshair.position = game.MousePos;

            Vector2 direction = game.MousePos - playerSprite.position;

            rotation = (float)Math.Atan2(direction.X, direction.Y);
            playerAnimation.Rotation = -rotation;

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Cast();
            }

            UpdateFireballs();
        }

        public void UpdateFireballs()
        {
            foreach (Fireball fireball in fireballs)
            {
                fireball.position += fireball.velocity;
                if (Vector2.Distance(fireball.position, playerSprite.position) > 500)
                {
                    fireball.isAlive = false;
                }
            }
            for (int i = 0; i < fireballs.Count; i++)
            {
                if (!fireballs[i].isAlive)
                {
                    fireballs.RemoveAt(i);
                    i--;
                }

            }
        }

        public void Cast()
        {
            Fireball newFireball = new Fireball(fireballTexture);
            newFireball.velocity = new Vector2((float)Math.Cos(-rotation + 1.5708f), (float)Math.Sin(-rotation + 1.5708f)) * 2f ;
            //newFireball.velocity.Normalize();
            newFireball.position = playerSprite.position + newFireball.velocity * 5 ;
            newFireball.isAlive = true;

            if (fireballs.Count() < 200)
            {
                fireballs.Add(newFireball);
            }
        }

        private void UpdateInput(float deltaTime)
        {
            bool wasMovingLeft = velocity.X < 0;
            bool wasMovingRight = velocity.X > 0;
            bool wasMovingUp = velocity.Y < 0;
            bool wasMovingDown = velocity.Y > 0;

            Vector2 acceleration = new Vector2(0, 0);

            if (Keyboard.GetState().IsKeyDown(Keys.A) == true)
            {
                acceleration.X -= Game1.xAcceleration;
            }
            else if (wasMovingLeft == true)
            {
                acceleration.X += Game1.xFriction;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D) == true)
            {
                acceleration.X += Game1.xAcceleration;
            }
            else if (wasMovingRight == true)
            {
                acceleration.X -= Game1.xFriction;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W) == true)
            {
                acceleration.Y -= Game1.yAcceleration;
            }
            else if (wasMovingUp == true)
            {
                acceleration.Y += Game1.yFriction;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S) == true)
            {
                acceleration.Y += Game1.yAcceleration;
            }
            else if (wasMovingDown == true)
            {
                acceleration.Y -= Game1.yFriction;
            }

            velocity += acceleration * deltaTime;

            velocity.X = MathHelper.Clamp(velocity.X, -Game1.maxVelocity.X, Game1.maxVelocity.X);
            velocity.Y = MathHelper.Clamp(velocity.Y, -Game1.maxVelocity.Y, Game1.maxVelocity.Y);

            playerSprite.position += velocity * deltaTime;

            if ((wasMovingLeft && (velocity.X > 0)) || (wasMovingRight && (velocity.X < 0)))
            {
                velocity.X = 0;
            }

            if ((wasMovingDown && (velocity.Y < 0)) || (wasMovingUp && (velocity.Y > 0)))
            {
                velocity.Y = 0;
            }

            // tx = Game.PixelToTile();

        }

        /*void CollisionDetection()
        {
            //Console.WriteLine("IS RUNNING?");
            // collision detection
            // Our collision detection logic is greatly simplified by the fact that 
            // the player is a rectangle and is exactly the same size as a single tile.
            // So we know that the player can only ever occupy 1, 2 or 4 cells.
            // This means we can short-circuit and avoid building a general purpose 
            // collision detection engine by simply looking at the 1 to 4 cells that 
            // the player occupies:
            int tx = game.PixelToTile(Position.X);
            int ty = game.PixelToTile(Position.Y);
            // nx = true if player overlaps right
            bool nx = (Position.X) % Game1.tile != 0;
            // ny = true if player overlaps below
            bool ny = (Position.Y) % Game1.tile != 0;
            bool cell = game.CellAtTileCoord(tx, ty) != 0;
            bool cellright = game.CellAtTileCoord(tx + 1, ty) != 0;
            bool celldown = game.CellAtTileCoord(tx, ty + 1) != 0;
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
                    playerSprite.position.Y = MathHelper.Lerp(playerSprite.position.Y, game.TileToPixel(ty), 0.5f);
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
                    playerSprite.position.Y = MathHelper.Lerp(playerSprite.position.Y, game.TileToPixel(ty + 1), .5f);
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
                    playerSprite.position.X = MathHelper.Lerp(playerSprite.position.X, game.TileToPixel(tx), .5f);
                    this.velocity.X = 0;      // stop horizontal velocity
                    playerSprite.Pause();
                }
            }
            else if (this.velocity.X < 0)
            {
                if ((cell && !cellright) || (celldown && !celldiag && ny))
                {
                    // clamp the x position to avoid moving into the platform 
                    // we just hit
                    playerSprite.position.X = MathHelper.Lerp(playerSprite.position.X, game.TileToPixel(tx + 1), .5f);
                    this.velocity.X = 0;      // stop horizontal velocity
                    playerSprite.Pause();
                }
            }

            // The last calculation for our update() method is to detect if the 
            // player is now falling or not. We can do that by looking to see if 
            // there is a platform below them
            //this.isFalling = !(celldown || (nx && celldiag));
        }*/


        public void Draw(SpriteBatch spriteBatch)
        {
            playerSprite.Draw(spriteBatch);
            crosshair.Draw(spriteBatch);

            foreach(Fireball fireball in fireballs)
            {
                fireball.Draw(spriteBatch);
            }
        }








    }
}
