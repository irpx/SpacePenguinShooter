using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Collisions;

namespace Platformer3D
{
    public class GameObject3D
    {
        protected Model _model;
        protected Body _body;
        protected Geom _geom;

        public bool onGround = false;
        public bool hangingOnWall = false;

        protected bool lighting = true;

        protected Vector3 _position;
        public Vector3 position
        {
            get
            {
                Vector3 vec = new Vector3(_body.Position.X, _body.Position.Y, _position.Z);
                return vec;
            }
            set
            {
                _body.Position = new Vector2(value.X, value.Y);
                _position = value;
            }
        }
        public Vector3 Get3Dpos()
        {
            Vector3 vec = new Vector3(_body.Position.X, -_body.Position.Y, _position.Z);
            return vec;
        }

        public Vector2 velocity
        {
            get { return _body.LinearVelocity; }
            set { _body.LinearVelocity = value; }
        }
        public float rotation = 0.0f;
        public Matrix rotationMatrix = Matrix.Identity;
        public float scale = 1.0f;
        public bool alive = false;
        public Matrix[] boneTransforms;
        


        public GameObject3D()
        {
            _position = Vector3.Zero;
            rotation = 0.0f;// Vector3.Zero;
        }
        public virtual bool Update(float dTime)
        {
            if (_body.Position.Y > 700)
            {
                _body.Position = new Vector2(400,0);
                _body.LinearVelocity = Vector2.Zero;
                Sounds.PlaySound("powerup");
            }

            _position.X = _body.Position.X;
            _position.Y = -_body.Position.Y;

            rotationMatrix = Matrix.CreateRotationY(rotation);

            //Ignore Z?
            return true;
        }
        public virtual void Jump(float force)
        {
            _body.ApplyImpulse(new Vector2(0.0f,-force));
        }

        public virtual void DrawShaders(float dTime)
        {

            Effect fx;
            Shaders.list.TryGetValue("test", out fx);

            _model.Root.Transform = rotationMatrix * Matrix.CreateScale(scale) * Matrix.CreateTranslation(_position);
            _model.CopyAbsoluteBoneTransformsTo(boneTransforms);

             // Use the AmbientLight technique from Shader.fx. You can have multiple techniques in a effect file. If you don't specify
            // what technique you want to use, it will choose the first one by default.
            fx.CurrentTechnique = fx.Techniques["AmbientLight"];

            // Begin our effect
            fx.Begin();

            // A shader can have multiple passes, be sure to loop trough each of them.
            foreach (EffectPass pass in fx.CurrentTechnique.Passes)
            {
                // Begin current pass
                pass.Begin();

            // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in _model.Meshes)
                {

                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        // calculate our worldMatrix..
                        //worldMatrix = bones[mesh.ParentBone.Index] * renderMatrix;

                        // .. and pass it into our shader.
                        // To access a parameter defined in our shader file ( Shader.fx ), use effectObject.Parameters["variableName"]
                        fx.Parameters["matWorldViewProj"].SetValue(boneTransforms[mesh.ParentBone.Index] * Game1.Instance.camera.view * Game1.Instance.camera.projection);

                        // Render our meshpart
                        Game1.Instance.GraphicsDevice.Vertices[0].SetSource(mesh.VertexBuffer, part.StreamOffset, part.VertexStride);
                        Game1.Instance.GraphicsDevice.Indices = mesh.IndexBuffer;
                        Game1.Instance.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,
                                                                      part.BaseVertex, 0, part.NumVertices,
                                                                      part.StartIndex, part.PrimitiveCount);
                    }

                    /* OLD!
                    // This is where the mesh orientation is set, as well as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = boneTransforms[mesh.ParentBone.Index];
                        effect.View = Game1.Instance.camera.view;
                        effect.Projection = Game1.Instance.camera.projection;
                        if (lighting)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;
                        }
                   }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                     */
                }
                pass.End();

            }
            fx.End();
        }
        public virtual void Draw(float dTime)
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
        public virtual bool OnCollision(Geom geom1, Geom geom2, ContactList contacts)
        {
            contacts.ForEach(delegate(Contact contact)
            {
                if (contact.Normal.Y > Math.Abs(contact.Normal.X))
                {
                    onGround = true;
                }
            });
            return true;
        }

        public virtual void OnSeparation(Geom geom1, Geom geom2)
        {
            onGround = false;
            return;
        }

        public virtual bool OnBroadPhaseCollision(Geom geom1, Geom geom2)
        {
            return true;
        }

        public virtual void Destroy()
        {
        }
        public virtual void EnemyHit(Vector2 force)
        { 
        }
    }
}
