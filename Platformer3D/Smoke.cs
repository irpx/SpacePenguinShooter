﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Factories;

namespace Platformer3D
{
    public class Smoke : GameObject3D
    {
        private int lifeTimer;
        private new Vector3 velocity;

        public Smoke(Vector3 spawnPos, float newScale)
        {
            lifeTimer = 1000;

            _model = Game1.Instance.Content.Load<Model>("Models\\smoke1");
            boneTransforms = new Matrix[_model.Bones.Count];

            scale = newScale * (float)Game1.random.Next(100) / 100f;

            _position = spawnPos;
            //_position.Y = -_position.Y;

            velocity = new Vector3(
                -25 + Game1.random.Next(50),
                -Game1.random.Next(50),
                -25 + Game1.random.Next(50)
                );

            //velocity.Normalize();
            velocity *= 0.001f;
            rotation += velocity.Y;
        }

        
        public override bool Update(float dTime)
        {
            //_position.X = _body.Position.X;
            //_position.Y = -_body.Position.Y;
            //_position.Z += 1;
            //_body.AngularVelocity = 0;

            velocity.Y -= dTime * 0.0001f;

            _position += velocity * dTime;

            rotationMatrix = Matrix.CreateRotationY(rotation);
            rotationMatrix *= Matrix.CreateRotationX(-rotation);
            if (lifeTimer > 0)
            {
                lifeTimer -= (int)dTime;
                scale += dTime * .05f;
                rotation += dTime * 0.1f*(float)Game1.random.NextDouble();
                return true;
            }
            return false;
        }
        public override void Destroy()
        {
            //Game1.physics.Remove(_body);
            //Game1.physics.Remove(_geom);
            
            Game1.Instance.particles.Remove(this);
        }

        public override void Draw(float dTime)
        {
            //Game1.Instance.debugTexts.Add("player X:" + (int)_position.X + ", Y:" + (int)_position.Y + ", Z:" + (int)_position.Z);
            //Game1.Instance.debugTexts.Add("player Body X:" + (int)_body.Position.X + ", Y:" + (int)_body.Position.Y);
            // Copy any parent transforms.

            _model.Root.Transform = rotationMatrix * Matrix.CreateScale(scale) * Matrix.CreateTranslation(_position);
            _model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in _model.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = Game1.Instance.camera.view;
                    effect.Projection = Game1.Instance.camera.projection;
                    effect.Alpha = lifeTimer / 1000f;
                    
                    if (lighting)
                    {
                        effect.EnableDefaultLighting();
                        effect.PreferPerPixelLighting = true;
                    }
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
