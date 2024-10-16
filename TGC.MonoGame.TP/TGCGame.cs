﻿namespace TGC.MonoGame.TP
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
    using TGC.MonoGame.TP.Components.Spawner;

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
        /// Defines the ContentFolderFonts.
        /// </summary>
        public const string ContentFolderFonts = "Fonts/";

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
        private Effect RedIlluminationEffect { get; set; }
        private Effect SkeletonEffect { get; set; }
        private Effect ShotgunEffect { get; set; }
        private SpawnerModel SpawnerModel { get; set; }
        private Spawner Spawner { get; set; }
        private Spawner Spawner2 { get; set; }
        private Model Skull { get; set; }
        private Model BulletModel { get; set; }
        private List<Bullet> Bullets { get; set; }

        /// <summary>
        /// Gets or sets the Shotgun.
        /// </summary>
        private Model Shotgun { get; set; }
        private float Recoil { get; set; }
        /// <summary>
        /// Gets or sets the World.
        /// </summary>

        private Matrix World { get; set; }

        /// <summary>
        /// Gets or sets the Camera.
        /// </summary>
        private FreeCamera Camera { get; set; }
        private Texture2D crosshair; 
        private SpriteFont Font { get; set; }

        /// <summary>
        /// Gets or sets the GamePause.
        /// </summary>
        private Boolean GamePause { get; set; }
        private Boolean ClickPressed { get; set; }
        private PlayerClass Player { get; set; }
        private List<EnemyClass> Enemies { get; set; }
        private Random random { get; set; }

        /// <summary>
        /// Define de dash hability power starts on 51
        /// </summary>
        private readonly float dashPower = 51f;

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
            
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromMilliseconds(16);
            IsMouseVisible = false;
            ClickPressed = false;

            // Initialize Camera
            var screenCenter = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            Camera = new FreeCamera(GraphicsDevice.Viewport.AspectRatio, Vector3.UnitZ * 350, screenCenter);
            Camera.SetDashPower(dashPower);
            Camera.SetIsDashing(isDashing);

            // Initialize player
            Player = new PlayerClass(Camera.Position);

            // Enable backface culling
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;
            GraphicsDevice.RasterizerState = rasterizerState;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            // Create World matrix
            World = Matrix.CreateRotationY(MathHelper.Pi);

            Map = new Map();

            Bullets = new List<Bullet>();

            Enemies = new List<EnemyClass>();

            Spawner = new Spawner();
            Spawner.SetPosition(new Vector3(850, 120, 850));

            Spawner2 = new Spawner();
            Spawner2.SetPosition(new Vector3(350, 120, 450));

            // Arranco el Game Pause en true para evitar que el jugador se mueva
            GamePause = true;

            // Configuramos nuestras matrices de la escena.
            World = Matrix.Identity;

            random = new Random();
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

            Font = Content.Load<SpriteFont>(ContentFolderFonts + "Font");

            var texture = Content.Load<Texture2D>(ContentFolderTextures + "ccreteflr016a_COLOR");

            particle = new Particle(GraphicsDevice, Vector3.One * 300, Vector3.UnitZ, Vector3.Up, 100, 100, texture, 1);
                 
            crosshair = Content.Load<Texture2D>(ContentFolderTextures + "crosshair");
            // Load bullet model
            BulletModel = Content.Load<Model>(ContentFolder3D + "bullet/Bullet_9x19");
            var modelEffectBullet = (BasicEffect)BulletModel.Meshes[0].Effects[0];
            modelEffectBullet.DiffuseColor = Color.White.ToVector3();
            modelEffectBullet.EnableDefaultLighting();

            // Load enemy model
            Skull = Content.Load<Model>(ContentFolder3D + "skull/skull");
            var modelEffectSkull = (BasicEffect)BulletModel.Meshes[0].Effects[0];
            modelEffectSkull.DiffuseColor = Color.White.ToVector3();
            modelEffectSkull.EnableDefaultLighting();

            // Load Spawner model
            SpawnerModel = new SpawnerModel();
            SpawnerModel.LoadContent(ContentFolder3D, Content);

            // Load shotgun model
            Shotgun = Content.Load<Model>(ContentFolder3D + "shotgun/shotgun");
            var modelEffect = (BasicEffect)Shotgun.Meshes[0].Effects[0];
            modelEffect.DiffuseColor = Color.White.ToVector3();
            modelEffect.EnableDefaultLighting();

            Song = Content.Load<Song>(ContentFolderMusic + "doom-ost-damnation");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Song);

            // Change the BasicShader name
            RedIlluminationEffect = Content.Load<Effect>(ContentFolderEffect + "BasicShader");
            var effectFloor = Content.Load<Effect>(ContentFolderEffect + "FloorShader");
            // Column = Content.Load<Model>(ContentFolder3D + "bonecolumn/bonecolumn");
            Map.LoadContent(texture, GraphicsDevice, effectFloor);

            RedIlluminationEffect.Parameters["ambientColor"].SetValue(Color.Red.ToVector3());
            RedIlluminationEffect.Parameters["diffuseColor"].SetValue(Color.White.ToVector3());
            RedIlluminationEffect.Parameters["specularColor"].SetValue(Color.Wheat.ToVector3());

            RedIlluminationEffect.Parameters["KAmbient"].SetValue(0.4f);
            RedIlluminationEffect.Parameters["KDiffuse"].SetValue(0.85f);
            RedIlluminationEffect.Parameters["KSpecular"].SetValue(0.15f);
            RedIlluminationEffect.Parameters["shininess"].SetValue(100f);
            RedIlluminationEffect.CurrentTechnique = RedIlluminationEffect.Techniques["BasicColorDrawing"];
            SpawnerModel.SetEffect(RedIlluminationEffect);

            SkeletonEffect = Content.Load<Effect>(ContentFolderEffect + "BasicShader");

            SkeletonEffect.Parameters["ambientColor"].SetValue(Color.Red.ToVector3());
            SkeletonEffect.Parameters["diffuseColor"].SetValue(Color.White.ToVector3());
            SkeletonEffect.Parameters["specularColor"].SetValue(Color.Wheat.ToVector3());

            SkeletonEffect.Parameters["KAmbient"].SetValue(0.2f);
            SkeletonEffect.Parameters["KDiffuse"].SetValue(0.64f);
            SkeletonEffect.Parameters["KSpecular"].SetValue(0.5f);
            SkeletonEffect.Parameters["shininess"].SetValue(96.078431f);
            SkeletonEffect.CurrentTechnique = SkeletonEffect.Techniques["SkeletonDrawing"];

            ShotgunEffect = Content.Load<Effect>(ContentFolderEffect + "ShotgunShader");

            ShotgunEffect.Parameters["ambientColor"].SetValue(Color.Red.ToVector3());
            ShotgunEffect.Parameters["diffuseColor"].SetValue(Color.White.ToVector3());
            ShotgunEffect.Parameters["specularColor"].SetValue(Color.Wheat.ToVector3());

            ShotgunEffect.Parameters["KAmbient"].SetValue(0.2f);
            ShotgunEffect.Parameters["KDiffuse"].SetValue(0.64f);
            ShotgunEffect.Parameters["KSpecular"].SetValue(0.5f);
            ShotgunEffect.Parameters["shininess"].SetValue(196.078431f);           

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

                // Creates new bullets when left click
                var mouse = Mouse.GetState();

                if (mouse.LeftButton == ButtonState.Pressed && Bullets.Count < 15 && Recoil == 0)
                {
                    Bullet singleBullet = new Bullet();
                    singleBullet.SetPosition(Camera.Position + Camera.FrontDirection * 25f);
                    singleBullet.SetDirection(Camera.FrontDirection);
                    singleBullet.SetUp(Camera.UpDirection);
                    Bullets.Add(singleBullet);
                    
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

                float totalGameTime = (float)gameTime.TotalGameTime.TotalSeconds;

                Map.Update(Camera.Position);

                Spawner.Update(this);
                Spawner2.Update(this);

                RedIlluminationEffect.Parameters["lightPosition"].SetValue(Camera.Position);
                RedIlluminationEffect.Parameters["eyePosition"].SetValue(Camera.Position);
                RedIlluminationEffect.Parameters["Time"].SetValue(totalGameTime);

                SkeletonEffect.Parameters["lightPosition"].SetValue(Camera.Position);
                SkeletonEffect.Parameters["eyePosition"].SetValue(Camera.Position);

                ShotgunEffect.Parameters["lightPosition"].SetValue(new Vector3( 1000,100,1000));
                ShotgunEffect.Parameters["eyePosition"].SetValue(Camera.Position);

                Player.SetPosition(Camera.Position);

                foreach (EnemyClass enemy in Enemies)
                {
                    Vector3 enemyPos = enemy.GetPosition();

                    enemy.SetUp(Camera.UpDirection);
                    enemy.SetDirection(Vector3.Normalize(new Vector3(enemyPos.X - Camera.Position.X, 0, enemyPos.Z - Camera.Position.Z)));
                    enemy.Update(Camera.Position, Enemies, Player);

                    foreach (Bullet bullet in Bullets)
                    {
                        bullet.Update();

                        if (Vector3.Distance(bullet.GetPosition(), enemy.GetPosition()) < 100 && !bullet.GetDidDamage())
                        {
                            enemy.TakeDamage(bullet.GetDamage());
                            bullet.DoDamage();
                        }
                    }
                }

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

            
            Map.Draw(World, Camera.View, Camera.Projection);

            float totalGameTime = (float)gameTime.TotalGameTime.TotalSeconds;

            RedIlluminationEffect.Parameters["ambientColor"].SetValue(Color.Black.ToVector3());
            RedIlluminationEffect.Parameters["diffuseColor"].SetValue(Color.Red.ToVector3());
            RedIlluminationEffect.Parameters["specularColor"].SetValue(Color.Wheat.ToVector3());

            RedIlluminationEffect.Parameters["KAmbient"].SetValue(0.4f);
            RedIlluminationEffect.Parameters["KDiffuse"].SetValue(0.85f);
            RedIlluminationEffect.Parameters["KSpecular"].SetValue(0.15f);
            RedIlluminationEffect.Parameters["shininess"].SetValue(100f);
            RedIlluminationEffect.CurrentTechnique = RedIlluminationEffect.Techniques["BasicColorDrawing"];
            SpawnerModel.Draw(Camera.View, Camera.Projection,totalGameTime,Spawner);
            SpawnerModel.Draw(Camera.View, Camera.Projection, totalGameTime, Spawner2);

            SkeletonEffect.Parameters["ambientColor"].SetValue(Color.Black.ToVector3());
            SkeletonEffect.Parameters["diffuseColor"].SetValue(Color.Red.ToVector3());
            SkeletonEffect.Parameters["specularColor"].SetValue(Color.Wheat.ToVector3());

            SkeletonEffect.Parameters["KAmbient"].SetValue(0.4f);
            SkeletonEffect.Parameters["KDiffuse"].SetValue(0.64f);
            SkeletonEffect.Parameters["KSpecular"].SetValue(0.5f);
            SkeletonEffect.Parameters["shininess"].SetValue(96.078431f);
            SkeletonEffect.CurrentTechnique = SkeletonEffect.Techniques["SkeletonDrawing"];
            foreach (EnemyClass enemy in Enemies)
            {
                if (enemy.GetLife() > 0)
                {
                    Vector3 SkullRight = Vector3.Cross(enemy.GetDirection(), enemy.GetUp());
                    Vector3 SkullPosition = enemy.GetPosition() + enemy.GetDirection() + SkullRight - enemy.GetUp();
                    
                    foreach (var mesh in Skull.Meshes)
                    {
                        foreach (var part in mesh.MeshParts)
                        {
                            Texture2D texturePart = null;

                            if (part.Effect.Parameters["Texture"] != null) texturePart = part.Effect.Parameters["Texture"].GetValueTexture2D();
                            
                            part.Effect = SkeletonEffect;

                            // We set the main matrices for each mesh to draw
                            var worldMatrix = mesh.ParentBone.Transform;

                            if (texturePart != null) SkeletonEffect.Parameters["baseTexture"].SetValue(texturePart);

                            SkeletonEffect.Parameters["World"].SetValue(Matrix.CreateScale(0.25f) * worldMatrix * Matrix.CreateWorld(SkullPosition, -SkullRight, enemy.GetUp()));
                            SkeletonEffect.Parameters["View"].SetValue(Camera.View);
                            SkeletonEffect.Parameters["Projection"].SetValue(Camera.Projection);
                            // InverseTransposeWorld is used to rotate normals
                            SkeletonEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));
                        }
                        mesh.Draw();
                    }
                }
            }

            foreach (Bullet bullet in Bullets)
            {
                Vector3 BulletRight = Vector3.Cross(bullet.GetDirection(), bullet.GetUp());
                Vector3 BulletPosition = bullet.GetPosition() + bullet.GetDirection() + BulletRight - bullet.GetUp();
                if (Vector3.Distance(bullet.GetPosition(), Camera.Position) < 2550) BulletModel.Draw(Matrix.CreateScale(5) * Matrix.CreateWorld(BulletPosition, -BulletRight, bullet.GetDirection()), Camera.View, Camera.Projection);
            }

            if (Recoil > 0)
            {
                Recoil -= 0.25f;
            }

            Vector3 cameraRight = Vector3.Cross(Camera.FrontDirection, Camera.UpDirection);
            Vector3 weaponPosition = new Vector3(Camera.Position.X, 0, Camera.Position.Z) + new Vector3(0, -15, 0) + Camera.FrontDirection * MathHelper.Lerp(40, 35, Recoil) + cameraRight * 10 - Camera.UpDirection * 4;

            ShotgunEffect.Parameters["ambientColor"].SetValue(Color.Red.ToVector3());
            ShotgunEffect.Parameters["diffuseColor"].SetValue(Color.OrangeRed.ToVector3());
            ShotgunEffect.Parameters["specularColor"].SetValue(Color.Wheat.ToVector3());

            ShotgunEffect.Parameters["KAmbient"].SetValue(0.2f);
            ShotgunEffect.Parameters["KDiffuse"].SetValue(0.64f);
            ShotgunEffect.Parameters["KSpecular"].SetValue(0.5f);
            ShotgunEffect.Parameters["shininess"].SetValue(196.078431f);
            
            
            foreach (var mesh in Shotgun.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    Texture2D texturePart = null;

                    if (part.Effect.Parameters["Texture"] != null) texturePart = part.Effect.Parameters["Texture"].GetValueTexture2D();

                    part.Effect = ShotgunEffect;

                    // We set the main matrices for each mesh to draw
                    var worldMatrix = mesh.ParentBone.Transform;

                    if (texturePart != null) ShotgunEffect.Parameters["baseTexture"].SetValue(texturePart);
                    ShotgunEffect.Parameters["World"].SetValue(Matrix.CreateScale(0.1f) * Matrix.CreateWorld(weaponPosition, -cameraRight, Camera.UpDirection));
                    ShotgunEffect.Parameters["View"].SetValue(Camera.View);
                    ShotgunEffect.Parameters["Projection"].SetValue(Camera.Projection);
                    // InverseTransposeWorld is used to rotate normals
                    ShotgunEffect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(worldMatrix)));
                }
                mesh.Draw();
            }

            SpriteBatch.Begin();
            // Draw crosshair
            int crosshairWidth = 64;
            SpriteBatch.Draw(crosshair, new Rectangle(GraphicsDevice.Viewport.Width / 2 - 25, GraphicsDevice.Viewport.Height / 2 - 25, crosshairWidth, crosshairWidth), Color.White);
            SpriteBatch.End();

            SpriteBatch.Begin();
            SpriteBatch.DrawString(Font, "Health : " + (int)Player.GetLife(), new Vector2(GraphicsDevice.Viewport.Width - 250, GraphicsDevice.Viewport.Height - 100), Color.Red);
            SpriteBatch.End();

            SpriteBatch.Begin();
            SpriteBatch.DrawString(Font, "Souls Collected : " + Player.GetSoulsCollected(), new Vector2(GraphicsDevice.Viewport.Width / 2 - 100, GraphicsDevice.Viewport.Height - 100), Color.Red);
            SpriteBatch.End();

            base.Draw(gameTime);
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

        public void AddEnemy(Vector3 enemyPosition)
        {
            EnemyClass enemy = new EnemyClass();
            enemy.SetPosition(enemyPosition);

            Enemies.Add(enemy);
        }
    }
}
