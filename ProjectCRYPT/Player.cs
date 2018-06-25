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

        }

    

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
