using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer3D
{
    public class GameObject2D
    {
        public Vector2 position;// { get; set; }
        public float radius { get; set; }

        private Texture2D _texture;
        public Texture2D texture
        {
            get { return this._texture; }
            set { this._texture = value; }
        }

        private Vector2 _center;

        private Color _drawColor;
        public Color drawColor
        {
            get { return this._drawColor; }
            set { this._drawColor = value; }
        }
        private Rectangle _drawRect;
        private Rectangle _sourceRect;

        public BoundingSphere bounds;

        public int currentFrame;
        public int frameWidth;
        public int currentAnimationTrack;
        public int animationTrackHeight;

        private float _alpha;
        private float _targetAlpha;
        public float alpha
        {
            get { return this._alpha; }
            set { this._targetAlpha = value; }
        }
        public int fadeSpeed = 30;

        private float _rotation;
        private float _scale;
        public float scale
        {
            get { return this._scale; }
            set { this._scale = value; }
        }
        private int _layer;
        private SpriteEffects _drawEffect;

        private bool flash;

        private int flashTimer;

        public bool collisionChecked;

        public GameObject2D(Vector2 pos, string texName)
        {
            this.position = pos;
            _rotation = 0f;
            _scale = 1f;
            _layer = 1;
            _drawEffect = SpriteEffects.None;

            Game1.textures.TryGetValue(texName, out this._texture);
            this._center = new Vector2(_texture.Width / 2, _texture.Height / 2);

            frameWidth = _texture.Width;
            animationTrackHeight = _texture.Height;
            currentFrame = currentAnimationTrack = 0;

            _sourceRect = new Rectangle(currentFrame * frameWidth, currentAnimationTrack * animationTrackHeight, frameWidth, animationTrackHeight);

            radius = _texture.Width;// / 2;

            bounds = new BoundingSphere(new Vector3(position.X, position.Y, 0), radius);

            this._drawColor = Color.White;
            this._alpha = 1f;
            this._targetAlpha = 0f;
        }

        public virtual void Update(float dTime)
        {
   

           // this.position.X += -1 + GameData.random.Next(3);
            //this.position.Y += -1 + GameData.random.Next(3);
            /*
             *  collisionChecked = false;
            if (position.X < 1) position.X = 1;
            if (position.Y < 1) position.Y = 1;
            if (position.X > GameData.screenWidth) position.X = GameData.screenWidth;
            if (position.Y > GameData.screenHeight) position.Y = GameData.screenHeight;

            bounds = new BoundingSphere(new Vector3(position.X, position.Y, 0), radius);

            GameData.spatialManager.registerObject(this);
            */

            if (_alpha != _targetAlpha)
            {
                float dif = _targetAlpha - _alpha;
                if (Math.Abs(dif) > 0.01f) dif /= fadeSpeed;
                this._alpha += dif;
                this._drawColor.A = (byte)(this._alpha * 255f);
            }
            else if (flash)
            {
                flash = false;
                //_alpha = 1.0f;
                _targetAlpha = 0.0f;
            }
        }

        public void Flash(int time)
        {
            flashTimer = time;
            flash = true;
            _alpha = 1.0f;
            _targetAlpha = 0.0f;
            fadeSpeed = 5;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            _drawRect = new Rectangle(
                (int)(position.X),
                (int)(position.Y),
                (int)(800*_scale),
                (int)(600 * _scale)
                );

            sb.Draw(_texture, _drawRect, _sourceRect, _drawColor, _rotation, _center, _drawEffect, _layer);
        }
    }
}
