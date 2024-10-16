namespace TGC.MonoGame.TP
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Media;
    using System;
    using System.Collections.Generic;
    using TGC.MonoGame.TP.Components.Bullet;
    using TGC.MonoGame.TP.Components.Camera;
    using TGC.MonoGame.TP.Components.Enemy;
    using TGC.MonoGame.TP.Components.Map;
    using TGC.MonoGame.TP.Components.Particles;
    using TGC.MonoGame.TP.Components.Player;

    /// <summary>
    /// Esta es la clase principal  del juego.
    ///     Inicialmente puede ser renombrado o copiado para hacer más ejemplos chicos, en el caso de copiar para que se
    ///     ejecute el nuevo ejemplo deben cambiar la clase que ejecuta Program <see cref="Program.Main()" /> linea 10.
    /// </summary>
    public class TGCGame : Game
    {
        /// <summary>
        /// Defines the ContentFolder3D.
        /// </summary>
        public const string ContentFolder3D = "Models/";

        /// <summary>
        /// Defines the ContentFolderEffect.
        /// </summary>
        public const string ContentFolderEffect = "Effects/";

        /// <summary>
        /// Defines the ContentFolderMusic.
        /// </summary>
        public const string ContentFolderMusic = "Music/";

        /// <summary>
        /// Defines the ContentFolderSounds.
        /// </summary>
        public const string ContentFolderSounds = "Sounds/";

        /// <summary>
        /// Defines the ContentFolderSpriteFonts.
        /// </summary>
        public const string ContentFolderSpriteFonts = "SpriteFonts/";

        /// <summary>
        /// Defines the ContentFolderTextures.
        /// </summary>
        public const string ContentFolderTextures = "Textures/";

        /// <summary>
        /// Initializes a new instance of the <see cref="TGCGame"/> class.
        /// </summary>
        public TGCGame()
        {
            // Maneja la configuracion y la administracion del dispositivo grafico.
            Graphics = new GraphicsDeviceManager(this);

            // Carpeta raiz donde va a estar toda la Media.
            Content.RootDirectory = "Content";
            // Hace que el mouse sea visible.
            IsMouseVisible = false;
        }

        /// <summary>
        /// Gets the Graphics.
        /// </summary>
        private GraphicsDeviceManager Graphics { get; }

        /// <summary>
        /// Gets or sets the SpriteBatch.
        /// </summary>
        private SpriteBatch SpriteBatch { get; set; }

        private Song Song { get; set; }

        /// <summary>
        /// Gets or sets the Column.
        /// </summary>
        private Model Column { get; set; }
        private Effect RedIlluminationEffect { get; set; }
        private Model BulletModel { get; set; }
        private List<Bullet> Bullets { get; set; }
        private Floor Floor { get; set; }
        private Matrix QuadWorld { get; set; }

        /// <summary>
        /// Weapons
        /// </summary>
        private Model Shotgun { get; set; }
        private Model RocketLauncher { get; set; }
        private Model MachineGun { get; set; }
        private Model GranadeLauncher { get; set; }

        /// <summary>
        /// Power ups
        /// </summary>
          private Model RedArmor { get; set; }

        private Model LargeCross { get; set; }
        private float Recoil { get; set; }
        /// <summary>
        /// Gets or sets the World.
        /// </summary>

        private Matrix World { get; set; }

        /// <summary>
        /// Gets or sets the Camera.
        /// </summary>
        private FreeCamera Camera { get; set; }

        /// <summary>
        /// Gets or sets the GamePause.
        /// </summary>
        private Boolean GamePause { get; set; }
        private Boolean ClickPressed { get; set; }
        private Player Player { get; set; }

        /// <summary>
        /// Define de dash hability power starts on 51
        /// </summary>
        private float dashPower = 51f;

        /// <summary>
        /// Define if player is dashing or not
        /// </summary>
        private bool isDashing = false;

        private Particle particle { get; set; }

        /// <summary>
        /// Gets or sets the Map.
        /// </summary>
        private Map Map { get; set; }

        /// <summary>
        /// Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aquí todo el código de inicialización: todo procesamiento que podemos pre calcular para nuestro juego.
        ///     Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///     Escribir aqui el codigo de inicializacion: el procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void Initialize()
        {
            // Adapt screen size
            Graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            Graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            // Enable fullscreen 
            //Graphics.IsFullScreen = true;
            // Apply changes
            Graphics.ApplyChanges();

            ClickPressed = false;

            // Initialize Camera
            var screenSize = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            Camera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.UnitZ * 350, screenSize);
            Camera.SetDashPower(dashPower);
            Camera.SetIsDashing(isDashing);

            // Initialize player
            Player = new Player(Camera.Position);
            
            // Enable backface culling
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;

            // Create World matrix
            World = Matrix.CreateRotationY(MathHelper.Pi);

            Map = new Map();
            
            Bullets = new List<Bullet>();

            // Arranco el Game Pause en true para evitar que el jugador se mueva
            GamePause = true;

            // Configuramos nuestras matrices de la escena.
            World = Matrix.Identity;            

            base.Initialize();
        }

        /// <summary>
        /// Se llama una sola vez, al principio cuando se ejecuta el ejemplo, despues de Initialize.
        ///     Escribir aqui el codigo de inicializacion: cargar modelos, texturas, estructuras de optimizacion, el
        ///     procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            var texture = Content.Load<Texture2D>(ContentFolderTextures + "ccreteflr016a_COLOR");

            particle = new Particle(GraphicsDevice, Vector3.One * 300, Vector3.UnitZ, Vector3.Up, 100, 100, texture, 1);

            Map.LoadContent(texture,GraphicsDevice);

            // Load bullet model
            BulletModel = Content.Load<Model>(ContentFolder3D + "bullet/Bullet_9x19");
            var modelEffectBullet = (BasicEffect)BulletModel.Meshes[0].Effects[0];
            modelEffectBullet.DiffuseColor = Color.White.ToVector3();
            modelEffectBullet.EnableDefaultLighting();

            // Load shotgun model
            Shotgun = Content.Load<Model>(ContentFolder3D + "shotgun/shotgun");
 
            var modelEffect = (BasicEffect)Shotgun.Meshes[0].Effects[0];
            modelEffect.DiffuseColor = Color.White.ToVector3();
            modelEffect.EnableDefaultLighting();

            RocketLauncher = Content.Load<Model>(ContentFolder3D + "rocketLauncher/rocketLauncher");
            MachineGun = Content.Load<Model>(ContentFolder3D + "mahineGun/machineGun");
            GranadeLauncher = Content.Load<Model>(ContentFolder3D + "granadeLauncher/granadeLauncher");

            RedArmor = Content.Load<Model>(ContentFolder3D + "redArmor/redArmor");
            LargeCross = Content.Load<Model>(ContentFolder3D + "largeCross/largeCross");

            Song = Content.Load<Song>(ContentFolderMusic + "doom-ost-damnation");
            MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(Song);

            // Change the BasicShader name
            RedIlluminationEffect = Content.Load<Effect>(ContentFolderEffect + "BasicShader");
            
            RedIlluminationEffect.Parameters["ambientColor"].SetValue(Color.Red.ToVector3());
            RedIlluminationEffect.Parameters["diffuseColor"].SetValue(Color.White.ToVector3());
            RedIlluminationEffect.Parameters["specularColor"].SetValue(Color.Wheat.ToVector3());

            RedIlluminationEffect.Parameters["KAmbient"].SetValue(0.4f);
            RedIlluminationEffect.Parameters["KDiffuse"].SetValue(0.85f);
            RedIlluminationEffect.Parameters["KSpecular"].SetValue(0.15f);
            RedIlluminationEffect.Parameters["shininess"].SetValue(100f);

            base.LoadContent();
        }

        /// <summary>
        /// Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones
        ///     ante ellas.
        /// </summary>
        /// <param name="gameTime">The gameTime<see cref="GameTime"/>.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                // Exit game
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                // Pause/Start game
                GamePause = !GamePause;
            }

            // If GamePause is false i can move and play
            if (!GamePause)
            {
                Camera.Update(gameTime);
                Player.SetPosition(Camera.Position);
            
                // Creates new bullets when left click
                var mouse = Mouse.GetState();

                if (mouse.LeftButton == ButtonState.Pressed && !ClickPressed && Bullets.Count < 15 && Recoil == 0)
                {
                    Bullet singleBullet = new Bullet();
                    singleBullet.SetPosition(Camera.Position + Camera.FrontDirection * 25f);
                    singleBullet.SetDirection(Camera.FrontDirection);
                    singleBullet.SetUp(Camera.UpDirection);
                    Bullets.Add(singleBullet);
                    ClickPressed = true;
                }

                if (mouse.RightButton == ButtonState.Pressed && !ClickPressed && Bullets.Count < 15 && Recoil == 0)
                {
                    Bullet Bullet1 = new Bullet();
                    Bullet1.SetPosition(Camera.Position + Camera.FrontDirection * 50f);
                    Bullet1.SetDirection(Camera.FrontDirection);
                    Bullet1.SetUp(Camera.UpDirection);
                    Bullets.Add(Bullet1);

                    Bullet Bullet2 = new Bullet();
                    Bullet2.SetPosition(Camera.Position + Vector3.Up * 5 + Camera.FrontDirection * 25f);
                    Bullet2.SetDirection(Camera.FrontDirection);
                    Bullet2.SetUp(Camera.UpDirection);
                    Bullets.Add(Bullet2);

                    ClickPressed = true;
                }

                if (mouse.LeftButton == ButtonState.Released && mouse.RightButton == ButtonState.Released && ClickPressed) ClickPressed = false;

                if (Bullets.Count >= 15)
                {
                    Bullets.Clear();
                }

                Player.SetPosition(Camera.Position);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        /// </summary>
        /// <param name="gameTime">The gameTime<see cref="GameTime"/>.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.Black);
           
            var mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed && Recoil == 0)
            {
                Recoil = 10;
            }
            if (mouse.RightButton == ButtonState.Pressed && Recoil == 0)
            {
                Recoil = 13.5f;
            }

            Map.Draw(Camera.View, Camera.Projection);
            
            foreach(Bullet bullet in Bullets)
            {
                Vector3 BulletRight = Vector3.Cross(bullet.GetDirection(), bullet.GetUp());
                Vector3 BulletPosition = bullet.GetPosition() + bullet.GetDirection()+ BulletRight - bullet.GetUp();

                if (Vector3.Distance(bullet.GetPosition(), Camera.Position) < 2550) {
                    BulletModel.Draw(Matrix.CreateScale(5) * Matrix.CreateWorld(BulletPosition, -BulletRight, bullet.GetDirection()), Camera.View, Camera.Projection); 
                }
            }
         
            if (Recoil > 0)
            {
                Recoil -= 0.25f;
            }
                        
            Vector3 cameraRight = Vector3.Cross(Camera.FrontDirection, Camera.UpDirection);
            Vector3 weaponPosition = new Vector3(Camera.Position.X, 0, Camera.Position.Z) + new Vector3(0, -25, 0) + Camera.FrontDirection * MathHelper.Lerp(80, 65, Recoil) + cameraRight * 25 - Camera.UpDirection * 4;
            Matrix shotgunWorld = Matrix.CreateScale(0.05f) * Matrix.CreateWorld(weaponPosition, -cameraRight, Camera.UpDirection);

            Shotgun.Draw(shotgunWorld, Camera.View, Camera.Projection);

            shotgunWorld = Matrix.CreateScale(0.05f) * Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds) * Matrix.CreateWorld(new Vector3(250, 15 * MathF.Sin( (float)gameTime.TotalGameTime.TotalSeconds) - 25, 250), Vector3.UnitZ, Vector3.UnitY);
            Shotgun.Draw(shotgunWorld, Camera.View, Camera.Projection);

            shotgunWorld = Matrix.CreateScale(0.05f) * Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds) * Matrix.CreateWorld(new Vector3(450, 15 * MathF.Sin((float)gameTime.TotalGameTime.TotalSeconds) - 25, 450), Vector3.UnitZ, Vector3.UnitY);
            RocketLauncher.Draw(shotgunWorld, Camera.View, Camera.Projection);

            shotgunWorld = Matrix.CreateScale(0.05f) * Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds) * Matrix.CreateWorld(new Vector3(650, 15 * MathF.Sin((float)gameTime.TotalGameTime.TotalSeconds) - 25, 650), Vector3.UnitZ, Vector3.UnitY);
            MachineGun.Draw(shotgunWorld, Camera.View, Camera.Projection);

            shotgunWorld = Matrix.CreateScale(0.05f) * Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds) * Matrix.CreateWorld(new Vector3(850, 15 * MathF.Sin((float)gameTime.TotalGameTime.TotalSeconds) - 25, 850), Vector3.UnitZ, Vector3.UnitY);
            GranadeLauncher.Draw(shotgunWorld, Camera.View, Camera.Projection);


            shotgunWorld = Matrix.CreateScale(0.025f) * Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds) * Matrix.CreateWorld(new Vector3(100, 15 * MathF.Sin((float)gameTime.TotalGameTime.TotalSeconds) - 25, 100), Vector3.UnitZ, Vector3.UnitY);
            RedArmor.Draw(shotgunWorld, Camera.View, Camera.Projection);

            shotgunWorld = Matrix.CreateScale(0.05f) * Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds) * Matrix.CreateWorld(new Vector3(100, 15 * MathF.Sin((float)gameTime.TotalGameTime.TotalSeconds) - 25, 300), Vector3.UnitZ, Vector3.UnitY);
            LargeCross.Draw(shotgunWorld, Camera.View, Camera.Projection);
        }

        /// <summary>
        /// Libero los recursos que se cargaron en el juego.
        /// </summary>
        protected override void UnloadContent()
        {
            // Libero los recursos.
            Content.Unload();

            base.UnloadContent();
        }

    }
}
