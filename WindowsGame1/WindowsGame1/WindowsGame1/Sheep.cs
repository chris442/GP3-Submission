using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GP3Project
{
    struct Sheep
    {
        public Vector3 position;
        public Vector3 direction;
        public float speed;
        public bool isActive;

        public void Update(float delta)
        {
            position += direction * speed *
                        GameConstants.SheepSpeedAdjustment * delta;

            //change direction if sheep leaves screen co-ordinates
            if (position.X > 40.0f && position.Z < 0)
            {
                direction.X -= delta;
                direction.Z += delta;
            }
            if (position.X > 40.0f && position.Z > 0)
            {
                direction.X -= delta;
                direction.Z -= delta;
            }

            if (position.X < -40.0f && position.Z < 0)
            {
                direction.X += delta;
                direction.Z += delta;
            }
            if (position.X < -40.0f && position.Z > 0)
            {
                direction.X += delta;
                direction.Z -= delta;
            }

            if (position.X < 0.0f && position.Z < -25.0f)
            {
                direction.X += delta;
                direction.Z += delta;
            }
            if (position.X > 0.0f && position.Z > 25.0f)
            {
                direction.X -= delta;
                direction.Z -= delta;
            }

            if (position.X > 0.0f && position.Z < -25.0f)
            {
                direction.X -= delta;
                direction.Z += delta;
            }
            if (position.X < 0.0f && position.Z > 25.0f)
            {
                direction.X += delta;
                direction.Z -= delta;
            }
        }
    }
}
