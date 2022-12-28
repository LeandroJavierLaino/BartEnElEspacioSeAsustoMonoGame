using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Components.Map
{
    /// <summary>
    /// Defines the <see cref="Map" />.
    /// </summary>
    internal class Map
    {
        /// <summary>
        /// Gets or sets the Column.
        /// </summary>
        private Model Column { get; set; }
        private Floor Floor { get; set; }
        private Floor FloorExternal1 { get; set; }
        private Floor FloorExternal2 { get; set; }
        private Floor FloorExternal3 { get; set; }
        private Floor FloorExternal4 { get; set; }
        private Floor FloorExternal5 { get; set; }
        private Floor FloorExternal6 { get; set; }
        private Floor FloorExternal7 { get; set; }
        private Floor FloorExternal8 { get; set; }
        private Matrix QuadWorld { get; set; }

        /// <summary>
        /// Defines the separation.
        /// </summary>
        private const float separation = 450f;
        private const float minSpace = 50f;
        private const float size = 2750;

        /// <summary>
        /// Initializes a new instance of the <see cref="Map"/> class.
        /// </summary>
        public Map()
        {
        }

        /// <summary>
        /// The LoadContent.
        /// </summary>
        /// <param name="Column">The Column<see cref="Model"/>.</param>
        public void LoadContent(Texture2D floorTexture, GraphicsDevice graphicsDevice, Effect effect)
        {
            Floor = new Floor(graphicsDevice, new Vector3(0, -110, 0), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5, effect);

            FloorExternal1 = new Floor(graphicsDevice, new Vector3(size, -110, -size), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5, effect);
            FloorExternal2 = new Floor(graphicsDevice, new Vector3(size, -110, 0), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5, effect);
            FloorExternal3 = new Floor(graphicsDevice, new Vector3(0, -110, size), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5, effect);
            FloorExternal4 = new Floor(graphicsDevice, new Vector3(size, -110, size), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5, effect);
            FloorExternal5 = new Floor(graphicsDevice, new Vector3(-size, -110, size), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5, effect);
            FloorExternal6 = new Floor(graphicsDevice, new Vector3(-size, -110, 0), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5, effect);
            FloorExternal7 = new Floor(graphicsDevice, new Vector3(0, -110, -size), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5, effect);
            FloorExternal8 = new Floor(graphicsDevice, new Vector3(-size, -110, -size), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5, effect);

            QuadWorld = Matrix.CreateTranslation(Vector3.UnitX * size / 2 + Vector3.UnitZ * size / 2);
        }

        public void Update(Vector3 cameraPosition)
        {
            Floor.Update(cameraPosition);
            FloorExternal1.Update(cameraPosition);
            FloorExternal2.Update(cameraPosition);
            FloorExternal3.Update(cameraPosition);
            FloorExternal4.Update(cameraPosition);
            FloorExternal5.Update(cameraPosition);
            FloorExternal6.Update(cameraPosition);
            FloorExternal7.Update(cameraPosition);
            FloorExternal8.Update(cameraPosition);
        }

        /// <summary>
        /// The Draw.
        /// </summary>
        /// <param name="World">The World<see cref="Matrix"/>.</param>
        /// <param name="View">The View<see cref="Matrix"/>.</param>
        /// <param name="Projection">The Projection<see cref="Matrix"/>.</param>
        public void Draw(Matrix World, Matrix View, Matrix Projection)
        {
            Floor.Draw(QuadWorld, View, Projection);
            FloorExternal1.Draw(QuadWorld, View, Projection);
            FloorExternal2.Draw(QuadWorld, View, Projection);
            FloorExternal3.Draw(QuadWorld, View, Projection);
            FloorExternal4.Draw(QuadWorld, View, Projection);
            FloorExternal5.Draw(QuadWorld, View, Projection);
            FloorExternal6.Draw(QuadWorld, View, Projection);
            FloorExternal7.Draw(QuadWorld, View, Projection);
            FloorExternal8.Draw(QuadWorld, View, Projection);
        }
    }
}
