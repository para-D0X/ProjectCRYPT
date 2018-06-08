using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        Sprite zombieSprite = new Sprite();

        Game1 game = null;

        Vector2 position = Vector2.Zero;

        Texture2D zombie;

        AnimatedTexture zombieAnimation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);

        public Vector2 Position
        {
            get
            {
                return zombieSprite.position;
            }
        }

        public Zombie(Game1 game)
        {
            this.game = game;
            position = Vector2.Zero;
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
        }

        private void UpdateInput(float deltaTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            zombieSprite.Draw(spriteBatch); 
        }
    }
}
