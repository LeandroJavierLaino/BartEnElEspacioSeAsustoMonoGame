using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TGC.MonoGame.TP.Components.Enemy
{
    public class Enemy
    {
        private Vector3 Position { get; set; }
        private Vector3 Direction { get; set; }
        private Vector3 Up { get; set; }
        private float Velocity { get; set; }
        private int Life { get; set; }
        private Model TGCitoEnemy { get; set; }
        private Effect TGCitoEffect { get; set; }
        private Texture2D Texture { get; set; }
        private Vector3 LightPosition { get; set; }

        private readonly int minDistance = 650;

        public Enemy(Model tgcitoModel, Effect tgcitoEffect, Texture2D tgcitoTexture, Vector3 lightPosition)
        {
            Random random = new();
            this.Velocity = 0.5f * random.Next(1, 3);
            this.Life = 130;
            this.TGCitoEnemy = tgcitoModel;
            this.TGCitoEffect = tgcitoEffect;
            this.Texture = tgcitoTexture;
            this.LightPosition = lightPosition;
            this.TGCitoEffect.Parameters["baseTexture"]?.SetValue(tgcitoTexture);
            // La luz y el material se configuran de forma centralizada en SceneLight.
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

        public void Update(Vector3 CameraPosition, List<Enemy> OtherEnemies, Vector3 PlayerPosition)
        {
            Vector3 distance = CameraPosition - this.Position;
            this.Position += (distance) * Velocity/200;
            Vector3 direction = this.Position - PlayerPosition;
            direction.Normalize();
            this.Direction = direction;

            foreach (Enemy enemy in OtherEnemies) 
            {
                if (this != enemy && Vector3.Distance(this.Position,enemy.Position) < minDistance)
                {
                    AvoidCollision(enemy.Position);
                }
            }

            if (Vector3.Distance(this.Position, PlayerPosition) < minDistance)
            {
                AvoidCollision(PlayerPosition);
            }
        }

        private void AvoidCollision(Vector3 EnemyPosition)
        {
            Vector3 direction = this.Position - EnemyPosition;
            direction.Normalize();
            this.Position += direction;
        }

        public void Draw(Matrix View, Matrix Projection, Vector3 eyePosition)
        {
            Matrix tgcitoWorld = Matrix.CreateScale(3.5f) * Matrix.CreateWorld(this.Position, this.Direction, Vector3.Up);

            foreach (ModelMesh mesh in TGCitoEnemy.Meshes)
            {
                foreach (ModelMeshPart modelMeshPart in mesh.MeshParts)
                {
                    this.TGCitoEffect.Parameters["baseTexture"]?.SetValue(this.Texture);
                    this.TGCitoEffect.Parameters["World"].SetValue(tgcitoWorld);
                    this.TGCitoEffect.Parameters["InverseTransposeWorld"]
                        ?.SetValue(Matrix.Transpose(Matrix.Invert(tgcitoWorld)));
                    this.TGCitoEffect.Parameters["View"].SetValue(View);
                    this.TGCitoEffect.Parameters["Projection"]?.SetValue(Projection);
                    this.TGCitoEffect.Parameters["eyePosition"]?.SetValue(eyePosition);
                    // Modo debug de normales: 1 = ver normales como color, 0 = iluminación normal
                    this.TGCitoEffect.Parameters["DebugNormals"]?.SetValue(0f);
                    modelMeshPart.Effect = this.TGCitoEffect;
                }

                mesh.Draw();
            }
        }
    }
}
