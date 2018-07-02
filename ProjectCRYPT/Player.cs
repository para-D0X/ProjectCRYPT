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

        #region Variables

        Vector2 velocity = Vector2.Zero;
        Vector2 position = Vector2.Zero;     

        float rotation = 0f;
        float timerDelay = 0.5f;

        Texture2D playerTexture = null;
        Texture2D crosshairTexture = null;
        Texture2D fireballTexture = null;
        Texture2D dustParticle = null;


        public List<Fireball> fireballs = new List<Fireball>();

        Sprite playerSprite = new Sprite();
        Sprite crosshair = new Sprite();
        
        Emitter dustEmitter = null;


        AnimatedTexture playerAnimation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);
        AnimatedTexture crosshairAnimation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);

        #endregion

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
            playerSprite.Add(playerAnimation, playerTexture.Width / 2, playerTexture.Height / 2);
            playerSprite.Pause();

            crosshairTexture = content.Load<Texture2D>("crosshair");
            crosshairAnimation.Load(content, "crosshair", 1, 1);
            //crosshairAnimation.Origin = new Vector2(crosshairTexture.Width / 2, crosshairTexture.Height / 2);
            crosshair.Add(crosshairAnimation, 0, 0);
            crosshair.Pause();

            fireballTexture = (content.Load<Texture2D>("fireball"));

            dustParticle = content.Load<Texture2D>("dust");
            dustEmitter = new Emitter(dustParticle, playerSprite.position);


        }

        public void Update(float deltaTime)
        {
            playerSprite.Update(deltaTime);
            UpdateInput(deltaTime);

            KeyboardState state = Keyboard.GetState();

            crosshair.position = game.MousePos;

            Vector2 direction = game.MousePos - playerSprite.position;

            rotation = (float)Math.Atan2(direction.X, direction.Y);
            playerAnimation.Rotation = -rotation;

            timerDelay -= deltaTime;

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && timerDelay <= 0)
            {
                Cast();
                timerDelay = 0.5f;
            }

            UpdateFireballs(deltaTime);

            #region RunningParticles
            if (state.IsKeyDown(Keys.A) || (state.IsKeyDown(Keys.D) || (state.IsKeyDown(Keys.W) || (state.IsKeyDown(Keys.S) == true))))
            {
             
                dustEmitter.position = playerSprite.position ;
                dustEmitter.emissionRate = 15;
                dustEmitter.transparency = 1f;
                dustEmitter.minSize = 2;
                dustEmitter.maxSize = 5;
                dustEmitter.maxLife = 1.0f;               
            }
            else
            {
                dustEmitter.position = new Vector2(-200, -200);
            }

            dustEmitter.Update(deltaTime);
            #endregion

        }

        public void UpdateFireballs(float deltaTime)
        {
            foreach (Fireball fireball in fireballs)
            {
                fireball.position += fireball.velocity;
                if (Vector2.Distance(fireball.position, playerSprite.position) > 500)
                {
                    fireball.isAlive = false;

                }

                fireball.Update(deltaTime);
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
            newFireball.position = playerSprite.position + new Vector2 (playerTexture.Width / 2, playerTexture.Height / 2) + newFireball.velocity * 5 ;
            newFireball.isAlive = true;
            newFireball.Load(game.Content);

            if (fireballs.Count() < 200)
            {
                fireballs.Add(newFireball);
            }
        }

        private void UpdateInput(float deltaTime)
        {
            #region Player Movement 
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

            #endregion

            #region Player-To-Tile Collision Detection

            int tx = game.PixelToTile(playerSprite.position.X);
            int ty = game.PixelToTile(playerSprite.position.Y);

            bool nx = (playerSprite.position.X) % Game1.tile != 0;

            bool ny = (playerSprite.position.Y) % Game1.tile != 0;

            bool cell = game.CellAtTileCoord(tx, ty) != 0;
            bool cellright = game.CellAtTileCoord(tx + 1, ty) != 0;
            bool celldown = game.CellAtTileCoord(tx, ty + 1) != 0;
            bool celldiag = game.CellAtTileCoord(tx + 1, ty + 1) != 0;

            if (this.velocity.Y > 0)
            {
                if ((celldown && !cell) || (celldiag && !cellright && nx))
                {
                    playerSprite.position.Y = game.TileToPixel(ty);
                    this.velocity.Y = 0;
                    ny = false;
                }
            }

            else if (this.velocity.Y < 0)
            {
                if ((cell && !celldown) || (cellright && !celldiag && nx))
                {
                    playerSprite.position.Y = game.TileToPixel(ty + 1);
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
                    playerSprite.position.X = game.TileToPixel(tx);
                    this.velocity.X = 0;
                    playerSprite.Pause();
                }
            }
            else if (this.velocity.X < 0)
            {
                if ((cell && !cellright) || (celldown && !celldiag && ny))
                {
                    playerSprite.position.X = game.TileToPixel(tx + 1);
                    this.velocity.X = 0;
                    playerSprite.Pause();
                }
            }
            #endregion
        }



        public void Draw(SpriteBatch spriteBatch)
        {
            dustEmitter.Draw(spriteBatch);
            playerSprite.Draw(spriteBatch);
        
            foreach (Fireball fireball in fireballs)
            {

                fireball.Draw(spriteBatch);
            }

            crosshair.Draw(spriteBatch);
        }








    }
}
