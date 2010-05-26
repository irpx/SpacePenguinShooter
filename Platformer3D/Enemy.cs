using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Factories;

namespace Platformer3D
{
    public class Enemy : GameObject3D
    {
        private int turnTimer;
        private int range;
        private Vector2 anchorPos;
        private bool isFacingLeft;

        private int health;

        private int corpseTimer;

        public Enemy(Vector3 spawnPos)
        {
            _model = Game1.Instance.Content.Load<Model>("Models\\enemy1");
            boneTransforms = new Matrix[_model.Bones.Count];

            _body = BodyFactory.Instance.CreateRectangleBody(Game1.physics, 10.0f, 20.0f, 10.0f);
            _body.IsAutoIdle = true;
            _body.RotationalDragCoefficient = 1.0f;
            _body.LinearDragCoefficient = 0f;

            _geom = GeomFactory.Instance.CreateRectangleGeom(Game1.physics, _body, 10.0f, 20.0f, new Vector2(0, -10), 0, 0.1f);
            _geom.CollisionCategories = FarseerGames.FarseerPhysics.CollisionCategory.Cat3;
            _geom.OnCollision = this.OnCollision;

            _geom.Tag = this;

            _geom.FrictionCoefficient = 0f;
            _geom.RestitutionCoefficient = 0.0f;

            Reset(spawnPos);
        }
        public void Reset(Vector3 spawnPos)
        {
            health = 5;
            corpseTimer = 3000; //Ms
            alive = true;
            isFacingLeft = true;
            range = 100;
            scale = 1f;
            _position = spawnPos;
            _position.Y = -_position.Y;
            _body.Position = new Vector2(_position.X, _position.Y);
            anchorPos = _body.Position;
            turnTimer = 2000;
        }

        public override bool Update(float dTime)
        {
            base.Update(dTime);

            if (!alive)
            {
                rotation = _body.Rotation;
                
                rotationMatrix = Matrix.CreateRotationZ(rotation);
                if (scale > 0.3f)
                {
                    _position.Z++;
                    scale *= 0.98f;
                    //TODO BloodTrail?
                    Game1.Instance.particles.Add(new BulletSpark(Get3Dpos(), Game1.random.Next(100) / 100.0f * 3f));
                    Game1.Instance.particles.Add(new BloodSpat(Get3Dpos(), 0.001f, -_body.LinearVelocity));
                    Game1.Instance.particles.Add(new Smoke(Get3Dpos(), 0.001f));
                }

                corpseTimer -= (int)dTime;
                if (corpseTimer < 0)return false;

                return true;
            }

            //if (!onGround) return true;
            _body.Rotation = 0;
            if (scale > 1) scale -= 0.1f;
            

            if (Math.Abs(anchorPos.X-_body.Position.X) > range)
            {
                if (_body.Position.X > anchorPos.X) isFacingLeft = true;
                if (_body.Position.X < anchorPos.X) isFacingLeft = false;
            }

            Vector2 addVel = new Vector2(100*_body.Mass, 0);
            rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(90));
            if (isFacingLeft)
            {
                rotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(-90));
                addVel.X = -addVel.X;
            }

            rotationMatrix *= Matrix.CreateRotationZ(_body.Rotation);

            if(Math.Abs(_body.LinearVelocity.X)<100) _body.ApplyForce(addVel * dTime);

            if (Game1.random.Next(1000) > 990&& onGround) _body.ApplyImpulse(new Vector2(0, -100 * _body.Mass - Physics.Instance.Gravity.Y));

            return true;
        }


        internal void BulletHit()
        {
            Sounds.PlaySound("hitDamage");

            Vector3 spawnPos = Get3Dpos();
            spawnPos.Y += 10;

            for (int i = 0; i < 5; i++)
            {
                spawnPos.Y += i;
                Game1.Instance.particles.Add(new BloodSpat(spawnPos, 0.001f, -_body.LinearVelocity));
            }

            _body.ApplyForce(new Vector2(0, 100 * _body.Mass));
            scale += 0.4f;
        
            if(!alive) return;

            if (--health <= 0)Kill();
        }

        private void Kill()
        {
            alive = false;
            Sounds.PlaySound("enemyExplode");
            Game1.FaderWhite.Flash(1000);

            //_geom.RestitutionCoefficient = 1;
            //_geom.IgnoreCollisionWith(Game1.Instance.player.GetGeom());

            _body.RotationalDragCoefficient = 10;

            for (int i = 0; i < 20; i++)
            {
                Game1.Instance.particles.Add(new BulletSpark(Get3Dpos(), Game1.random.Next(100) / 100.0f * 3f));
            }

            _body.LinearVelocity = new Vector2(-100 + Game1.random.Next(200), -200-Game1.random.Next(300));
            _body.AngularVelocity = -5 + Game1.random.Next(10);
        }
        public override void Destroy()
        {
            //Game1.physics.Remove(_body);
            //Game1.physics.Remove(_geom);
            Sounds.PlaySound("swap");
            Game1.Instance.enemies.Remove(this);
        }
        public override bool OnCollision(FarseerGames.FarseerPhysics.Collisions.Geom geom1, FarseerGames.FarseerPhysics.Collisions.Geom geom2, FarseerGames.FarseerPhysics.Collisions.ContactList contacts)
        {
            if (!alive) return true;

            FarseerGames.FarseerPhysics.Collisions.Geom another;

            if (geom1 == this._geom)
                another = geom2;
            else
                another = geom1;

            if (another.Tag == null) return true;

            GameObject3D obj = another.Tag as GameObject3D;

            Vector2 force =this._body.Position- another.Body.Position;

            obj.EnemyHit(force);

            return true;
        }
    }
}
