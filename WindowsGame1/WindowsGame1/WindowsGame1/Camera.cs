using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace GP3Project
{
    class Camera
    {
        //enum to select a camera to use
        public enum CameraMode
        { 
            planView = 0,
            firstPerson = 1,
            cinematic = 2
        }
        //current camera
        public CameraMode currentCameraMode=CameraMode.planView;

        //camera matrices
        public Matrix viewMatrix;
        public Matrix projectionMatrix;

        //camera vectors
        public Vector3 positionCAM;
        private Vector3 target;
        private Vector3 cameraUp;

        //camera rotation matrix
        private Matrix cameraRotation;

        //camera constructor
        public Camera(float aspectRatio)
        {   
            //creates projection matrix using defined aspect ratio
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), aspectRatio , 1.0f, 1000f);
        }

        //camera update using new values
        public void Update(Matrix chasedObjectsWorld, Vector3 mdlPos, float mdlRotation)
        {
            UpdateViewMatrix(chasedObjectsWorld, mdlPos, mdlRotation);
        }

        private void UpdateViewMatrix(Matrix chasedObjectsWorld, Vector3 mdlPos, float mdlRotation)
        {
            //select camera using switch
            switch (currentCameraMode)
            {
                case CameraMode.planView:

                    //code

                    cameraRotation = Matrix.CreateRotationY(mdlRotation);//camera rotation
                    positionCAM = new Vector3(0.0f, 100.0f, 0.0f); //camera position
                    cameraUp = new Vector3(0.0f, 0.0f, -1.0f); //camera up vector
                    target = cameraRotation.Forward; //target to look at
                    viewMatrix = Matrix.CreateLookAt(positionCAM, target, cameraUp);  //new view matrix
               
                    break;

                case CameraMode.firstPerson:

                    //code

                    Vector3 targetA;
                    cameraRotation = Matrix.CreateRotationY(mdlRotation); //create rotation to follow player

                    positionCAM = new Vector3(0.0f, 10.0f, -20.0f);
                    cameraUp = new Vector3(0.0f,1.0f,0.0f);
                    targetA = Vector3.Transform(positionCAM,cameraRotation);
                    positionCAM = targetA + mdlPos;
                    target = positionCAM + targetA;
                    viewMatrix = Matrix.CreateLookAt(positionCAM, mdlPos, Vector3.Up);

                    break;

                case CameraMode.cinematic:

                    //code
                    positionCAM = new Vector3(mdlPos.X + 0.35f, mdlPos.Y + 1.8f, mdlPos.Z - 15.0f);
                    cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
                    target = new Vector3(mdlPos.X, mdlPos.Y, mdlPos.Z);
                    viewMatrix = Matrix.CreateLookAt(positionCAM, target, cameraUp);

                    break;
            }            
        }

        //method to switch the camera
        public void SwitchCameraMode()
        {
            currentCameraMode++;
            if ((int)currentCameraMode > 2)
            {
                currentCameraMode = 0;
            }
        }
    }
}
