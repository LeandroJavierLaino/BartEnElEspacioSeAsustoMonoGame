using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TGC.MonoGame.TP.Components.Camera;
using BulletEntity = TGC.MonoGame.TP.Components.Bullet.Bullet;

namespace TGC.MonoGame.TP.Components.Weapons
{
    /// <summary>
    /// Define el comportamiento de disparo de un arma (cuantas balas, en que boton y con que recoil).
    /// Cada arma delega aqui para no duplicar la logica de disparo en WeaponFPS.
    /// </summary>
    interface IWeaponFireMode
    {
        /// <summary>
        /// Procesa el input de disparo. Si se dispara, agrega balas a la lista y devuelve el recoil a aplicar.
        /// Devuelve 0 si no hubo disparo en este frame.
        /// </summary>
        float Update(MouseState mouse, FreeCamera camera, List<BulletEntity> bullets, float currentRecoil);
    }

    /// <summary>
    /// Helpers compartidos por los modos de disparo.
    /// </summary>
    static class FireModeHelpers
    {
        /// <summary>
        /// Crea una bala orientada segun la camara, desplazada por el offset indicado.
        /// </summary>
        public static BulletEntity CreateBullet(FreeCamera camera, Vector3 offset)
        {
            BulletEntity bullet = new BulletEntity();
            bullet.SetPosition(camera.Position + offset);
            bullet.SetDirection(camera.FrontDirection);
            bullet.SetUp(camera.UpDirection);
            return bullet;
        }
    }

    /// <summary>
    /// Modo de la Shotgun: click izquierdo dispara una bala (recoil normal),
    /// click derecho dispara dos balas (recoil mas profundo). Ambos son gatillados.
    /// </summary>
    class ShotgunFireMode : IWeaponFireMode
    {
        // Recoil del disparo simple (click izquierdo).
        private const float PrimaryRecoil = 5f;
        // Recoil del disparo doble (click derecho), mas profundo.
        private const float SecondaryRecoil = 6.75f;

        // Evita que se repita el disparo mientras se mantiene presionado el boton.
        private bool clickPressed;

        public float Update(MouseState mouse, FreeCamera camera, List<BulletEntity> bullets, float currentRecoil)
        {
            bool canFire = currentRecoil == 0;
            float recoil = 0f;

            // Disparo primario (click izquierdo): una sola bala.
            if (mouse.LeftButton == ButtonState.Pressed && !clickPressed && canFire)
            {
                bullets.Add(FireModeHelpers.CreateBullet(camera, camera.FrontDirection * 25f));
                recoil = PrimaryRecoil;
                clickPressed = true;
            }
            // Disparo secundario (click derecho): dos balas.
            else if (mouse.RightButton == ButtonState.Pressed && !clickPressed && canFire)
            {
                bullets.Add(FireModeHelpers.CreateBullet(camera, camera.FrontDirection * 50f));
                bullets.Add(FireModeHelpers.CreateBullet(camera, Vector3.Up * 5 + camera.FrontDirection * 25f));
                recoil = SecondaryRecoil;
                clickPressed = true;
            }

            // Liberamos el gatillo cuando se sueltan ambos botones.
            if (mouse.LeftButton == ButtonState.Released && mouse.RightButton == ButtonState.Released)
            {
                clickPressed = false;
            }

            return recoil;
        }
    }

    /// <summary>
    /// Modo de un solo disparo gatillado (click izquierdo). Hay que soltar y volver a presionar
    /// para disparar de nuevo. El recoil es configurable (profundo para el RocketLauncher,
    /// intermedio para el GranadeLauncher).
    /// </summary>
    class SingleFireMode : IWeaponFireMode
    {
        private readonly float recoilAmount;
        private bool clickPressed;

        public SingleFireMode(float recoilAmount)
        {
            this.recoilAmount = recoilAmount;
        }

        public float Update(MouseState mouse, FreeCamera camera, List<BulletEntity> bullets, float currentRecoil)
        {
            float recoil = 0f;

            if (mouse.LeftButton == ButtonState.Pressed && !clickPressed && currentRecoil == 0)
            {
                bullets.Add(FireModeHelpers.CreateBullet(camera, camera.FrontDirection * 25f));
                recoil = recoilAmount;
                clickPressed = true;
            }

            if (mouse.LeftButton == ButtonState.Released)
            {
                clickPressed = false;
            }

            return recoil;
        }
    }

    /// <summary>
    /// Modo de disparo continuo (Machinegun): mientras se mantiene presionado el click izquierdo,
    /// dispara una bala tras otra en cuanto el recoil se recupera. El recoil corto produce una
    /// cadencia alta sin necesidad de soltar el boton.
    /// </summary>
    class ContinuousFireMode : IWeaponFireMode
    {
        private readonly float recoilAmount;

        public ContinuousFireMode(float recoilAmount)
        {
            this.recoilAmount = recoilAmount;
        }

        public float Update(MouseState mouse, FreeCamera camera, List<BulletEntity> bullets, float currentRecoil)
        {
            // Sin gatillo: dispara cada vez que el recoil llega a 0 y el boton sigue presionado.
            if (mouse.LeftButton == ButtonState.Pressed && currentRecoil == 0)
            {
                bullets.Add(FireModeHelpers.CreateBullet(camera, camera.FrontDirection * 25f));
                return recoilAmount;
            }

            return 0f;
        }
    }
}
