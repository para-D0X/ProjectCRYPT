using System;
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
    public class Bluefireball
    {
        public Texture2D texture;

        public Vector2 position;
        public Vector2 velocity;
        public Vector2 origin = new Vector2(8, 8);

        public bool isAlive;

        Emitter fireEmitter = null;
        Texture2D fireParticle = null;

        public Sprite bluefireballSprite = new Sprite();
        AnimatedTexture bluefireballAnimation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);

        public Rectangle Bounds
        {
            get { return bluefireballSprite.Bounds; }
        }

        public Bluefireball(Texture2D newTexture)
        {
            texture = newTexture;
            isAlive = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            bluefireballSprite.Draw(spriteBatch);
            fireEmitter.Draw(spriteBatch);
        }

        public void Load(ContentManager content)
        {
            fireParticle = content.Load<Texture2D>("fireball2");

            bluefireballAnimation.Load(content, "fireball2", 1, 1);

            bluefireballAnimation.Origin = new Vector2(texture.Width / 2, texture.Height / 2);

            bluefireballSprite.Add(bluefireballAnimation, 0, 0);
            bluefireballSprite.Pause();

            fireEmitter = new Emitter(fireParticle, new Vector2(10, 10));
        }

        public void Update(float deltaTime)
        {

            bluefireballSprite.position = position;

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
