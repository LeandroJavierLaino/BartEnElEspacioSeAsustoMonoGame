using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.MonoGame.TP.Components.Lighting
{
    /// <summary>
    ///     Configuración central de la luz para todos los modelos que usan el shader de iluminación.
    ///     Un único punto para ajustar posición, colores y atenuación de la luz de la escena.
    /// </summary>
    public class SceneLight
    {
        /// <summary>
        ///     Posición de la luz en el mundo.
        /// </summary>
        public Vector3 Position { get; set; } = new Vector3(1500f, 20f, 1500f);

        /// <summary>
        ///     Color de la componente ambiente de la luz.
        /// </summary>
        public Vector3 AmbientColor { get; set; } = Color.Red.ToVector3();

        /// <summary>
        ///     Color de la componente difusa de la luz.
        /// </summary>
        public Vector3 DiffuseColor { get; set; } = Color.White.ToVector3();

        /// <summary>
        ///     Color de la componente especular de la luz.
        /// </summary>
        public Vector3 SpecularColor { get; set; } = Color.White.ToVector3();

        /// <summary>
        ///     Coeficiente de atenuación lineal. Más bajo = mayor radio de luz.
        /// </summary>
        public float LinearAttenuation { get; set; } = 0f;

        /// <summary>
        ///     Coeficiente de atenuación cuadrático. Controla principalmente el tamaño de la esfera de luz.
        /// </summary>
        public float QuadraticAttenuation { get; set; } = 0.00000000013f;

        // --- Material (por ahora compartido por todos los meshes) ---

        /// <summary>
        ///     Coeficiente de la componente ambiente del material.
        /// </summary>
        public float KAmbient { get; set; } = 0.275f;

        /// <summary>
        ///     Coeficiente de la componente difusa del material.
        /// </summary>
        public float KDiffuse { get; set; } = 0.70f;

        /// <summary>
        ///     Coeficiente de la componente especular del material.
        /// </summary>
        public float KSpecular { get; set; } = 0.5f;

        /// <summary>
        ///     Exponente especular (brillo). Más alto = brillo más concentrado.
        /// </summary>
        public float Shininess { get; set; } = 256f;

        /// <summary>
        ///     Aplica la configuración de la luz al effect compartido.
        ///     Como el effect es una única instancia cacheada, basta con llamar a esto una sola vez.
        /// </summary>
        /// <param name="effect">El effect que usa el shader de iluminación.</param>
        public void Apply(Effect effect)
        {
            effect.Parameters["lightPosition"]?.SetValue(Position);
            effect.Parameters["ambientColor"]?.SetValue(AmbientColor);
            effect.Parameters["diffuseColor"]?.SetValue(DiffuseColor);
            effect.Parameters["specularColor"]?.SetValue(SpecularColor);
            effect.Parameters["LightLinearAttenuation"]?.SetValue(LinearAttenuation);
            effect.Parameters["LightQuadraticAttenuation"]?.SetValue(QuadraticAttenuation);

            // Material compartido por todos los meshes
            effect.Parameters["KAmbient"]?.SetValue(KAmbient);
            effect.Parameters["KDiffuse"]?.SetValue(KDiffuse);
            effect.Parameters["KSpecular"]?.SetValue(KSpecular);
            effect.Parameters["shininess"]?.SetValue(Shininess);
        }
    }
}
