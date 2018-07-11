using Microsoft.Xna.Framework.Content;
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
    public class Turret
    {
        GraphicsDeviceManager graphics;
        public Sprite turretSprite = new Sprite();

        Game1 game = null; 

        float rotation = 0f;
        float distance = 85;
        float timerDelay = 0.60f;

        public Player GetPlayer { get; set; }
        public Bluefireball GetBluefireball { get; set; }
        public bool isAlive;
        Vector2 position = Vector2.Zero;
        Vector2 velocity = Vector2.Zero;
        Vector2 offset = new Vector2(8, 8);
        public float turretRotation = 0f;
                
        SoundEffect fireballSound;
        SoundEffectInstance fireballSoundInstance;

        public List<Bluefireball> bluefireballs = new List<Bluefireball>();

        Texture2D turret;
        Texture2D bluefireballTexture = null;

        AnimatedTexture turretAnimation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);

        public Rectangle Bounds
        {
            get { return turretSprite.Bounds; }
        }

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

            rot += MathHelper.ToRadians(270);

            return rot;
        }

        public Turret(Game1 game)
        {
            this.game = game;
            Position = new Vector2(50, 0);
            turretRotation = 0f;
        }

        public void Load(ContentManager content)
        {
            turret = content.Load<Texture2D>("turret");

            turretAnimation.Load(content, "turret", 1, 1);

            turretAnimation.Origin = new Vector2(turret.Width / 2, turret.Height / 2);

            turretSprite.Add(turretAnimation, turret.Width / 2, turret.Height / 2);
            turretSprite.Pause();

            bluefireballTexture = (content.Load<Texture2D>("fireball2"));

            fireballSound = content.Load<SoundEffect>("fireballSound");
            fireballSoundInstance = fireballSound.CreateInstance();
            

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
            int tx = game.PixelToTile(turretSprite.position.X);
            int ty = game.PixelToTile(turretSprite.position.Y);

            bool nx = (turretSprite.position.X) % Game1.tile != 0;

            bool ny = (turretSprite.position.Y) % Game1.tile != 0;

            bool cell = game.CellAtTileCoord(tx, ty) != 0;
            bool cellright = game.CellAtTileCoord(tx + 1, ty) != 0;
            bool celldown = game.CellAtTileCoord(tx, ty + 1) != 0;
            bool celldiag = game.CellAtTileCoord(tx + 1, ty + 1) != 0;
            
            Vector2.Distance(GetPlayer.Position, turretSprite.position);
            
            fireballSoundInstance.Volume = 0.3f;
            timerDelay -= deltaTime;

            if (Vector2.Distance(GetPlayer.Position, turretSprite.position) <= distance)
            {
                Vector2 direction = GetPlayer.playerSprite.position - turretSprite.position;

                rotation = (float)Math.Atan2(direction.X, direction.Y);
                turretAnimation.Rotation = -rotation;

                if (timerDelay <= 0)
                {
                    Cast();
                    timerDelay = 0.75f;
                }

            }

            UpdateBluefireballs(deltaTime);

            
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
            }

        }

        public void UpdateBluefireballs(float deltaTime)
        {
            foreach (Bluefireball bluefireball in bluefireballs)
            {
                bluefireball.position += bluefireball.velocity;
                if (Vector2.Distance(bluefireball.position, turretSprite.position) > 500)
                {
                    bluefireball.isAlive = false;

                }

                bluefireball.Update(deltaTime);
            }
            for (int i = 0; i < bluefireballs.Count; i++)
            {
                if (!bluefireballs[i].isAlive)
                {
                    bluefireballs.RemoveAt(i);
                    i--;
                }

            }
        }
        
        public void Cast()
        {
            Bluefireball newBluefireball = new Bluefireball(bluefireballTexture, game);
            newBluefireball.velocity = new Vector2((float)Math.Cos(-rotation + 1.5708f), (float)Math.Sin(-rotation + 1.5708f)) * 2f;
            newBluefireball.position = turretSprite.position + new Vector2(turret.Width / 2, turret.Height / 2) + newBluefireball.velocity * 5;
            newBluefireball.isAlive = true;
            newBluefireball.Load(game.Content);
            fireballSoundInstance.Play();


            if (bluefireballs.Count() < 200)
            {
                bluefireballs.Add(newBluefireball);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            turretSprite.Draw(spriteBatch);
            turretAnimation.Rotation = turretRotation;

            foreach (Bluefireball bluefireball in bluefireballs)
            {

                bluefireball.Draw(spriteBatch);
            }
        }
    }
}
