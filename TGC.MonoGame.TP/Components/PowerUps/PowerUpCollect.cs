using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.MonoGame.TP.Components.Camera;

namespace TGC.MonoGame.TP.Components.PowerUps
{
    internal class PowerUpCollect
    {
        private Vector3 Position { get; set; }
        private Vector3 Direction { get; set; }
        private Vector3 Up { get; set; }
        private Texture2D Texture { get; set; }
        private Model PowerUpModel { get; set; }
        private Effect PowerUpEffect { get; set; }
        private Vector3 LightPosition { get; set; }
        public PowerUpCollect(Model weaponModel, Effect weaponEffect, Texture2D weaponTexture, Vector3 position, Vector3 lightPosition)
        {
            this.PowerUpModel = weaponModel;
            this.Texture = weaponTexture;
            this.PowerUpEffect = weaponEffect;
            this.LightPosition = lightPosition;
            this.PowerUpEffect.Parameters["baseTexture"]?.SetValue(weaponTexture);
            // La luz y el material se configuran de forma centralizada en SceneLight.
            this.Position = position;
        }

        public void Update()
        {

        }

        public void Draw(Matrix View, Matrix Projection, GameTime gameTime, FreeCamera camera, bool hasBones)
        {
            Matrix weaponWorld = Matrix.CreateScale(3.5f) * Matrix.CreateRotationX(-MathF.PI / 2) * Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds) * Matrix.CreateWorld(new Vector3(this.Position.X, 15 * MathF.Sin((float)gameTime.TotalGameTime.TotalSeconds) - 25, this.Position.Z), Vector3.UnitZ, Vector3.UnitY);

            if (hasBones)
            {
                weaponWorld = Matrix.CreateScale(3.5f) * Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds) * Matrix.CreateWorld(new Vector3(this.Position.X, 15 * MathF.Sin((float)gameTime.TotalGameTime.TotalSeconds) - 25, this.Position.Z), Vector3.UnitX, Vector3.UnitY);
            }

            foreach (ModelMesh mesh in PowerUpModel.Meshes)
            {
                foreach (ModelMeshPart modelMeshPart in mesh.MeshParts)
                {
                    this.PowerUpEffect.Parameters["baseTexture"]?.SetValue(this.Texture);
                    this.PowerUpEffect.Parameters["World"].SetValue(hasBones ? mesh.ParentBone.ModelTransform * weaponWorld : weaponWorld);
                    this.PowerUpEffect.Parameters["View"].SetValue(View);
                    this.PowerUpEffect.Parameters["Projection"]?.SetValue(Projection);
                    this.PowerUpEffect.Parameters["eyePosition"]?.SetValue(camera.Position);
                    modelMeshPart.Effect = this.PowerUpEffect;
                }

                mesh.Draw();
            }
        }
    }
}
