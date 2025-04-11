using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Components.Weapons
{
    class RocketLauncher
    {
        private Vector3 Position { get; set; }
        private Vector3 Scale { get; set; }

        public RocketLauncher()
        {
            Position = new Vector3();
            Scale = Vector3.One;
        }

        /// <summary>
        /// Rocket launcher constructor which needs a position and a scale
        /// </summary>
        /// <param name="Position"></param>
        /// <param name="Scale"></param>
        public RocketLauncher(Vector3 Position, Vector3 Scale)
        {
            this.Position = Position;
            this.Scale = Scale;
        }

        public void Update()
        {

        }

        /// <summary>
        /// Renders a rocket launcher model
        /// </summary>
        /// <param name="rocketLauncherModel"></param>
        /// <param name="View"></param>
        /// <param name="Projection"></param>
        /// <param name="totalSeconds"></param>        
        public void Draw(Model rocketLauncherModel, Matrix View, Matrix Projection, float totalSeconds)
        {
            Matrix shotgunWorld = Matrix.CreateScale(this.Scale) * Matrix.CreateRotationY(totalSeconds) * Matrix.CreateWorld(new Vector3(this.Position.X, 15 * MathF.Sin(totalSeconds) - 25, this.Position.Z), Vector3.UnitZ, Vector3.UnitY);
            rocketLauncherModel.Draw(shotgunWorld, View, Projection);
        }
    }
}
