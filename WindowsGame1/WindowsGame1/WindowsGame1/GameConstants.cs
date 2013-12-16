using System;
using System.Collections.Generic;
using System.Text;

namespace GP3Project
{
    static class GameConstants
    {
        //camera constants
        public const float CameraHeight = 25000.0f;
        public const float PlayfieldSizeX = 100f;
        public const float PlayfieldSizeZ = 300f;
        //Dalek constants
        public const int NumSheep = 10;
        public const float SheepMinSpeed = 3.0f;
        public const float SheepMaxSpeed = 10.0f;
        public const float SheepSpeedAdjustment = 2.5f;
        public const float SheepScalar = 0.01f;
        //collision constants
        public const float SheepBoundingSphereScale = 0.05f;  //50% size
        public const float PlayerBoundingSphereScale = 0.5f;  //50% size
    }
}
