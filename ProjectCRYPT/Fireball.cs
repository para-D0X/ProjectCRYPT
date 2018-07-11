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
    public class Fireball
    {
        public Texture2D texture;

        Game1 game = null;

        public Vector2 position;
        public Vector2 velocity;
        public Vector2 origin = new Vector2(8, 8);

        public bool isAlive;

        Emitter fireEmitter = null;
        Texture2D fireParticle = null;

        public Sprite fireballSprite = new Sprite();
        AnimatedTexture fireballAnimation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);

        public Fireball(Texture2D newTexture, Game1 game)
        {
            texture = newTexture;
            isAlive = false;
            this.game = game;
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(texture, position, null, Color.White, 0f, origin, 1f, SpriteEffects.None, 0);
            fireballSprite.Draw(spriteBatch);

            fireEmitter.Draw(spriteBatch);
        }

        public void Load (ContentManager content)
        {
            fireParticle = content.Load<Texture2D>("fireball");

            fireballAnimation.Load(content, "fireball", 1, 1);

            fireballAnimation.Origin = new Vector2(texture.Width / 2, texture.Height / 2);

            fireballSprite.Add(fireballAnimation, 0, 0);
            fireballSprite.Pause();

            fireEmitter = new Emitter(fireParticle, new Vector2 (10, 10));
        }

        public void Update (float deltaTime)
        {

            fireballSprite.position = position;

            fireEmitter.position = position;
            fireEmitter.emissionRate = 15;
            fireEmitter.transparency = 1f;
            fireEmitter.minSize = 2;
            fireEmitter.maxSize = 5;
            fireEmitter.maxLife = 1.0f;
          
            fireEmitter.Update(deltaTime);


            int tx = game.PixelToTile(fireballSprite.position.X - 8);
            int ty = game.PixelToTile(fireballSprite.position.Y - 8);

            bool nx = (fireballSprite.position.X) % Game1.tile != 0;

            bool ny = (fireballSprite.position.Y) % Game1.tile != 0;

            bool cell = game.CellAtTileCoord(tx, ty) != 0;
            bool cellright = game.CellAtTileCoord(tx + 1, ty) != 0;
            bool celldown = game.CellAtTileCoord(tx, ty + 1) != 0;
            bool celldiag = game.CellAtTileCoord(tx + 1, ty + 1) != 0;

            if (this.velocity.Y > 0)
            {
                if ((celldown && !cell) || (celldiag && !cellright && nx))
                {
                    fireballSprite.position.Y = game.TileToPixel(ty);
                    isAlive = false;
                    ny = false;
                }
            }

            else if (this.velocity.Y < 0)
            {
                if ((cell && !celldown) || (cellright && !celldiag && nx))
                {
                    fireballSprite.position.Y = game.TileToPixel(ty + 1);
                    cell = celldown;
                    cellright = celldiag;
                    ny = false;
                    isAlive = false;
                    
                }
            }

            if (this.velocity.X > 0)
            {
                if ((cellright && !cell) || (celldiag && !celldown && ny))
                {
                    fireballSprite.position.X = game.TileToPixel(tx);
                    isAlive = false;
                    fireballSprite.Pause();
                }
            }
            else if (this.velocity.X < 0)
            {
                if ((cell && !cellright) || (celldown && !celldiag && ny))
                {
                    fireballSprite.position.X = game.TileToPixel(tx + 1);
                    isAlive = false;
                    fireballSprite.Pause();
                }
            }
        }

    }
}
