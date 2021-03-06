﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCRYPT
{
    class Skeleton
    {
        GraphicsDeviceManager graphics;
        public Sprite skeletonSprite = new Sprite();


        Game1 game = null;


        float distance = 140;

        Vector2 position = Vector2.Zero;
        Vector2 velocity = Vector2.Zero;

        public Player GetPlayer { get; set; }

        public float skeletonRotation = 0f;
        float skeletonSpeed = 38f;

        Texture2D skeleton;

        AnimatedTexture skeletonAnimation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);

        public Rectangle Bounds
        {
            get { return skeletonSprite.Bounds; }
        }

        public Vector2 Position
        {
            get
            {
                return skeletonSprite.position;
            }
            set
            {
                skeletonSprite.position = value;
            }
        }

        float RotateTo(Vector2 pointTo)
        {
            float rot = 0;

            Vector2 direction = position - pointTo;
            direction.Normalize();

            rot = (float)Math.Atan2((double)direction.Y, (double)direction.X);

            rot += MathHelper.ToRadians(180);

            return rot;
        }

        public Skeleton(Game1 game)
        {
            this.game = game;
            Position = new Vector2(100, 0);
            skeletonRotation = 0;
        }

        public void Load(ContentManager content)
        {
            skeleton = content.Load<Texture2D>("skeleton");

            skeletonAnimation.Load(content, "skeleton", 1, 1);

            skeletonAnimation.Origin = new Vector2(skeleton.Width / 2, skeleton.Height / 2);

            skeletonSprite.Add(skeletonAnimation, skeleton.Width / 2, skeleton.Height / 2);
            skeletonSprite.Pause();
            
        }

        public void Update(float deltaTime)
        {
            skeletonSprite.Update(deltaTime);
            UpdateInput(deltaTime);
            skeletonRotation = RotateTo(GetPlayer.Position);
            skeletonAnimation.Rotation = skeletonRotation;
            position = skeletonSprite.position;

        }

        private void UpdateInput(float deltaTime)
        {
            int tx = game.PixelToTile(skeletonSprite.position.X);
            int ty = game.PixelToTile(skeletonSprite.position.Y);

            bool nx = (skeletonSprite.position.X) % Game1.tile != 0;

            bool ny = (skeletonSprite.position.Y) % Game1.tile != 0;

            bool cell = game.CellAtTileCoord(tx, ty) != 0;
            bool cellright = game.CellAtTileCoord(tx + 1, ty) != 0;
            bool celldown = game.CellAtTileCoord(tx, ty + 1) != 0;
            bool celldiag = game.CellAtTileCoord(tx + 1, ty + 1) != 0;

            Vector2.Distance(GetPlayer.Position, skeletonSprite.position);

            if (Vector2.Distance(GetPlayer.Position, skeletonSprite.position) <= distance)
            {
                Vector2 direction;

                direction = GetPlayer.Position - Position;
                direction.Normalize();

                velocity = direction * (Game1.maxVelocity * deltaTime);

                skeletonSprite.position += velocity * skeletonSpeed * deltaTime;
            }



            if (this.velocity.Y > 0)
            {
                if ((celldown && !cell) || (celldiag && !cellright && nx))
                {
                    skeletonSprite.position.Y = game.TileToPixel(ty);
                    this.velocity.Y = 0;
                    ny = false;
                }
            }

            else if (this.velocity.Y < 0)
            {
                if ((cell && !celldown) || (cellright && !celldiag && nx))
                {
                    skeletonSprite.position.Y = game.TileToPixel(ty + 1);
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
                    skeletonSprite.position.X = game.TileToPixel(tx);
                    this.velocity.X = 0;
                    skeletonSprite.Pause();
                }
            }
            else if (this.velocity.X < 0)
            {
                if ((cell && !cellright) || (celldown && !celldiag && ny))
                {
                    skeletonSprite.position.X = game.TileToPixel(tx + 1);
                    this.velocity.X = 0;
                    skeletonSprite.Pause();
                }
            }

        }


        public void Draw(SpriteBatch spriteBatch)
        {
            skeletonSprite.Draw(spriteBatch);
            skeletonAnimation.Rotation = skeletonRotation;
            //spriteBatch.DrawRectangle(zombieSprite.Bounds, Color.White, 1);
        }
    }
}

