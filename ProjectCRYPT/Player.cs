using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectCRYPT
{
    class Player
    {
        Sprite playerSprite = new Sprite();
        Sprite crosshair = new Sprite();

        Game1 game = null;

        Vector2 velocity = Vector2.Zero;
        Vector2 position = Vector2.Zero;

        public Vector2 Position
        {
            get
            {
                return playerSprite.position;
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
            playerSprite.Load(content, "player2");
            crosshair.Load(content, "crosshair");
        }

        public void Update(float deltaTime)
        {
            playerSprite.Update(deltaTime);
            UpdateInput(deltaTime);
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




        public void Draw(SpriteBatch spriteBatch)
        {
            playerSprite.Draw(spriteBatch);
            crosshair.Draw(spriteBatch);
        }








    }
}
