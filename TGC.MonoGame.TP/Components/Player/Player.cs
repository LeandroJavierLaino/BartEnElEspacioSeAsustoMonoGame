namespace TGC.MonoGame.TP.Components.Player
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using TGC.MonoGame.TP.Components.Weapons;

    /// <summary>
    /// Defines the <see cref="Player" />.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="Player"/> class.
    /// </remarks>
    /// <param name="startPosition">The startPosition<see cref="Vector3"/>.</param>
    internal class Player(Vector3 startPosition)
    {
        /// <summary>
        /// Defines the Life.
        /// </summary>
        private int Life { get; set; } = 100;

        /// <summary>
        /// Defines the Position.
        /// </summary>
        private Vector3 Position { get; set; } = startPosition;
        private List<WeaponFPS> Weapons { get; set; } = [];

        public void SetPosition(Vector3 newPosition)
        {
            Position = newPosition;
        }

        public Vector3 GetPosition() 
        {
            return Position;
        }

        public void SetLife(int newLife)
        {
            Life = newLife;
        }

        public int GetLife()
        {
            return Life;
        }   

        public void AddWeapon(WeaponFPS weapon)
        {        
            Weapons.Add(weapon);
        }
    }
}
