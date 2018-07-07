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
using Microsoft.Xna.Framework.Audio;

namespace ProjectCRYPT
{
    class Turret
    {
        GraphicsDeviceManager graphics;
        public Sprite turretSprite = new Sprite();

        Game1 game = null;

        //float rotation = 0f;
        float distance = 75;
        //float timerDelay = 0.75f;

        public Player GetPlayer { get; set; }
        public Fireball GetFireball { get; set; }
        public bool isAlive;
        Vector2 position = Vector2.Zero;
        Vector2 velocity = Vector2.Zero;
        public float turretRotation = 40f;
        
        //SoundEffect fireballSound;
        //SoundEffectInstance fireballSoundInstance;

        //public List<Fireball> fireballs = new List<Fireball>();

        Texture2D turret;
        //Texture2D fireballTexture = null;

        AnimatedTexture turretAnimation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);

        public Vector2 Position
        {
            get
            {
                return turretSprite.position;
            }
            set
            {
                turretSprite.position = value;
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

        public Turret(Game1 game)
        {
            this.game = game;
            Position = new Vector2(100, 0);
            turretRotation = 40;
        }

        public void Load(ContentManager content)
        {
            turret = content.Load<Texture2D>("turret");

            turretAnimation.Load(content, "turret", 1, 1);

            turretAnimation.Origin = new Vector2(turret.Width / 2, turret.Height / 2);

            turretSprite.Add(turretAnimation, turret.Width / 2, turret.Height / 2);
            turretSprite.Pause();

            /*fireballTexture = (content.Load<Texture2D>("fireball"));

            fireballSound = content.Load<SoundEffect>("fireballSound");
            fireballSoundInstance = fireballSound.CreateInstance();
            */

        }

        public void Update(float deltaTime)
        {
            turretSprite.Update(deltaTime);
            UpdateInput(deltaTime);
            turretRotation = RotateTo(GetPlayer.Position);
            turretAnimation.Rotation = turretRotation;
            position = turretSprite.position;
        }

        private void UpdateInput(float deltaTime)
        {
            /*int tx = game.PixelToTile(turretSprite.position.X);
            int ty = game.PixelToTile(turretSprite.position.Y);

            bool nx = (turretSprite.position.X) % Game1.tile != 0;

            bool ny = (turretSprite.position.Y) % Game1.tile != 0;

            bool cell = game.CellAtTileCoord(tx, ty) != 0;
            bool cellright = game.CellAtTileCoord(tx + 1, ty) != 0;
            bool celldown = game.CellAtTileCoord(tx, ty + 1) != 0;
            bool celldiag = game.CellAtTileCoord(tx + 1, ty + 1) != 0;
            
            Vector2.Distance(GetPlayer.Position, turretSprite.position);

            timerDelay -= deltaTime;

            if (Vector2.Distance(GetPlayer.Position, turretSprite.position) <= distance)
            {
                if (timerDelay <= 0)
                {
                    Cast();
                    timerDelay = 0.75f;
                }

            }

            UpdateFireballs(deltaTime);

            
            if (this.velocity.Y > 0)
            {
                if ((celldown && !cell) || (celldiag && !cellright && nx))
                {
                    turretSprite.position.Y = game.TileToPixel(ty);
                    this.velocity.Y = 0;
                    ny = false;
                }
            }

            else if (this.velocity.Y < 0)
            {
                if ((cell && !celldown) || (cellright && !celldiag && nx))
                {
                    turretSprite.position.Y = game.TileToPixel(ty + 1);
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
                    turretSprite.position.X = game.TileToPixel(tx);
                    this.velocity.X = 0;
                    turretSprite.Pause();
                }
            }
            else if (this.velocity.X < 0)
            {
                if ((cell && !cellright) || (celldown && !celldiag && ny))
                {
                    turretSprite.position.X = game.TileToPixel(tx + 1);
                    this.velocity.X = 0;
                    turretSprite.Pause();
                }
            }*/

        }

        /*public void UpdateFireballs(float deltaTime)
        {
            foreach (Fireball fireball in fireballs)
            {
                fireball.position += fireball.velocity;
                if (Vector2.Distance(fireball.position, turretSprite.position) > 500)
                {
                    fireball.isAlive = false;

                }

                fireball.Update(deltaTime);
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
            newFireball.velocity = new Vector2((float)Math.Cos(-rotation + 1.5708f), (float)Math.Sin(-rotation + 1.5708f)) * 1f;
            newFireball.position = turretSprite.position + new Vector2(turret.Width / 2, turret.Height / 2) + newFireball.velocity * 5;
            newFireball.isAlive = true;
            newFireball.Load(game.Content);
            fireballSoundInstance.Play();


            if (fireballs.Count() < 200)
            {
                fireballs.Add(newFireball);
            }
        }*/

        public void Draw(SpriteBatch spriteBatch)
        {
            turretSprite.Draw(spriteBatch);
            turretAnimation.Rotation = turretRotation;

            /*foreach (Fireball fireball in fireballs)
            {

                fireball.Draw(spriteBatch);
            }*/
        }
    }
}
