using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GP3Project
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region User Defined Variables

        //enum to control screen selection
        private enum GameState
        {
            titleScreen = 0,
            gamePlay,
            gameEnd,
            gameFail,
            info,
        }
        GameState currentGameState = GameState.titleScreen;

        //graphics properties
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;
        Effect effect;

        //
        //------------------------------------------
        // Added for use with fonts
        //------------------------------------------
        SpriteFont fontToUse;

        //--------------------------------------------------
        // Added for use with playing Audio via Media player
        //--------------------------------------------------
        private Song backgroundMusic;
        private Song menuMusic;

        //--------------------------------------------------
        //Set the sound effects to use
        //--------------------------------------------------
        private SoundEffectInstance sirenSoundInstance;
        private SoundEffectInstance boostingSoundInstance;
        private SoundEffect sheepSound;
        private SoundEffect sirenSound;
        private SoundEffect failSound;
        private SoundEffect successSound;
        private SoundEffect boosting;

        private bool toggleSound=false;

        // The aspect ratio determines how to scale 3d to 2d projection.
        public float aspectRatio;
        //

        // player model, matrices and position
        private Model playerModel;
        private Matrix[] playerModelTransforms;
        private Matrix playerModelTransform;
        private Vector3 playerModelPosition;
        private float playerModelRotation;
        private Vector3 playerModelVelocity;

        // enemy position and variables
        private Vector3 enemyPos;
        private float enemyAngle;
        private Model enemyModel;
        private Matrix[] enemyModelTransforms;
        private float enemySpeed = 0.2f;

        // sheep models and array
        private Model sheepModel;
        private Matrix[] sheepModelTransforms;
        private Sheep[] sheepList = new Sheep[GameConstants.NumSheep];

        //random number generator
        private Random random = new Random();

        //keyboard state
        private KeyboardState oldKeyState;

        //new camera class
        Camera camera;

        //speed variable for calculations
        private double speedX, speedZ;
        
        //player game attributes
        private float booster = 1.0f;
        private int boost = 100;
        private int hitCount;
        private int health = 100;
        private int totalScore;

        //start and end screens
        private Texture2D backgroundTexture;
        private Texture2D foregroundTexture;
        private Texture2D failScreen;
        private Texture2D infoScreen;

        //skybox model and textures
        private Texture2D[] skyboxTextures;
        private Model skyboxModel;

        //terrain model
        private Model terrain;
        private Matrix[] terrainTransforms;
        
        //colour variable
        private Color colour;

        //screen dimensions
        private int screenWidth;
        private int screenHeight;

        //timer variables
        private float timer;
        private int timerCount;

        //handles all keyboard input
        private void KeyboardControl()
        {
            //handles keyboard state
            KeyboardState keyboardState = Keyboard.GetState();

            //handles keyboard control for title screen
            if (currentGameState == GameState.titleScreen)
            {
                if (keyboardState.IsKeyDown(Keys.Enter) && oldKeyState.IsKeyUp(Keys.Enter) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    //allows game to start on enter key being pressed
                    currentGameState = GameState.gamePlay;
                    MediaPlayer.Play(backgroundMusic);
                    MediaPlayer.IsRepeating = true;
                }
                if (keyboardState.IsKeyDown(Keys.I) && oldKeyState.IsKeyUp(Keys.I) || GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed)
                {
                    //allows info screen to load
                    currentGameState = GameState.info;
                }
            }

            //handles info screen
            if (currentGameState == GameState.info)
            {
                if (keyboardState.IsKeyDown(Keys.Enter) && oldKeyState.IsKeyUp(Keys.Enter)||GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    //allows game to start on enter key being pressed
                    currentGameState = GameState.gamePlay;
                    MediaPlayer.Play(backgroundMusic);//play music
                    MediaPlayer.IsRepeating = true;
                }
            }

            //handles keyboard control for fail screen
            if (currentGameState == GameState.gameFail)
            {
                if (keyboardState.IsKeyDown(Keys.Enter) && oldKeyState.IsKeyUp(Keys.Enter) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    //allows game to start on enter key being pressed
                    currentGameState = GameState.titleScreen;
                }
            }

            if (currentGameState == GameState.gameEnd)
            {
                if (keyboardState.IsKeyDown(Keys.Enter) && oldKeyState.IsKeyUp(Keys.Enter) || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                {
                    //allows game to start on enter key being pressed
                    currentGameState = GameState.titleScreen;
                }
            }

            //handles keyboard control for game play
            if(currentGameState==GameState.gamePlay)
            {
                //methods to allow angle to be calculated within 360 degrees
                if (playerModelRotation >= (MathHelper.Pi * 2))
                {
                    playerModelRotation = 0.0f;
                }
                if (playerModelRotation < 0)
                {
                    playerModelRotation = (MathHelper.Pi * 2);
                }

                //variables detemining x and z speed of player character
                speedX=(Math.Sin(playerModelRotation))*0.3f;
                speedZ=(Math.Cos(playerModelRotation))*0.3f;

                //variables to convert double calculations to float using cast
                float speedXDouble, speedZDouble;
                speedXDouble = (float)speedX;
                speedZDouble = (float)speedZ;

                //controls for player character
                if (keyboardState.IsKeyDown(Keys.Down) || GamePad.GetState(PlayerIndex.One).DPad.Down
                    == ButtonState.Pressed)
                {
                    //when down key is pressed move back
                    playerModelPosition += Vector3.Forward * speedZDouble * 0.5f;
                    playerModelPosition += Vector3.Left * speedXDouble * 0.5f;
                }

                if (keyboardState.IsKeyDown(Keys.Up)&&boost>0)
                {
                    //when up key is pressed move forward, boost also factored into calculation
                    playerModelPosition += Vector3.Backward * speedZDouble*1.5f*booster;
                    playerModelPosition += Vector3.Right * speedXDouble*1.5f*booster;
                }

                if (keyboardState.IsKeyDown(Keys.Up)&&boost==0)
                {
                    //stops boost once it runs out
                    playerModelPosition += Vector3.Backward * speedZDouble * 1.5f;
                    playerModelPosition += Vector3.Right * speedXDouble * 1.5f;
                }

                if (keyboardState.IsKeyDown(Keys.Right) && keyboardState.IsKeyDown(Keys.Up) || GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                {
                    //when up key and right key pressed move forward while rotating right
                    playerModelRotation -= 10 * MathHelper.ToRadians(0.2f);
                }

                if (keyboardState.IsKeyDown(Keys.Left) && keyboardState.IsKeyDown(Keys.Up) || GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed)
                {
                    //when up key and left are pressed move forward while rotating left
                    playerModelRotation += 10 * MathHelper.ToRadians(0.2f);
                }

                if (keyboardState.IsKeyDown(Keys.Down) && keyboardState.IsKeyDown(Keys.Right) || GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed)
                {
                    //when down key and right are pressed move back and rotate back right
                    playerModelPosition += Vector3.Forward * speedZDouble * 0.25f;
                    playerModelPosition += Vector3.Left * speedXDouble * 0.25f;
                    playerModelRotation += 10 * MathHelper.ToRadians(0.25f);
                }

                if (keyboardState.IsKeyDown(Keys.Down) && keyboardState.IsKeyDown(Keys.Left) || GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                {
                    //when down key and left are pressed move back and rotate back left
                    playerModelPosition += Vector3.Forward * speedZDouble * 0.25f;
                    playerModelPosition += Vector3.Left * speedXDouble * 0.25f;
                    playerModelRotation -= 10 * MathHelper.ToRadians(0.25f);
                }

                if (keyboardState.IsKeyDown(Keys.C) && oldKeyState.IsKeyUp(Keys.C) || GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed)
                {
                    //when c is pressed switch the camera
                    camera.SwitchCameraMode();
                }

                if (keyboardState.IsKeyDown(Keys.B)&&boost>0)
                {
                    //when b is pressed and boost available, boost speed
                    booster = 2.0f;
                    boost--;
                    boostingSoundInstance.Play();
                }

                if (keyboardState.IsKeyUp(Keys.B) || GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                {
                    //when b is released reduce speed
                    booster = 1.0f; ;
                }
            }

            if (keyboardState.IsKeyDown(Keys.M) && oldKeyState.IsKeyUp(Keys.M) || GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed)
            {
                //when m is pressed toggle music/sound effects on and off
                if (toggleSound == false)
                {
                    MediaPlayer.IsMuted = true;
                    SoundEffect.MasterVolume = 0.0f;
                    toggleSound = true;
                }
                else if (toggleSound == true)
                {
                    MediaPlayer.IsMuted = false;
                    SoundEffect.MasterVolume = 1.0f;
                    toggleSound = false;
                }
            }

            //save last key state
            oldKeyState = keyboardState;
        }

        private void ResetGame()
        {
            //variables to be reset on new game
            enemySpeed = 0.2f;
            totalScore = 0;
            timerCount = 0;
            boost = 100;
            health = 100;
            hitCount = 0;
            playerModelPosition = new Vector3(20.0f, 0.0f, -20.0f);
            playerModelRotation = 0.0f;
            playerModelVelocity = Vector3.Zero;
            enemyPos = new Vector3(-20.0f, 3.20f, 20.0f);


            //starting co-ordinates for sheep
            float xStart;
            float zStart;
            for (int i = 0; i < GameConstants.NumSheep; i++)
            {
                if (random.Next(2) == 0)
                {
                    xStart = (float)-GameConstants.PlayfieldSizeX;
                }
                else
                {
                    xStart = (float)GameConstants.PlayfieldSizeX;
                }
                zStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeZ;
                double angle = random.NextDouble() * 2 * Math.PI;
                sheepList[i].direction.X = -(float)Math.Sin(angle);
                sheepList[i].direction.Z = (float)Math.Cos(angle);
                sheepList[i].speed = GameConstants.SheepMinSpeed +
                   (float)random.NextDouble() * GameConstants.SheepMaxSpeed;
                sheepList[i].isActive = true;
            }
        }

        //collates all the bones in a model to allow easier rotation
        private Matrix[] SetupEffectTransformDefaults(Model myModel)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = camera.projectionMatrix;
                    effect.View = camera.viewMatrix;
                }
            }
            return absoluteTransforms;
        }

        //calculates the final score using the time and health variables
        private void calculateScore(int ti, int hea)
        {
            int t = (1000 - ti)/100;
            totalScore = hea * t;
        }

        public void DrawModel(Model model, Matrix modelTransform, Matrix[] absoluteBoneTransforms)
        {
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        //write text on screen
        private void writeText(string msg, Vector2 msgPos, Color msgColour)
        {
            if (currentGameState == GameState.titleScreen)
            {
                spriteBatch.Begin();
                string output = msg;
                // Find the center of the string
                Vector2 FontOrigin = fontToUse.MeasureString(output) / 2;
                Vector2 FontPos = msgPos;
                // Draw the string
                spriteBatch.DrawString(fontToUse, output, FontPos, msgColour);
                spriteBatch.End();
            }
            if (currentGameState == GameState.gamePlay)
            {
                spriteBatch.Begin();
                string output = msg;
                // Find the center of the string
                Vector2 FontOrigin = fontToUse.MeasureString(output) / 2;
                Vector2 FontPos = msgPos;
                // Draw the string
                spriteBatch.DrawString(fontToUse, output, FontPos, msgColour);
                spriteBatch.End();
            }
        }

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = false;
            Window.Title = "Sheep Harvest";
            hitCount = 0;
           
            ResetGame();
            base.Initialize();
        }

        //different method to load skybox models so that they contain the textures as an array
        private Model LoadModel(string assetName, out Texture2D[] textures)
        {
            //load model 
            Model newModel = Content.Load<Model>(assetName);
            textures = new Texture2D[newModel.Meshes.Count];
            int i = 0;
            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    textures[i++] = currentEffect.Texture;

            //set up model
            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();

            return newModel;
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //determine viewport and aspect ratio
            Viewport viewP = graphics.GraphicsDevice.Viewport;
            aspectRatio = (float)viewP.Width/(float)viewP.Height;

            //start camera
            camera = new Camera(aspectRatio);

            //load start and end screens
            backgroundTexture = Content.Load<Texture2D>(".\\Images\\startScreen");
            foregroundTexture = Content.Load<Texture2D>(".\\Images\\complete");
            failScreen = Content.Load<Texture2D>(".\\Images\\fail");
            infoScreen = Content.Load<Texture2D>(".\\Images\\controls");

            //load font
            fontToUse = Content.Load<SpriteFont>(".\\Fonts\\spriteFont1");

            //load game music
            backgroundMusic = Content.Load<Song>(".\\Audio\\yaketysax");
            menuMusic = Content.Load<Song>(".\\Audio\\menu");
            MediaPlayer.Play(menuMusic);
            MediaPlayer.IsRepeating = true;

            //load models
            playerModel = Content.Load<Model>(".\\Models\\e100");
            playerModelTransforms = SetupEffectTransformDefaults(playerModel);
            sheepModel = Content.Load<Model>(".\\Models\\sheep");
            sheepModelTransforms = SetupEffectTransformDefaults(sheepModel);
            terrain = Content.Load<Model>(".\\Models\\Road");
            terrainTransforms = SetupEffectTransformDefaults(terrain);
            enemyModel = Content.Load<Model>(".\\Models\\turbosonic");
            enemyModelTransforms = SetupEffectTransformDefaults(enemyModel);
                
            //load sound
            sheepSound = Content.Load<SoundEffect>("Audio\\ba");
            sirenSound = Content.Load<SoundEffect>("Audio\\siren");
            failSound = Content.Load<SoundEffect>("Audio\\1fail");
            successSound = Content.Load<SoundEffect>("Audio\\1success");
            boosting = Content.Load<SoundEffect>("Audio\\zoom");
            sirenSoundInstance = sirenSound.CreateInstance();
            boostingSoundInstance = boosting.CreateInstance();

            //load skybox textures
            effect=Content.Load<Effect>("Effects\\effects");
            skyboxModel = LoadModel(".\\Models\\skybox",out skyboxTextures);
            device = graphics.GraphicsDevice;
            screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        //draws a skybox around the world
        private void DrawSkybox()
        {
            //sets clamps for textures
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            device.SamplerStates[0] = ss;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            device.DepthStencilState = dss;

            //set up skybox
            Matrix[] skyboxTransforms = new Matrix[skyboxModel.Bones.Count];
            skyboxModel.CopyAbsoluteBoneTransformsTo(skyboxTransforms);
            int i = 0;
            foreach (ModelMesh mesh in skyboxModel.Meshes)
            {
                //add effects to skybox
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = skyboxTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(15.0f) * Matrix.CreateTranslation(camera.positionCAM);
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(camera.viewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(camera.projectionMatrix);
                    currentEffect.Parameters["xTexture"].SetValue(skyboxTextures[i++]);
                }
                mesh.Draw();
            }

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            device.DepthStencilState = dss;
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (currentGameState == GameState.titleScreen)
            {
                ResetGame();
            }

           
            //check to see if the game is to be player
            if (currentGameState == GameState.gamePlay)
            {
                //load the models
                playerModelTransforms = SetupEffectTransformDefaults(playerModel);
                sheepModelTransforms = SetupEffectTransformDefaults(sheepModel);
                terrainTransforms = SetupEffectTransformDefaults(terrain);
                enemyModelTransforms = SetupEffectTransformDefaults(enemyModel);
                camera.Update(playerModelTransform, playerModelPosition, playerModelRotation);

                //start timer
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                timerCount += (int)timer;
                if (timer >= 1.0f)
                {
                    timer = 0.0f;
                }
                
                //prevent player model from leaving screen
                if (playerModelPosition.X > 65)
                {
                    playerModelPosition.X = 65;
                }
                if (playerModelPosition.X < -65)
                {
                    playerModelPosition.X = -65;
                }
                if (playerModelPosition.Z > 37)
                {
                    playerModelPosition.Z = 37;
                }
                if (playerModelPosition.Z < -37)
                {
                    playerModelPosition.Z = -37;
                }

                float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

                //update sheep
                for (int i = 0; i < GameConstants.NumSheep; i++)
                {
                    sheepList[i].Update(timeDelta);
                }

                //create bounding sphere around player
                BoundingSphere playerSphere =
                  new BoundingSphere(playerModelPosition,
                           4.0f *
                                 GameConstants.PlayerBoundingSphereScale);

                //calculate angle to determine facing position for model
                Vector3 totDir = playerModelPosition -enemyPos;
                totDir.Normalize();
                enemyPos += totDir * enemySpeed;
                enemyAngle = (float)Math.Atan2(-totDir.X,-totDir.Z);

                //create bounding sphere sphere around enemy
                BoundingSphere enemySphere = new BoundingSphere(enemyPos, 5.0f);

                if (enemySphere.Intersects(playerSphere))
                {
                    health--; //if enemy and player sphere are colliding reduce health
                    sirenSoundInstance.Play(); //play sound
                }

                if (health < 0)
                {
                    currentGameState = GameState.gameFail;//if health is less than 100 load fail screen
                    failSound.Play();
                    MediaPlayer.Play(menuMusic);
                }

                for (int i = 0; i < sheepList.Length; i++)
                {
                    if (sheepList[i].isActive)
                    {
                        //create bounding sphere for each sheep
                        BoundingSphere sheepSphereA =
                          new BoundingSphere(sheepList[i].position, sheepModel.Meshes[0].BoundingSphere.Radius *
                                         GameConstants.SheepBoundingSphereScale);

                        if (sheepSphereA.Intersects(playerSphere)) //Check collision between player and sheep
                        {
                            sheepSound.Play(); //play sheep noise
                            sheepList[i].isActive = false; //destroy sheep
                            hitCount++; //increase hit
                            enemySpeed += 0.02f; //increase enemy speed
                            boost += 10; //increase boost

                            if (boost > 100)
                            {
                                boost = 100; //of boost is more than 100, cap at 100
                            }

                            if (hitCount == 10)
                            {
                                calculateScore(timerCount,health);
                                currentGameState = GameState.gameEnd; //end game if all sheep caught
                                MediaPlayer.Play(menuMusic);
                                successSound.Play();
                            }
                        }
                    }
                }
            }
            
            //check for keyboard control
            KeyboardControl();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        //method to draw background images
        private void DrawScenery()
        {
            if (currentGameState == GameState.titleScreen)
            {
                Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
                spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            }
            if (currentGameState == GameState.gameEnd)
            {
                Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
                spriteBatch.Draw(foregroundTexture, screenRectangle, Color.White);
                spriteBatch.DrawString(fontToUse,totalScore.ToString()+" CASH", new Vector2(350, 300), Color.Black);
            }
            if (currentGameState == GameState.gameFail)
            {
                Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
                spriteBatch.Draw(failScreen, screenRectangle, Color.White);
            }
            if (currentGameState == GameState.info)
            {
                Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
                spriteBatch.Draw(infoScreen, screenRectangle, Color.White);
            }
        }
        //method to draw models and textures
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //if screen requires background image use this method
            if (currentGameState == GameState.titleScreen || currentGameState == GameState.gameEnd|| currentGameState==GameState.gameFail||currentGameState==GameState.info)
            {
                spriteBatch.Begin();
                DrawScenery();
                spriteBatch.End();
            }

            //handles gameplay drawing
            if (currentGameState == GameState.gamePlay)
            {
                //set up for drawing skybox
                device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
                DrawSkybox();

                //handle drawing of sheep
                for (int i = 0; i < GameConstants.NumSheep; i++)
                {
                    if (sheepList[i].isActive)
                    {
                        Vector3 line = sheepList[i].direction;
                        float rotationDal= (float)Math.Atan2(sheepList[i].direction.X,sheepList[i].direction.Z);
                     
                        Matrix dalekTransform = Matrix.CreateScale(GameConstants.SheepScalar) * Matrix.CreateRotationY(rotationDal) * Matrix.CreateTranslation(sheepList[i].position);
                        DrawModel(sheepModel, dalekTransform, sheepModelTransforms);
                    }
                }

                //draw player model
                Matrix modelTransform = Matrix.CreateScale(0.002f) * Matrix.CreateRotationY(playerModelRotation) * Matrix.CreateTranslation(playerModelPosition);
                DrawModel(playerModel, modelTransform, playerModelTransforms);

                //draw terrain
                Matrix terrainTransform = Matrix.CreateScale(2.0f) *  Matrix.CreateTranslation(Vector3.Zero);
                DrawModel(terrain, terrainTransform, terrainTransforms);

                //draw enemy model
                Matrix enemyTransform = Matrix.CreateScale(0.01f) * Matrix.CreateRotationY(enemyAngle) * Matrix.CreateTranslation(enemyPos);
                DrawModel(enemyModel, enemyTransform, enemyModelTransforms);

                //if health is greater than 75 the colour to use will be yellowgreen
                if (health > 75)
                {
                    colour = Color.YellowGreen;
                }
                
                if (health > 25 && health <75)
                {
                    colour = Color.Yellow;//cahnge colour to yellow
                }
                else if (health < 25)
                {
                    colour = Color.Red;//change clolour to red
                }
                int sheepLeft = 10 - hitCount;//show number of sheep left to kill

                writeText("Health: "+health.ToString(), new Vector2(10, 10), colour);//show health
                writeText("Sheep Left: "+sheepLeft.ToString(), new Vector2(550, 10), Color.AntiqueWhite);//show sheep left
                writeText("Time: "+timerCount.ToString(), new Vector2(350, 10), Color.Pink);//show time lappsed
                writeText("Boost: "+boost.ToString(),new Vector2(600, 400),Color.AntiqueWhite);//show boost available
            }

            base.Draw(gameTime);
        }
    }
}
