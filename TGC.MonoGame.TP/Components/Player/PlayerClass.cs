using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Components.Player
{
    /// <summary>
    /// Defines the <see cref="PlayerClass" />.
    /// </summary>
    public class PlayerClass
    {
        /// <summary>
        /// Defines the Life.
        /// </summary>
        private float Life { get; set; }

        /// <summary>
        /// Defines the Position.
        /// </summary>
        private Vector3 Position { get; set; }
        
        /// <summary>
        /// Defines de SoulsCollected
        /// </summary>
        private int SoulsCollected { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerClass"/> class.
        /// </summary>
        /// <param name="startPosition">The startPosition<see cref="Vector3"/>.</param>
        public PlayerClass(Vector3 startPosition)
        {
            Life = 100;
            Position = startPosition;
        }

        public void SetPosition(Vector3 newPosition)
        {
            Position = newPosition;
        }

        public Vector3 GetPosition()
        {
            return Position;
        }

        public float GetLife()
        {
            return Life;
        }

        public void LoseLife(float damage)
        {
            Life -= damage;
        }

        public void GainSoul()
        {
            SoulsCollected++;
        }

        public int GetSoulsCollected()
        {
            return SoulsCollected;
        }
    }
}
