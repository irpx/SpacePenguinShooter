using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Collisions;

namespace Platformer3D
{
    public class Player : GameObject3D
    {
        public bool isFacingLeft;

        private bool moved;
        private int jumpTimer;
        private int footStepTimer;
        private int shootTimer = 0;
        private Vector2 shootDirection;
        private float acceleration;
        private bool jumpHold;
        public int health;
        private int hitTimer;
        public Player()
        {
            health = 100;
            hitTimer = 0;
            acceleration = 500f;
            footStepTimer = 0;
            shootDirection = new Vector2(1, 0);
            isFacingLeft = true;
            _model = Game1.Instance.Content.Load<Model>("Models\\bot");
            boneTransforms = new Matrix[_model.Bones.Count];

            //scale = 10.0f;
            //_body = BodyFactory.Instance.CreateCircleBody(Game1.physics, 10.0f, 1.0f);
            _body = BodyFactory.Instance.CreateRectangleBody(Game1.physics, 10.0f, 20.0f, 10.0f);
            //_geom = GeomFactory.Instance.CreateCircleGeom(Game1.physics, _body, 10.0f, 12);
            _geom = GeomFactory.Instance.CreateRectangleGeom(Game1.physics, _body,10.0f, 20.0f,new Vector2(0,-10),0);
            
            _geom.FrictionCoefficient = 0f;
            _geom.RestitutionCoefficient = 0f;

            _geom.Tag = this;

            _body.RotationalDragCoefficient = 1.0f;
            _body.LinearDragCoefficient = 0;

            _geom.CollisionCategories = FarseerGames.FarseerPhysics.CollisionCategory.Cat1;

            _geom.OnSeparation += OnSeparation;
            _geom.OnCollision += OnCollision;
            Game1.physics.BroadPhaseCollider.OnBroadPhaseCollision += OnBroadPhaseCollision;
        }

        public override bool OnCollision(Geom geom1, Geom geom2, ContactList contacts)
        {
            //onGround = false;

            contacts.ForEach(delegate(Contact contact)
            {
                if (contact.Normal.Y > Math.Abs(contact.Normal.X))
                {
                    //if (!onGround) Game1.sounds.PlaySound("snap");
                    onGround = true;
                }
            });
            return true;
        }

        public override void OnSeparation(Geom geom1, Geom geom2)
        {
            onGround = false;
            //Game1.sounds.PlaySound("swap");
            //TODO
            return;
        }

        public override bool OnBroadPhaseCollision(Geom geom1, Geom geom2)
        {
            return true;
        }

        public override bool Update(float dTime)
        {
            base.Update(dTime);

            rotationMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(90));
            rotationMatrix *= Matrix.CreateRotationZ(-_body.Rotation);

            moved = false;

            if (Input.pad.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Start)||Input.key.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                Reset();
            }

            if (Input.buttonPressedFirsttime(Microsoft.Xna.Framework.Input.Buttons.LeftShoulder)) Jump(500.0f);
            //if (Input.buttonPressedFirsttime(Microsoft.Xna.Framework.Input.Buttons.RightShoulder)) shoot();
            if (Input.pad.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.RightShoulder) && shootTimer <= 0)
                shoot();
            else
                shootTimer -= (int)(dTime/(Game1.bulletTime*0.7f));

            //Game1.Instance.debugTexts.Add("INPUT&VEL:" + Input.pad.ThumbSticks.Left.LengthSquared() + "/" + velocity.LengthSquared());
            if (Input.pad.ThumbSticks.Left.LengthSquared()>0.1f)
            {
                rotation = (float)Math.Atan2(Input.pad.ThumbSticks.Left.Y, Input.pad.ThumbSticks.Left.X);
                Vector2 addVel = new Vector2(Input.pad.ThumbSticks.Left.X*acceleration, 0);

                //Game1.Instance.debugTexts.Add(Math.Abs(_body.LinearVelocity.X).ToString() + "/" + Math.Abs(-_body.LinearVelocity.X - addVel.X).ToString());

                if (Math.Abs(_body.LinearVelocity.X) < 100f)
                {
                    _body.ApplyForce(addVel * dTime);
                }
                else if(_body.LinearVelocity.X > 0 && addVel.X<0)
                {
                    _body.ApplyForce(addVel * dTime);
                }
                else if (_body.LinearVelocity.X < 0 && addVel.X > 0)
                {
                    _body.ApplyForce(addVel * dTime);
                }

                if (footStepTimer <= 0)
                {
                    footStepTimer = 500;
                    if(onGround) Sounds.PlaySound("footStep");
                }
            }
            footStepTimer -= (int)dTime;
            if (Input.pad.ThumbSticks.Right.LengthSquared() > 0.5f)
            {
                shootDirection = Input.pad.ThumbSticks.Right;
                shootDirection.Normalize();
            }

            if (jumpHold) if (!Input.pad.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.LeftShoulder)) jumpHold = false;

            if (jumpTimer > 0)
            {
                jumpTimer -= (int)dTime;
                //Add to jump
                if (jumpHold) _body.ApplyForce(new Vector2(0, -5f * dTime * _body.Mass));
                //Stop if playing
                Sounds.StopSound("jetpack","jetpack_stop");
            }
            else
            {
                //Check for jetpack.
                if (Input.pad.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.LeftShoulder)&&!jumpHold&&!onGround)
                {
                    float yvar;
                    if (Input.pad.ThumbSticks.Left.Y == 0)
                        yvar = (_body.Mass * _body.LinearVelocity.Y)/0.5f;
                    else
                        yvar = Input.pad.ThumbSticks.Left.Y * acceleration*2;

                    float xvar = 0f;
                    if (Input.pad.ThumbSticks.Left.X == 0) xvar = -_body.Mass * _body.LinearVelocity.X;
                    
                    _body.ApplyForce(new Vector2(xvar, -yvar +_body.Mass*-Game1.physics.Gravity.Y));
                    
                    Game1.Instance.particles.Add(new Smoke(Get3Dpos(), 0.1f));

                    Sounds.LoopSound("jetpack","jetpack_start");
                }
                else
                {
                    Sounds.StopSound("jetpack","jetpack_stop");
                }
            }

            if (!moved)
            {
                //_geom.FrictionCoefficient = 1000f;
                _body.AngularVelocity = 0f;
                _body.Rotation = 0.0f;
                if (onGround)
                {
                    _body.LinearVelocity *= 0.9f;// Vector2.Zero;
                    if(jumpTimer<=0) _body.LinearVelocity.Y = 0;
                }

                if (velocity.X < 0)
                    isFacingLeft = true;
                else
                    isFacingLeft = false;
            }
            else
            {
                //_geom.FrictionCoefficient = 0f;
            }
            
            Game1.Instance.debugTexts.Add("Player isFacingLeft:" + isFacingLeft.ToString());
            Game1.Instance.debugTexts.Add("LinearVelocity:" + _body.LinearVelocity.ToString());
            Game1.Instance.debugTexts.Add("input:" + Input.pad.ThumbSticks.Left.ToString());

            return true;
        }

        private void shoot()
        {
            shootTimer = 50;
            Sounds.PlaySound("shootBullet");
            Sounds.PlaySound("shootLaser");
            Game1.Instance.camera.Shake();
            //Game1.sounds.PlaySound("rocket");
            Vector3 newPos = Get3Dpos();
            newPos.Y += 10;
            Vector2 speed = shootDirection;
            newPos += new Vector3(speed.X, speed.Y, 0f) * 13;
            speed.Y = -speed.Y;
            speed *= 500f;
            Game1.Instance.bullets.Add(new Bullet1(speed,newPos));
        }

        public override void Jump(float force)
        {
            if (onGround)
            {
                Sounds.PlaySound("jump");
                jumpHold = true;
                moved = true;
                onGround = false;
                jumpTimer = 500;
                _body.ApplyImpulse(new Vector2(0, -100 * _body.Mass - Physics.Instance.Gravity.Y));
            }
        }

        internal Geom GetGeom()
        {
            return _geom;
        }

        public override void EnemyHit(Vector2 force)
        {
            Vector3 spawnPos = Get3Dpos();
            spawnPos.Y += 10;

            Game1.Instance.particles.Add(new BloodSpat(spawnPos, 0.001f, force));

            if (--hitTimer <= 0)
            {
                rotationMatrix *= Matrix.CreateRotationZ(MathHelper.ToRadians(55));
                _body.ApplyImpulse(-force*_body.Mass*50);
                Sounds.PlaySound("hitDamage");
                hitTimer = 10;
            }
            if (--health <= 0) this.Reset();
        }

        private void Reset()
        {
            health = 100;
            hitTimer = 0;
            velocity = new Vector2(0, 0);
            _body.Position = new Vector2(400, 0);
            for (int i = 0; i < 30; i++)
            {
                Game1.Instance.particles.Add(new BulletSpark(Get3Dpos(), Game1.random.Next(100) / 100.0f * 2));
                Game1.Instance.particles.Add(new Smoke(Get3Dpos(), 2f * (float)Game1.random.NextDouble()));
            }
            Sounds.PlaySound("powerup");
        }
    }
}
