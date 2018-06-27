﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using ParticleEffects;

namespace ProjectCRYPT
{
    class Fireball
    {
        public Texture2D texture;

        public Vector2 position;
        public Vector2 velocity;
        public Vector2 origin = new Vector2(8, 8);

        public bool isAlive;

        Emitter fireEmitter = null;
        Texture2D fireParticle = null;

        public Fireball(Texture2D newTexture)
        {
            texture = newTexture;
            isAlive = false;
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0);

            fireEmitter.Draw(spriteBatch);
        }

        public void Load (ContentManager content)
        {
            fireParticle = content.Load<Texture2D>("fireball");
            fireEmitter = new Emitter(fireParticle, new Vector2 (10, 10));
        }

        public void Update (float deltaTime)
        {
            
            fireEmitter.position = position;
            fireEmitter.emissionRate = 15;
            fireEmitter.transparency = 1f;
            fireEmitter.minSize = 2;
            fireEmitter.maxSize = 5;
            fireEmitter.maxLife = 1.0f;
           

            fireEmitter.Update(deltaTime);
        }

    }
}
