using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Factories;


namespace Platformer3D
{
    public class Bullet1 : GameObject3D
    {
        private int lifeSpan = 5000;//IN MS
        public Bullet1(Vector2 speed, Vector3 spawnPos)
        {
            _model = Game1.Instance.Content.Load<Model>("Models\\bullet1");
            boneTransforms = new Matrix[_model.Bones.Count];

            scale = 1.0f;

            lighting = false;
            
            _position = spawnPos;
            _position.Y = -_position.Y;

            _body = BodyFactory.Instance.CreateCircleBody(Game1.physics, 1.0f, 0.01f);
            _body.IgnoreGravity = true;
            _geom = GeomFactory.Instance.CreateCircleGeom(Game1.physics, _body, 1.0f,4);
            _geom.CollisionCategories = FarseerGames.FarseerPhysics.CollisionCategory.Cat2;
            _geom.OnCollision = OnCollision;
            
            _geom.FrictionCoefficient = .4f;
            _geom.RestitutionCoefficient = 0.2f;

            _body.Position = new Vector2(_position.X, _position.Y);
            _body.LinearVelocity = speed;
        }
        public override bool Update(float dTime)
        {
            rotation += 0.5f;
            rotationMatrix = Matrix.CreateRotationY(rotation);
            if (scale < 5.0f)
            {
                scale += 0.5f;
                lighting = false;
            }
            else
            {
                scale = 2.0f;
                lighting = true;
            }
            base.Update(dTime);
            lifeSpan -= (int)dTime;
            if (lifeSpan < 0) return false;
            return true;
        }
        public override void Destroy()
        {
            Game1.physics.Remove(_body);
            Game1.physics.Remove(_geom);
            Game1.Instance.bullets.Remove(this);
        }
        public override bool OnCollision(FarseerGames.FarseerPhysics.Collisions.Geom geom1, FarseerGames.FarseerPhysics.Collisions.Geom geom2, FarseerGames.FarseerPhysics.Collisions.ContactList contacts)
        {
            FarseerGames.FarseerPhysics.Collisions.Geom anotherGeom;
            if (geom1 == _geom)
                anotherGeom = geom2;
            else
                anotherGeom = geom1;

            //Game1.Instance.debugTexts.Add("Col:" + anotherGeom.CollisionCategories.ToString());
            if (anotherGeom.CollisionCategories == FarseerGames.FarseerPhysics.CollisionCategory.Cat3)
            {
                Enemy obj = (Enemy)anotherGeom.Tag;
                if (!obj.alive) return false;
                obj.BulletHit();
            }

            //return base.OnCollision(geom1, geom2, contacts);
            lifeSpan = -1;

            Game1.Instance.particles.Add(new BulletSpark(Get3Dpos(), 0.00001f, -_body.LinearVelocity));

            return true;
        }
    }
}
