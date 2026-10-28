using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TGC.MonoGame.TP.Components.Camera;
using BulletEntity = TGC.MonoGame.TP.Components.Bullet.Bullet;

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
        private bool IsMachinegun { get; set; }
        private float Recoil { get; set; }
        // Progreso de la animacion de aparicion del arma (0 = abajo/oculta, 1 = en posicion final).
        private float SwitchProgress { get; set; }
        private Model WeaponModel { get; set; }
        private Effect WeaponEffect { get; set; }
        private Texture2D Texture { get; set; }

        // Posicion previa de la camara para detectar si el jugador se esta moviendo.
        private Vector3 PreviousCameraPosition { get; set; }
        // Fase de la oscilacion lateral del arma.
        private float SwayTime { get; set; }
        // Intensidad actual del sway (0 = centrado, 1 = sway completo). Se suaviza al entrar/salir del movimiento.
        private float SwayAmount { get; set; }

        // Amplitud maxima del desplazamiento lateral del arma, en unidades.
        private const float MaxSway = 16f;
        // Amplitud maxima del desplazamiento vertical del arma (bob), en unidades. Combinado con el lateral da un movimiento en forma de 8.
        private const float MaxVerticalSway = 18f;
        // Velocidad de la oscilacion lateral.
        private const float SwaySpeed = 4.5f;
        // Velocidad de suavizado del sway al entrar/salir del movimiento.
        private const float SwaySmoothing = 8f;
        // Retroceso maximo del arma sobre el eje Z (hacia atras) durante el recoil.
        private const float RecoilZ = 50f;
        // Exponente de moldeado de la onda de sway (>1 = mas lento en el centro y mas rapido hacia los lados).
        private const float SwaySharpness = 1f;

        // Velocidad a la que el recoil decae cada frame hasta volver a 0.
        private const float RecoilRecovery = 0.25f;
        // Cantidad maxima de balas vivas antes de limpiar la lista.
        private const int MaxBullets = 15;

        // Modo de disparo del arma (define cuantas balas, en que boton y con que recoil).
        private IWeaponFireMode FireMode { get; set; }

        // Angulo acumulado de giro del canon de la machinegun (rotacion sobre el eje Z del mesh 1).
        private float BarrelSpin { get; set; }
        // Indice del mesh que gira en la machinegun.
        private const int MachinegunSpinMeshIndex = 1;
        // Velocidad de giro del canon (rad/seg) mientras se dispara.
        private const float BarrelSpinFiringSpeed = 24f;
        // Que tan rapido la velocidad de giro se acerca a su objetivo (mayor = acelera/frena mas rapido).
        private const float BarrelSpinAcceleration = 1.4f;
        // Velocidad de giro actual del canon, se suaviza hacia el objetivo para dar inercia (ease-in / ease-out).
        private float BarrelSpinSpeed { get; set; }
        // Desplazamiento del canon sobre el eje X (hacia adelante) de la machinegun.
        private const float BarrelForwardOffset = 0f;

        public WeaponFPS(Model weaponModel, Effect weaponEffect, Texture2D weaponTexture, WeaponKeys activationKey, IWeaponFireMode fireMode)
        {
            this.WeaponModel = weaponModel;
            this.WeaponEffect = weaponEffect;
            this.Texture = weaponTexture;
            this.IsActive = false;
            this.ActivationKey = activationKey;
            this.FireMode = fireMode;
            this.WeaponEffect.Parameters["baseTexture"]?.SetValue(weaponTexture);
            // La luz y el material se configuran de forma centralizada en SceneLight.
            this.Recoil = 0;
        }

        public void SetActive(bool isActive)
        {
            this.IsActive = isActive;
        }

        public void Update(FreeCamera camera, List<BulletEntity> bullets, GameTime gameTime)
        {
            Vector3 CameraPosition = camera.Position;
            this.Position = CameraPosition;

            // El disparo y el recoil solo se procesan para el arma que esta activa.
            if (this.IsActive)
            {
                UpdateShooting(camera, bullets);
            }

            // El recoil decae de forma continua hasta volver a 0.
            if (this.Recoil > 0)
            {
                this.Recoil -= RecoilRecovery;

                if (this.Recoil < 0)
                {
                    this.Recoil = 0;
                }
            }

            // Guardamos el estado previo para detectar la transicion de inactiva -> activa.
            bool wasActive = this.IsActive;

            Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();

            // Recorremos las teclas presionadas y solo reaccionamos ante las que están definidas en WeaponKeys.
            foreach (Keys key in pressedKeys)
            {
                // Ignoramos cualquier tecla que no corresponda a un arma configurada.
                if (!Enum.IsDefined(typeof(WeaponKeys), (int)key))
                {
                    continue;
                }

                WeaponKeys pressedWeaponKey = (WeaponKeys)key;

                // Vemos si el arma activa es la Machinegun para aplicar un transform por bones.
                if (pressedWeaponKey == WeaponKeys.Machinegun)
                {
                    this.IsMachinegun = true;
                }
                else
                {
                    this.IsMachinegun = false;
                }

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

            // Detectamos si el jugador se esta moviendo comparando la posicion de la camara.
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector3 horizontalDelta = new Vector3(CameraPosition.X - this.PreviousCameraPosition.X, 0f, CameraPosition.Z - this.PreviousCameraPosition.Z);
            bool isMoving = horizontalDelta.LengthSquared() > 0.0001f;
            this.PreviousCameraPosition = CameraPosition;

            // Suavizamos la intensidad del sway: sube hacia 1 al moverse, baja hacia 0 al detenerse (vuelve al centro).
            float targetSway = isMoving ? 1f : 0f;
            this.SwayAmount = MathHelper.Lerp(this.SwayAmount, targetSway, Math.Clamp(SwaySmoothing * deltaTime, 0f, 1f));

            // Avanzamos la fase de la oscilacion solo mientras hay movimiento.
            if (isMoving)
            {
                this.SwayTime += deltaTime * SwaySpeed;
            }

            // Aplicar retroceso en la direccion Z (hacia atras) basado en el valor de recoil.
            if (this.Recoil > 0)
            {
                float recoilValue = MathHelper.Clamp(this.Recoil, 0f, 1f) * RecoilZ;
                this.Position -= new Vector3(0, 0, recoilValue);
            }

            // Animacion de giro del canon de la machinegun con inercia: la velocidad de giro acelera (ease-in)
            // al empezar a disparar y desacelera (ease-out) al soltar, en vez de arrancar/frenar de golpe.
            if (this.IsMachinegun)
            {
                bool firing = this.IsActive && Mouse.GetState().LeftButton == ButtonState.Pressed;
                float targetSpeed = firing ? BarrelSpinFiringSpeed : 0f;

                // Suavizamos la velocidad actual hacia la objetivo para dar sensacion de inercia.
                this.BarrelSpinSpeed = MathHelper.Lerp(this.BarrelSpinSpeed, targetSpeed, Math.Clamp(BarrelSpinAcceleration * deltaTime, 0f, 1f));

                this.BarrelSpin += deltaTime * this.BarrelSpinSpeed;

                // Mantenemos el angulo acotado para evitar que crezca indefinidamente.
                this.BarrelSpin %= MathHelper.TwoPi;
            }
        }

        private void UpdateShooting(FreeCamera camera, List<BulletEntity> bullets)
        {
            MouseState mouse = Mouse.GetState();

            // Si se alcanzo el limite de balas vivas, limpiamos la lista.
            if (bullets.Count >= MaxBullets)
            {
                bullets.Clear();
            }

            // No permitimos nuevos disparos si ya se alcanzo el limite de balas.
            if (bullets.Count >= MaxBullets)
            {
                return;
            }

            // Delegamos la logica de disparo al modo del arma. Devuelve el recoil a aplicar (0 si no disparo).
            float firedRecoil = this.FireMode.Update(mouse, camera, bullets, this.Recoil);

            if (firedRecoil > 0f)
            {
                this.Recoil = firedRecoil;
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

                // Retroceso sobre el eje Z (hacia atras, en direccion opuesta al frente de la camara) segun el recoil.
                float recoilOffset = RecoilZ * Recoil;

                // Desplazamiento lateral del arma (sway): oscila de lado a lado mientras el jugador se mueve.
                // Moldeamos la onda con un exponente para que el arma vaya mas lenta en el centro y acelere hacia los extremos.
                float rawSin = MathF.Sin(SwayTime);
                float shapedSin = MathF.Sign(rawSin) * MathF.Pow(MathF.Abs(rawSin), SwaySharpness);
                float lateralSway = shapedSin * MaxSway * SwayAmount;
                // Desplazamiento vertical: usamos el valor absoluto del seno (siempre hacia abajo) para trazar un semicirculo inferior (forma de U) en vez de un 8.
                float verticalSway = MathF.Pow(MathF.Abs(rawSin), SwaySharpness) * MaxVerticalSway * SwayAmount;

                Vector3 weaponPosition = new Vector3(camera.Position.X, 0, camera.Position.Z) + new Vector3(0, -25, 0) + camera.FrontDirection * MathHelper.Lerp(20, 10, Recoil) - camera.FrontDirection * recoilOffset + cameraRight * 25 + cameraRight * lateralSway - camera.UpDirection * 4 + camera.UpDirection * switchOffset - camera.UpDirection * verticalSway;

                Matrix weaponWorld = Matrix.CreateScale(3.5f) * Matrix.CreateRotationX(-MathF.PI / 2) * Matrix.CreateWorld(weaponPosition, -cameraRight, camera.UpDirection);

                if(IsMachinegun)
                {
                    weaponWorld = Matrix.CreateScale(3.5f) * Matrix.CreateWorld(weaponPosition, -cameraRight, camera.UpDirection);
                }

                for (int meshIndex = 0; meshIndex < WeaponModel.Meshes.Count; meshIndex++)
                {
                    ModelMesh mesh = WeaponModel.Meshes[meshIndex];

                    // Para la machinegun, el mesh del canon (indice 1) gira sobre su eje X.
                    Matrix meshWorld = weaponWorld;
                    if (IsMachinegun)
                    {
                        Matrix boneTransform = mesh.ParentBone.ModelTransform;

                        if (meshIndex == MachinegunSpinMeshIndex)
                        {
                            boneTransform = Matrix.CreateRotationX(BarrelSpin) * Matrix.CreateTranslation(BarrelForwardOffset, 0, 0) * boneTransform;
                        }

                        meshWorld = boneTransform * weaponWorld;
                    }

                    foreach (ModelMeshPart modelMeshPart in mesh.MeshParts)
                    {
                        this.WeaponEffect.Parameters["baseTexture"]?.SetValue(this.Texture);
                        this.WeaponEffect.Parameters["World"].SetValue(meshWorld);
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
