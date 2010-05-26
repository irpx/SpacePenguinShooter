using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Platformer3D
{
    public class Camera
    {
        public float aspectRatio;

        public Vector3 position;
        public Vector3 targetPosition;

        private int shakeTimer;


        /// <summary>
        /// Projection matrix
        /// </summary>
        public Matrix projection;
        /// <summary>
        /// View Matrix
        /// </summary>
        public Matrix view;
        public Vector3 cameraUpVector;

        public Camera()
        {
            Init();
        }

        private void Init()
        {
            aspectRatio = (float)Game1.Instance.GraphicsDevice.Viewport.Width /
            (float)Game1.Instance.GraphicsDevice.Viewport.Height;

            position = Vector3.Zero;
            targetPosition = Vector3.Zero;
            cameraUpVector = Vector3.Up;
            shakeTimer = 0;
        }
        public void Update(float dTime)
        {
            targetPosition += (Game1.Instance.player.Get3Dpos() - targetPosition + new Vector3(0, 30, 0)) * 0.1f;
            targetPosition += new Vector3(Input.pad.ThumbSticks.Right.X, Input.pad.ThumbSticks.Right.Y, 0) * 10.0f;

            Vector3 shakeOffset = Vector3.Zero;
            if (shakeTimer > 0)
            {
                float shakeValue = shakeTimer / 500f;
                shakeOffset = new Vector3(
                    -50 + Game1.random.Next(100),
                    -50 + Game1.random.Next(100),
                    -50 + Game1.random.Next(100));
                shakeOffset *= 0.002f*dTime;
                
                shakeOffset *= shakeValue;

                shakeTimer -= (int)dTime;
                targetPosition += shakeOffset;
                Game1.Instance.debugTexts.Add("SHAKE:" + shakeOffset.ToString());
            }

            position += (Game1.Instance.player.Get3Dpos() - position + new Vector3(0.0f, 30.0f, 80.0f+Input.pad.ThumbSticks.Right.LengthSquared()*60)) * 0.1f;
            //position += (targetPosition - position + new Vector3(0.0f, 10.0f, 80.0f)) * 0.1f;
            
            Game1.Instance.debugTexts.Add("Camera pos:" + position.ToString());
            
            Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, aspectRatio, 1.0f, 1000.0f, out projection);
            Matrix.CreateLookAt(ref this.position, ref this.targetPosition, ref cameraUpVector, out view);
            view *= Matrix.CreateRotationZ(MathHelper.ToRadians(Input.pad.ThumbSticks.Right.X*15));
        }

        internal void Shake()
        {
            shakeTimer = 500;
        }
    }
}