using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TGC.MonoGame.TP.Components.Camera;

namespace TGC.MonoGame.TP.Components.Weapons
{
    class WeaponFPS
    {
        private Vector3 Position {  get; set; }
        private Vector3 Direction { get; set; }
        private Vector3 Up {  get; set; }
        private Vector3 LightPosition { get; set; }
        private int Ammo { get; set; }
        private float Recoil { get; set; }
        private Model WeaponModel { get; set; }
        private Effect WeaponEffect { get; set; }
        private Texture2D Texture { get; set; }

        public WeaponFPS(Model weaponModel, Effect weaponEffect,Texture2D weaponTexture, Vector3 lightPosition) 
        {
            this.WeaponModel = weaponModel;
            this.WeaponEffect = weaponEffect;
            this.Texture = weaponTexture;
            this.LightPosition = lightPosition;
            this.WeaponEffect.Parameters["baseTexture"]?.SetValue(weaponTexture);
            // La luz y el material se configuran de forma centralizada en SceneLight.
            this.Recoil = 0;
        }

        public void Update(Vector3 CameraPosition, float Recoil)
        {
            this.Position = CameraPosition;
            this.Recoil = Recoil;
        }

        public void Draw(Matrix View, Matrix Projection, FreeCamera camera)
        {
            Vector3 cameraRight = Vector3.Cross(camera.FrontDirection, camera.UpDirection);
            Vector3 weaponPosition = new Vector3(camera.Position.X, 0, camera.Position.Z) + new Vector3(0, -25, 0) + camera.FrontDirection * MathHelper.Lerp(80, 65, Recoil) + cameraRight * 25 - camera.UpDirection * 4;
            Matrix weaponWorld = Matrix.CreateScale(3.5f) * Matrix.CreateRotationX(-MathF.PI / 2) * Matrix.CreateWorld(weaponPosition, -cameraRight, camera.UpDirection);

            foreach (ModelMesh mesh in WeaponModel.Meshes)
            {
                foreach (ModelMeshPart modelMeshPart in mesh.MeshParts)
                {
                    this.WeaponEffect.Parameters["baseTexture"]?.SetValue(this.Texture);
                    this.WeaponEffect.Parameters["World"].SetValue(weaponWorld);
                    this.WeaponEffect.Parameters["View"].SetValue(View);
                    this.WeaponEffect.Parameters["Projection"]?.SetValue(Projection);
                    this.WeaponEffect.Parameters["eyePosition"]?.SetValue(camera.Position);
                    modelMeshPart.Effect = this.WeaponEffect;
                }

                mesh.Draw();
            }
        }
    }
}
