using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DemoBaseXNA;
using DemoBaseXNA.DrawingSystem;

namespace Platformer3D
{
    public class Physics : PhysicsSimulator
    {
        public static PhysicsSimulatorView physicsSimulatorView;
        public static PhysicsSimulator Instance;
        public Physics(Vector2 newGravity)
        {
            if (Instance != null) throw new Exception("Physics engine instance doubled!");
            Physics.Instance = this;

            this.setGravity(newGravity);
            BiasFactor = 0.01f;
            //AllowedPenetration = 0.01f;
            this.Iterations = 50;
            this.MaxContactsToResolve = 3;
            physicsSimulatorView = new DemoBaseXNA.PhysicsSimulatorView(this);
            // physicsSimulatorView.EnableEdgeView = false;
            physicsSimulatorView.LoadContent(Game1.Instance.GraphicsDevice, Game1.Instance.Content);
        }
        public void Draw(SpriteBatch sb)
        {
            this.EnableDiagnostics = true;
            physicsSimulatorView.Draw(sb);
            //EnableDiagnostics = false;
        }
        public void setGravity(Vector2 newGravity)
        {
            this.Gravity = newGravity;
        }
        public override void Update(float dt)
        {
            base.Update(dt);
        }
    }
}
