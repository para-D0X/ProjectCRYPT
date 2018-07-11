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

        Game1 game = null;

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

        public Bluefireball(Texture2D newTexture, Game1 game)
        {
            texture = newTexture;
            isAlive = false;
            this.game = game;
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

            int tx = game.PixelToTile(bluefireballSprite.position.X - 8);
            int ty = game.PixelToTile(bluefireballSprite.position.Y - 8);

            bool nx = (bluefireballSprite.position.X) % Game1.tile != 0;

            bool ny = (bluefireballSprite.position.Y) % Game1.tile != 0;

            bool cell = game.CellAtTileCoord(tx, ty) != 0;
            bool cellright = game.CellAtTileCoord(tx + 1, ty) != 0;
            bool celldown = game.CellAtTileCoord(tx, ty + 1) != 0;
            bool celldiag = game.CellAtTileCoord(tx + 1, ty + 1) != 0;

            if (this.velocity.Y > 0)
            {
                if ((celldown && !cell) || (celldiag && !cellright && nx))
                {
                    bluefireballSprite.position.Y = game.TileToPixel(ty);
                    isAlive = false;
                    ny = false;
                }
            }

            else if (this.velocity.Y < 0)
            {
                if ((cell && !celldown) || (cellright && !celldiag && nx))
                {
                    bluefireballSprite.position.Y = game.TileToPixel(ty + 1);
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
                    bluefireballSprite.position.X = game.TileToPixel(tx);
                    isAlive = false;
                    bluefireballSprite.Pause();
                }
            }
            else if (this.velocity.X < 0)
            {
                if ((cell && !cellright) || (celldown && !celldiag && ny))
                {
                    bluefireballSprite.position.X = game.TileToPixel(tx + 1);
                    isAlive = false;
                    bluefireballSprite.Pause();
                }
            }
        }
    }
}
