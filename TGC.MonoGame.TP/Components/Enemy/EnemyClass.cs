using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TGC.MonoGame.TP.Components.Player;

namespace TGC.MonoGame.TP.Components.Enemy
{
    public class EnemyClass
    {
        private Vector3 Position { get; set; }
        private Vector3 Direction { get; set; }
        private Vector3 Up { get; set; }
        private float Velocity { get; set; }
        private int Life { get; set; }
        private bool GotSoul { get; set; }

        private const int MIN_DISTANCE = 125;
        private const int MIN_DISTANCE_PLAYER = 200;
        private const float EPSILON = 1.0000001f;
        private const float DAMAGE = 0.15f;
        public EnemyClass()
        {
            Random random = new Random();
            Velocity = 5f * random.Next(1, 3);
            Life = 130;
            GotSoul = true;
        }

        public void SetPosition(Vector3 Position)
        {
            this.Position = Position;
        }

        public Vector3 GetPosition()
        {
            return Position;
        }

        public void SetDirection(Vector3 Direction)
        {
            this.Direction = Direction;
        }

        public Vector3 GetDirection()
        {
            return Direction;
        }

        public void SetUp(Vector3 Up)
        {
            this.Up = Up;
        }

        public Vector3 GetUp()
        {
            return Up;
        }

        public int GetLife()
        {
            return Life;
        }

        public void TakeDamage(int Damage)
        {
            Life -= Damage;
        }

        public void Update(Vector3 CameraPosition, List<EnemyClass> OtherEnemies, PlayerClass player)
        {
            Vector3 distance = CameraPosition - Position;
            distance.Normalize();

            if(Vector3.Distance(Position, player.GetPosition()) <= MIN_DISTANCE_PLAYER && Life > 0)
            {
                AvoidCollisionPlayer(player.GetPosition());
                player.LoseLife(DAMAGE); 
            }
            else
            {
                Direction.Normalize();
                Position += -Direction * Velocity;
            }

            foreach (EnemyClass enemy in OtherEnemies)
            {
                if (this != enemy && Vector3.Distance(Position, enemy.Position) <= MIN_DISTANCE && Life > 0)
                {
                    AvoidCollision(enemy.Position);
                }              
            }

            if (Position.Y <= 100)
            {
                Position = new Vector3(Position.X, 100, Position.Z);
            }

            if(Life <= 0 && GotSoul)
            {
                player.GainSoul();
                GotSoul = false;
            }
        }

        private void AvoidCollision(Vector3 EnemyPosition)
        {
            Vector3 direction = Position - EnemyPosition;
            direction.Normalize();
            Position += direction * EPSILON;
        }

        private void AvoidCollisionPlayer(Vector3 PlayerPosition)
        {
            Vector3 direction = PlayerPosition - Position;
            direction.Normalize();
            Position += Direction;
        }
    }
}
