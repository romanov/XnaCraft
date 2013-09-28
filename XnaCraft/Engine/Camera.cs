using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaCraft.Engine
{
    public class Camera
    {
        private readonly GraphicsDevice _device;

        private Vector3 _position = new Vector3(0, 12, 0);
        private float _leftRightRotation = MathHelper.ToRadians(-135);
        private float _upDownRotation = MathHelper.ToRadians(-20);

        private float _moveSpeed = 5.0f;
        private float _rotationSpeed = 0.1f;

        public Matrix View { get; set; }
        public Matrix Projection { get; set; }

        public Vector3 Position
        {
            get
            {
                return _position;
            }
        }

        public Camera(GraphicsDevice device)
        {
            _device = device;
        }

        public void Update(GameTime gameTime, float dx, float dy, Vector3 moveVector)
        {
            _leftRightRotation -= (dx / 50)  * _rotationSpeed;
            _upDownRotation -= (dy / 50) * _rotationSpeed;

            _upDownRotation = MathHelper.Clamp(_upDownRotation, -MathHelper.PiOver2, MathHelper.PiOver2);

            var rotation = Matrix.CreateRotationX(_upDownRotation) * Matrix.CreateRotationY(_leftRightRotation);
            var direction = Vector3.Transform(Vector3.Forward, rotation);

            _position += Vector3.Transform(moveVector * _moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, Matrix.CreateRotationY(_leftRightRotation));

            View = Matrix.CreateLookAt(_position, _position + direction, Vector3.Transform(Vector3.Up, rotation));
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _device.Viewport.AspectRatio, 1.0f, 1000);
        }
    }
}
