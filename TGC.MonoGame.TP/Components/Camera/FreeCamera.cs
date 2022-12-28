namespace TGC.MonoGame.TP.Components.Camera
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;

    /// <summary>
    /// Defines the <see cref="FreeCamera" />.
    /// </summary>
    internal class FreeCamera : Camera
    {
        /// <summary>
        /// Defines the screenCenter.
        /// </summary>
        private readonly Point screenCenter;

        /// <summary>
        /// Defines the changed.
        /// </summary>
        private bool changed;

        /// <summary>
        /// Defines the pastMousePosition.
        /// </summary>
        private Vector2 pastMousePosition;

        /// <summary>
        /// Defines the accumulatedTime.
        /// </summary>
        private float accumulatedTime = 0f;

        /// <summary>
        /// Defines the bobOscilate.
        /// </summary>
        private float bobOscilate = 0f;

        /// <summary>
        /// Define de dash hability power starts on 51
        /// </summary>
        private float dashPower;

        /// <summary>
        /// Define if player is dashing or not
        /// </summary>
        private bool isDashing;

        private readonly float SLIDING_VELOCITY = 3f;
        private readonly float DELTA_AMOUNT = 0.00015f;
        private readonly int POSITIVE_LIMIT = 5400;
        private readonly int NEGATIVE_LIMIT = -2700;
        private readonly float MAX_PITCH = 89.0f;

        // Angles
        /// <summary>
        /// Defines the pitch.
        /// </summary>
        public float pitch;

        /// <summary>
        /// Defines the yaw.
        /// </summary>
        public float yaw = -90f;

        public void SetDashPower(float newDashPower)
        {
            dashPower = newDashPower;
        }

        public float GetDashPower()
        {
            return dashPower;
        }

        public void SetIsDashing(bool newIsDashing)
        {
            isDashing = newIsDashing;
        }

        public bool GetIsDashing()
        {
            return isDashing;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreeCamera"/> class.
        /// </summary>
        /// <param name="aspectRatio">The aspectRatio<see cref="float"/>.</param>
        /// <param name="position">The position<see cref="Vector3"/>.</param>
        /// <param name="screenCenter">The screenCenter<see cref="Point"/>.</param>
        public FreeCamera(float aspectRatio, Vector3 position, Point screenCenter) : this(aspectRatio, position)
        {
            this.screenCenter = screenCenter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FreeCamera"/> class.
        /// </summary>
        /// <param name="aspectRatio">The aspectRatio<see cref="float"/>.</param>
        /// <param name="position">The position<see cref="Vector3"/>.</param>
        public FreeCamera(float aspectRatio, Vector3 position) : base(aspectRatio)
        {
            Position = position;
            pastMousePosition = Mouse.GetState().Position.ToVector2();        
            UpdateCameraVectors();
            CalculateView();
        }

        /// <summary>
        /// Gets or sets the MovementSpeed.
        /// </summary>
        public float MovementSpeed { get; set; } = 0.65f;

        /// <summary>
        /// Gets or sets the MouseSensitivity.
        /// </summary>
        public float MouseSensitivity { get; set; } = 0.000035f;

        /// <summary>
        /// The CalculateView.
        /// </summary>
        private void CalculateView()
        {
            View = Matrix.CreateLookAt(Position, Position + FrontDirection, UpDirection);
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            var elapsedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            changed = false;
            ProcessKeyboard(elapsedTime);
            ProcessMouseMovement(elapsedTime);

            if (changed)
                CalculateView();
        }

        /// <summary>
        /// The BobingOscilation.
        /// </summary>
        /// <returns>The <see cref="float"/>.</returns>
        private float BobingOscilation()
        {
            return MathF.Sin(accumulatedTime / 100 );
        }

        /// <summary>
        /// The ProcessKeyboard.
        /// </summary>
        /// <param name="elapsedTime">The elapsedTime<see cref="float"/>.</param>
        private void ProcessKeyboard(float elapsedTime)
        {
            accumulatedTime += elapsedTime;
            var keyboardState = Keyboard.GetState();

            var currentMovementSpeed = MovementSpeed;

            var newPosition = Position;

            if (dashPower >= 50 && isDashing) isDashing = false;

            if (dashPower < 51) dashPower += 0.1f;

            if (keyboardState.IsKeyDown(Keys.LeftShift) && dashPower >= 50 && !isDashing)
            {
                dashPower -= 50;
                isDashing = true;
                currentMovementSpeed *= 150f;
            }

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
            {
                newPosition += -RightDirection * currentMovementSpeed * elapsedTime;
                bobOscilate = BobingOscilation();
                changed = true;
            }

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
            {
                newPosition += RightDirection * currentMovementSpeed * elapsedTime;
                bobOscilate = BobingOscilation();
                changed = true;
            }

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            {
                newPosition += FrontDirection * currentMovementSpeed * elapsedTime;
                bobOscilate = BobingOscilation();
                changed = true;
            }

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
            {
                newPosition += -FrontDirection * currentMovementSpeed * elapsedTime;
                bobOscilate = BobingOscilation();
                changed = true;
            }

            // Collision with external space
            if (newPosition.X >= NEGATIVE_LIMIT && newPosition.Z >= NEGATIVE_LIMIT && newPosition.X <= POSITIVE_LIMIT && newPosition.Z <= POSITIVE_LIMIT)
            {
                Position = new Vector3(newPosition.X, bobOscilate, newPosition.Z);
            }
            else
            {
                // Simple sliding solution
                if(newPosition.X < NEGATIVE_LIMIT)
                {
                    Position = new Vector3(Position.X, bobOscilate, Position.Z + SLIDING_VELOCITY * FrontDirection.Z);
                }

                if (newPosition.X > POSITIVE_LIMIT)
                {
                    Position = new Vector3(Position.X, bobOscilate, Position.Z + SLIDING_VELOCITY * FrontDirection.Z);
                }

                if (newPosition.Z < NEGATIVE_LIMIT)
                {
                    Position = new Vector3(Position.X + SLIDING_VELOCITY * FrontDirection.X, bobOscilate, Position.Z );
                }

                if (newPosition.Z > POSITIVE_LIMIT)
                {
                    Position = new Vector3(Position.X + SLIDING_VELOCITY * FrontDirection.X, bobOscilate, Position.Z );
                }
            }
        }

        public void RotateLeftOrRight(float gameTime, float amount)
        {

            yaw -= 2 * amount * -MouseSensitivity * gameTime;

        }
        public void RotateUpOrDown(float gameTime, float amount)
        {
            pitch += 2 * amount * -MouseSensitivity * gameTime;
            if (pitch > MAX_PITCH)
                pitch = MAX_PITCH;
            if (pitch < -MAX_PITCH)
                pitch = -MAX_PITCH;
        }

        /// <summary>
        /// The ProcessMouseMovement.
        /// </summary>
        /// <param name="elapsedTime">The elapsedTime<see cref="float"/>.</param>
        private void ProcessMouseMovement(float elapsedTime)
        {
            MouseState mouseState = Mouse.GetState();
            
            var mouseDelta = mouseState.Position.ToVector2() - pastMousePosition;
            

            if(mouseDelta.X != DELTA_AMOUNT)
            {
                RotateLeftOrRight(elapsedTime, mouseDelta.X);
                changed = true;
            }
            if (mouseDelta.Y != DELTA_AMOUNT) 
            {
                RotateUpOrDown(elapsedTime, mouseDelta.Y);
                changed = true;
            }
            

            UpdateCameraVectors();
            Mouse.SetPosition(screenCenter.X, screenCenter.Y);

            pastMousePosition = Mouse.GetState().Position.ToVector2();
        }

        /// <summary>
        /// The UpdateCameraVectors.
        /// </summary>
        private void UpdateCameraVectors()
        {
            // Calculate the new Front vector
            Vector3 tempFront;
            tempFront.X = MathF.Cos((yaw)) * MathF.Cos((pitch));
            tempFront.Y = MathF.Sin((pitch));
            tempFront.Z = MathF.Sin((yaw)) * MathF.Cos((pitch));

            FrontDirection = Vector3.Normalize(tempFront);

            // Also re-calculate the Right and Up vector
            // Normalize the vectors, because their length gets closer to 0 the more you look up or down which results in slower movement.
            RightDirection = Vector3.Normalize(Vector3.Cross(FrontDirection, Vector3.Up));
            UpDirection = Vector3.Normalize(Vector3.Cross(RightDirection, FrontDirection));
        }
    }
}
