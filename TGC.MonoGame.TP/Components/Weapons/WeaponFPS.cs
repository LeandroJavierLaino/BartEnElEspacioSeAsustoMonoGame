using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using TGC.MonoGame.TP.Components.Camera;

enum WeaponKeys
{
    Shotgun = Keys.D1,
    Machinegun = Keys.D2,
    RocketLauncher = Keys.D3,
    GranadeLauncher = Keys.D4,
}

namespace TGC.MonoGame.TP.Components.Weapons
{
    class WeaponFPS
    {
        private Vector3 Position {  get; set; }
        private int Ammo { get; set; }
        private WeaponKeys ActivationKey { get; set; }
        private bool IsActive { get; set; }
        private bool IsSwitching { get; set; }
        private float Recoil { get; set; }
        // Progreso de la animacion de aparicion del arma (0 = abajo/oculta, 1 = en posicion final).
        private float SwitchProgress { get; set; }
        private Model WeaponModel { get; set; }
        private Effect WeaponEffect { get; set; }
        private Texture2D Texture { get; set; }

        public WeaponFPS(Model weaponModel, Effect weaponEffect,Texture2D weaponTexture, WeaponKeys activationKey)
        {
            this.WeaponModel = weaponModel;
            this.WeaponEffect = weaponEffect;
            this.Texture = weaponTexture;
            this.IsActive = false;
            this.ActivationKey = activationKey;
            this.WeaponEffect.Parameters["baseTexture"]?.SetValue(weaponTexture);
            // La luz y el material se configuran de forma centralizada en SceneLight.
            this.Recoil = 0;
        }

        public void SetActive(bool isActive)
        {
            this.IsActive = isActive;
        }

        public void Update(Vector3 CameraPosition, float Recoil, GameTime gameTime)
        {
            this.Position = CameraPosition;
            this.Recoil = Recoil;

            // Guardamos el estado previo para detectar la transicion de inactiva -> activa.
            bool wasActive = this.IsActive;

            Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();

            // Recorremos las teclas presionadas y solo reaccionamos ante las que estan definidas en WeaponKeys.
            foreach (Keys key in pressedKeys)
            {
                // Ignoramos cualquier tecla que no corresponda a un arma configurada.
                if (!Enum.IsDefined(typeof(WeaponKeys), (int)key))
                {
                    continue;
                }

                WeaponKeys pressedWeaponKey = (WeaponKeys)key;

                // Se activa si la tecla presionada es la de esta arma, se desactiva si es la de otra arma.
                this.IsActive = pressedWeaponKey == this.ActivationKey;
            }

            // Si el arma acaba de activarse, arrancamos la animacion de aparicion desde abajo.
            if (this.IsActive && !wasActive)
            {
                this.IsSwitching = true;
                this.SwitchProgress = 0f;
            }

            // Avanzamos la animacion de aparicion mientras este en curso.
            if (this.IsSwitching)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Velocidad de la animacion: ~0.25s para aparecer por completo.
                this.SwitchProgress += elapsed / 0.25f;

                if (this.SwitchProgress >= 1f)
                {
                    this.SwitchProgress = 1f;
                    this.IsSwitching = false;
                }
            }
        }

        public void Draw(Matrix View, Matrix Projection, FreeCamera camera)
        {
            if (this.IsActive)
            {
                Vector3 cameraRight = Vector3.Cross(camera.FrontDirection, camera.UpDirection);

                // Suavizamos el progreso (ease-out) para que la aparicion desacelere al llegar arriba.
                float easedProgress = 1f - (1f - this.SwitchProgress) * (1f - this.SwitchProgress);

                // Desplazamiento vertical: arranca abajo (fuera de la vista) y sube hasta la posicion final.
                float switchOffset = MathHelper.Lerp(-150f, 0f, easedProgress);

                Vector3 weaponPosition = new Vector3(camera.Position.X, 0, camera.Position.Z) + new Vector3(0, -25, 0) + camera.FrontDirection * MathHelper.Lerp(80, 65, Recoil) + cameraRight * 25 - camera.UpDirection * 4 + camera.UpDirection * switchOffset;
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
}
