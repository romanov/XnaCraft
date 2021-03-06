﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XnaCraft.Engine.Framework
{
    public class Camera
    {
        private readonly GraphicsDevice _device;

        private Vector3 _position = new Vector3(0, 12, 0);
        private float _leftRightRotation = MathHelper.ToRadians(-135);
        private float _upDownRotation = MathHelper.ToRadians(-20);
        private Vector3 _direction;
        private Matrix _rotation;

        private const float RotationSpeed = 0.1f;

        public Matrix View { get; set; }
        public Matrix Projection { get; set; }

        public float LeftRightRotation
        {
            get
            {
                return _leftRightRotation;
            }
        }

        public float UpDownRotation
        {
            get
            {
                return _upDownRotation;
            }
        }

        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        public Vector3 Direction
        {
            get
            {
                return _direction;
            }
        }

        public Ray Ray
        {
            get
            {
                return new Ray(_position, _direction);
            }
        }

        public Camera(GraphicsDevice device)
        {
            _device = device;
        }

        public void MoveTo(Vector3 position)
        {
            _position = position;
        }

        public void UpdateRotation(float leftRightDelta, float upDownDelta)
        {
            _leftRightRotation -= (leftRightDelta / 50) * RotationSpeed;
            _upDownRotation -= (upDownDelta / 50) * RotationSpeed;

            _upDownRotation = MathHelper.Clamp(_upDownRotation, -MathHelper.PiOver2, MathHelper.PiOver2);

            _rotation = Matrix.CreateRotationX(_upDownRotation) * Matrix.CreateRotationY(_leftRightRotation);
            _direction = Vector3.Transform(Vector3.Forward, _rotation);
        }

        public void Update()
        {
            View = Matrix.CreateLookAt(_position, _position + _direction, Vector3.Transform(Vector3.Up, _rotation));
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _device.Viewport.AspectRatio, 0.0001f, 1000);
        }
    }
}
