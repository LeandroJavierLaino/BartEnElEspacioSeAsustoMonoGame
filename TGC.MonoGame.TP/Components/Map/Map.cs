namespace TGC.MonoGame.TP.Components.Map
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Defines the <see cref="Map" />.
    /// </summary>
    internal class Map
    {
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
        public void LoadContent( Texture2D floorTexture, GraphicsDevice graphicsDevice)
        {
            Floor = new Floor(graphicsDevice, new Vector3(0, -110, 0), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5);

            FloorExternal1 = new Floor(graphicsDevice, new Vector3(size, -110, -size), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5);
            FloorExternal2 = new Floor(graphicsDevice, new Vector3(size, -110, 0), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5);
            FloorExternal3 = new Floor(graphicsDevice, new Vector3(0, -110, size), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5);
            FloorExternal4 = new Floor(graphicsDevice, new Vector3(size, -110, size), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5);
            FloorExternal5 = new Floor(graphicsDevice, new Vector3(-size, -110, size), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5);
            FloorExternal6 = new Floor(graphicsDevice, new Vector3(-size, -110, 0), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5);
            FloorExternal7 = new Floor(graphicsDevice, new Vector3(0, -110, -size), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5);
            FloorExternal8 = new Floor(graphicsDevice, new Vector3(-size, -110, -size), Vector3.Up, Vector3.Backward, size, size, floorTexture, 5);

            QuadWorld = Matrix.CreateTranslation(Vector3.UnitX * size / 2 + Vector3.UnitZ * size / 2);
        }

        /// <summary>
        /// The Draw.
        /// </summary>
        /// <param name="View">The View<see cref="Matrix"/>.</param>
        /// <param name="Projection">The Projection<see cref="Matrix"/>.</param>
        public void Draw( Matrix View, Matrix Projection)
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
