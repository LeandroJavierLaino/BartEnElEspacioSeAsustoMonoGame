using Microsoft.Xna.Framework;

namespace TGC.MonoGame.TP.Components.Spawner
{
    class Spawner
    {
        private int enemies = 15;
        private Vector3 Position { get; set; }
        private float timer = 300;
        private Vector3[] Waypoints;
        private int waypointPosition = 0;
        private const float VELOCITY = 1.65f;

        public void SetPosition(Vector3 Position)
        {
            int maxDistance = 2500;
            float newPositionX = Position.X + maxDistance;
            float newPositionZ = Position.Z + maxDistance;
            Vector3 position2D0 = new Vector3(newPositionX, Position.Y, Position.Z);
            Vector3 position2D1 = new Vector3(newPositionX, Position.Y, newPositionZ);
            Vector3 position2D2 = new Vector3(Position.X, Position.Y, newPositionZ);
            Vector3 position2D3 = new Vector3(Position.X, Position.Y, Position.Z);

            Waypoints = new Vector3[] { position2D0, position2D1, position2D2, position2D3 };
            this.Position = Position;
        }

        public void UpdatePosition()
        {
            if(waypointPosition > 3)
            {
                waypointPosition = 0;
            }
            if(Vector3.Distance(Position,Waypoints[waypointPosition]) > 0.4)
            {
                Position += Vector3.Normalize(Waypoints[waypointPosition] - Position) * VELOCITY;
            }
            else
            {
                waypointPosition += 1;
            }
        }

        public Vector3 GetPosition()
        {
            return Position;
        }

        public void Update(TGCGame tgcGame)
        {
            timer -= 0.5f;
            if (enemies > 0 && timer == 0)
            {
                tgcGame.AddEnemy(Position);
                enemies -= 1;
                timer = 300;
            }
            UpdatePosition();
        }
    }
}
