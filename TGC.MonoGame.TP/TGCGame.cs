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
    using TGC.MonoGame.TP.Components.Lighting;
    using TGC.MonoGame.TP.Components.Map;
    using TGC.MonoGame.TP.Components.Particles;
    using TGC.MonoGame.TP.Components.Player;
    using TGC.MonoGame.TP.Components.PowerUps;
    using TGC.MonoGame.TP.Components.Weapons;

    /// <summary>
    ///     Esta es la clase principal del juego.
    ///     Inicialmente puede ser renombrado o copiado para hacer mas ejemplos chicos, en el caso de copiar para que se
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
            
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 100;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 100;
            
            // Para que el juego sea pantalla completa se puede usar Graphics IsFullScreen.
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
        private Vector3 LightPosition { get; set; }
        private SceneLight Light { get; set; }
        private Model BulletModel { get; set; }
        private List<Bullet> Bullets { get; set; }
        private Floor Floor { get; set; }
        private Matrix QuadWorld { get; set; }

        /// <summary>
        /// Weapons
        /// </summary>
        private Model Shotgun { get; set; }
        private Model RocketLauncherModel { get; set; }
        private Model MachineGunModel { get; set; }
        private Model GranadeLauncherModel { get; set; }
        private WeaponCollect RocketLauncherCollect { get; set; }
        private WeaponCollect MachineGunCollect { get; set; }
        private WeaponCollect GranadeLauncherCollect { get; set; }
       
        /// <summary>
        /// Power ups
        /// </summary>
        private Model RedArmor { get; set; }
        private Model LargeCross { get; set; }
        private PowerUpCollect RedArmorCollect { get; set; }
        private PowerUpCollect LargeCrossCollect { get; set; }


        /// <summary>
        /// Enemy
        /// </summary>
        private Model TGCitoEnemyModel { get; set; }
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
        private Texture2D TGCitoTexture { get; set; }
        private Effect TGCitoEffect { get; set; }
        private Enemy TGCitoEnemy { get; set; }
        private WeaponFPS ShotgunFPS { get; set; }
        private WeaponFPS RocketLauncherFPS { get; set; }

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
            rasterizerState.CullMode = CullMode.None;
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
            LightPosition = new Vector3(1500f, 120f, 1500f);

            // Punto único de configuración de la luz para todo modelo que use el shader
            Light = new SceneLight { Position = LightPosition };

            TGCitoEnemyModel = Content.Load<Model>(ContentFolder3D + "tgcitohead/tgcitohead");
            Texture2D TGCitoTexture = TGCitoEnemyModel.Meshes[0].MeshParts[0].Effect.Parameters["Texture"].GetValueTexture2D();

            //var pixelatedEffect = Content.Load<Effect>(ContentFolderEffect + "BlingPhongPixelated");
            TGCitoEffect = Content.Load<Effect>(ContentFolderEffect + "BlingPhongPixelatedModels");

            // Aplico la configuración de luz centralizada al effect compartido
            Light.Apply(TGCitoEffect);

            Map.LoadContent(texture, GraphicsDevice, TGCitoEffect, LightPosition);

            TGCitoEffect = Content.Load<Effect>(ContentFolderEffect + "BlingPhongPixelatedModels"); ;
            
            TGCitoEnemy = new Enemy(TGCitoEnemyModel, TGCitoEffect, TGCitoTexture, new Vector3(200, 50, 200));

            Model FPSWeaponModel = Content.Load<Model>(ContentFolder3D + "shotgun/shotgun");
            Texture2D FPSWeaponTexture = FPSWeaponModel.Meshes[0].MeshParts[1].Effect.Parameters["Texture"].GetValueTexture2D();
            ShotgunFPS = new WeaponFPS(FPSWeaponModel, TGCitoEffect, FPSWeaponTexture, WeaponKeys.Shotgun);
            ShotgunFPS.SetActive(true);
            Player.AddWeapon(ShotgunFPS);

            // Load bullet model
            BulletModel = Content.Load<Model>(ContentFolder3D + "bullet/Bullet_9x19");

            RocketLauncherModel = Content.Load<Model>(ContentFolder3D + "rocketLauncher/rocketLauncher");
            Texture2D RocketLauncherTexture = RocketLauncherModel.Meshes[0].MeshParts[0].Effect.Parameters["Texture"].GetValueTexture2D();
            RocketLauncherCollect = new WeaponCollect(RocketLauncherModel, TGCitoEffect, RocketLauncherTexture, new Vector3(450f, 0f, 450f), LightPosition);

            RocketLauncherFPS = new WeaponFPS(RocketLauncherModel, TGCitoEffect, RocketLauncherTexture, WeaponKeys.RocketLauncher);
            
            Player.AddWeapon(RocketLauncherFPS);

            MachineGunModel = Content.Load<Model>(ContentFolder3D + "mahineGun/machineGun");
            Texture2D machineGunTexture2D = MachineGunModel.Meshes[0].MeshParts[0].Effect.Parameters["Texture"].GetValueTexture2D();
            MachineGunCollect = new WeaponCollect(MachineGunModel, TGCitoEffect, machineGunTexture2D, new Vector3(650f, 0f, 650f), LightPosition);

            GranadeLauncherModel = Content.Load<Model>(ContentFolder3D + "granadeLauncher/granadeLauncher");
            Texture2D granadeLauncherTexture2D = GranadeLauncherModel.Meshes[0].MeshParts[0].Effect.Parameters["Texture"].GetValueTexture2D();
            GranadeLauncherCollect = new WeaponCollect(GranadeLauncherModel, TGCitoEffect, granadeLauncherTexture2D, new Vector3(850f, 0f, 850f), LightPosition);

            RedArmor = Content.Load<Model>(ContentFolder3D + "redArmor/redArmor");
            Texture2D redArmorTexture2D = RedArmor.Meshes[0].MeshParts[0].Effect.Parameters["Texture"].GetValueTexture2D();
            RedArmorCollect = new PowerUpCollect(RedArmor, TGCitoEffect, redArmorTexture2D, new Vector3(250f, 0f, 250f), LightPosition);

            LargeCross = Content.Load<Model>(ContentFolder3D + "largeCross/largeCross");
            Texture2D largeCrossTexture2D = LargeCross.Meshes[0].MeshParts[0].Effect.Parameters["Texture"].GetValueTexture2D();
            LargeCrossCollect = new PowerUpCollect(LargeCross, TGCitoEffect, largeCrossTexture2D, new Vector3(1050f, 0f, 1050f), LightPosition);

            Song = Content.Load<Song>(ContentFolderMusic + "doom-game");
            MediaPlayer.Volume = 50;
            MediaPlayer.IsRepeating = true;
            //MediaPlayer.Play(Song);
            
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
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape))
                // Exit game
                Exit();

            if (keyboardState.IsKeyDown(Keys.P))
            {
                // Pause/Start game
                GamePause = !GamePause;
            }

            // If GamePause is false i can move and play
            if (!GamePause)
            {
                // Creates new bullets when left click
                var mouse = Mouse.GetState();

                Camera.Update(gameTime);
                Player.SetPosition(Camera.Position);
                
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

                if (mouse.LeftButton == ButtonState.Pressed && Recoil == 0)
                {
                    Recoil = 10;
                }

                if (mouse.RightButton == ButtonState.Pressed && Recoil == 0)
                {
                    Recoil = 13.5f;
                }

                if (Recoil > 0)
                {
                    Recoil -= 0.25f;
                }

                ShotgunFPS.Update(Camera.Position, Recoil, gameTime);
                RocketLauncherFPS.Update(Camera.Position, Recoil, gameTime);
                Player.SetPosition(Camera.Position);
                TGCitoEnemy.Update( [],Player.GetPosition(), gameTime);
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

            // Reaplico la luz centralizada para que ningún componente la pise
            Light.Apply(TGCitoEffect);

            foreach(Bullet bullet in Bullets)
            {
                Vector3 BulletRight = Vector3.Cross(bullet.GetDirection(), bullet.GetUp());
                Vector3 BulletPosition = bullet.GetPosition() + bullet.GetDirection()+ BulletRight - bullet.GetUp();

                if (Vector3.Distance(bullet.GetPosition(), Camera.Position) < 2550) {
                    BulletModel.Draw(Matrix.CreateScale(5) * Matrix.CreateWorld(BulletPosition, -BulletRight, bullet.GetDirection()), Camera.View, Camera.Projection); 
                }
            }

            TGCitoEnemy.Draw(Camera.View, Camera.Projection, Camera.Position);

            Map.Draw(Camera.View, Camera.Projection, Camera.Position);

            // Player
            ShotgunFPS.Draw(Camera.View, Camera.Projection, Camera);
            RocketLauncherFPS.Draw(Camera.View, Camera.Projection, Camera);

            // Weapons collectables
            RocketLauncherCollect.Draw(Camera.View, Camera.Projection, gameTime, Camera, false);
            MachineGunCollect.Draw(Camera.View, Camera.Projection, gameTime, Camera, true);
            GranadeLauncherCollect.Draw(Camera.View, Camera.Projection, gameTime, Camera, false);

            // Power ups collectables
            RedArmorCollect.Draw(Camera.View, Camera.Projection, gameTime, Camera, false);
            LargeCrossCollect.Draw(Camera.View, Camera.Projection, gameTime, Camera, false);
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
