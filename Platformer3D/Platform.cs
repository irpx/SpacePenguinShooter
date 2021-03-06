﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Factories;

namespace Platformer3D
{
    public class Platform : GameObject3D
    {
        public Platform(Vector2 pos)
        {
            _model = Game1.Instance.Content.Load<Model>("Models\\platform");
            boneTransforms = new Matrix[_model.Bones.Count];
            BoundingBox modelBox = new BoundingBox();
            bool firstBox = true;
            foreach (ModelMesh mesh in _model.Meshes)
            {
                BoundingBox meshBox = BoundingBox.CreateFromSphere(mesh.BoundingSphere);

                if (firstBox)
                    modelBox = meshBox;
                else
                    modelBox = BoundingBox.CreateMerged(modelBox, meshBox);
            }



            _position = new Vector3(pos.X, pos.Y, 0);

            float width = 10.0f;// modelBox.Max.X - modelBox.Min.X;
            float height = 1.0f;// modelBox.Max.Z - modelBox.Min.Z;

            scale = 10;

            width *= scale;
            height *= scale;


            /*
             *            Vector2[] vertsi = new Vector2[6];
            vertsi[0] = new Vector2(-20, -20);
            vertsi[1] = new Vector2(20, -20);
            vertsi[2] = new Vector2(20, 20);
            vertsi[3] = new Vector2(-20, 20);
            vertsi[4] = new Vector2(10, 30);
            vertsi[5] = new Vector2(0, 10);
            FarseerGames.FarseerPhysics.Collisions.Vertices vert = new FarseerGames.FarseerPhysics.Collisions.Vertices(vertsi);

            _body = BodyFactory.Instance.CreatePolygonBody(Game1.physics,vert,10.0f);

            _geom = GeomFactory.Instance.CreatePolygonGeom(Game1.physics, _body, vert, 10);
             * */

            _body = BodyFactory.Instance.CreateRectangleBody(Game1.physics, width, height, 100.0f);
            _body.Position = new Vector2(_position.X, -_position.Y);
            _body.IsStatic = true;
            _geom = GeomFactory.Instance.CreateRectangleGeom(Game1.physics, _body, width, height, 1f);
            _geom.FrictionCoefficient = 0.4f;
            _geom.RestitutionCoefficient = 0f;
            _geom.CollisionCategories = FarseerGames.FarseerPhysics.CollisionCategory.Cat5;

            _position.Z = 0;
        }
    }
}
