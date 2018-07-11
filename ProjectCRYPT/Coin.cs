using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectCRYPT
{
    class Coin
    {
        public Sprite coinSprite = new Sprite();
        // keep a reference to the Game object to check for collisions on the map
        Game1 game = null;

        Vector2 position = Vector2.Zero;
        Vector2 offset = new Vector2(8, 8);

        Texture2D coin;
               
        public bool isAlive;
        
        public Vector2 Position
        {
            get
            {
                return coinSprite.position;
            }
            set
            {
                coinSprite.position = value;
            }
        }
        public Coin(Sprite sprite)
        {
            isAlive = false;
        }

        public Rectangle Bounds
        {
            get
            {
                return coinSprite.Bounds;
            }
        }
        public Coin(Game1 game)
        {
            this.game = game;
        }
        public void Load(ContentManager content)
        {
            coin = content.Load<Texture2D>("coin");
            AnimatedTexture animation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);
            animation.Load(content, "coin", 1, 1);
            animation.Origin = new Vector2(coin.Width / 2, coin.Height / 2);
            coinSprite.Add(animation, coin.Width / 2, coin.Height / 2);
            coinSprite.Pause();
        }
        public void Update(float deltaTime)
        {
            coinSprite.Update(deltaTime);

            
        }

        
        public void Draw(SpriteBatch spriteBatch)
        {
            coinSprite.Draw(spriteBatch, Position);
        }
    }
}
