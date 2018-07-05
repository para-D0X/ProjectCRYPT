﻿using System;
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
            AnimatedTexture animation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);
            animation.Load(content, "coin", 1, 1);

            coinSprite.Add(animation, 0, 1);
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
