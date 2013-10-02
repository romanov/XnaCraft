using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XnaCraft.Engine
{
    class Player
    {
        private float _width = 0.8f;
        private float _height = 1.8f;
        private float _speed = 4f;
        private float _jumpSpeed = 5f;

        private Vector3 _position;
        private float _rotation;
        private BoundingBox _boundingBox;

        private readonly World _world;

        private const float G = 10;
        private float _downfallSpeed = 0;

        public Vector3 Position { get { return _position; } }

        public Player(World world, Vector3 position)
        {
            _world = world;
            _position = position;
        }

        private void CreateBoundingBox()
        {
            var halfWidth = _width / 2;
            var halfHeight = _height / 2;

            _boundingBox = new BoundingBox(
                _position + new Vector3(-halfWidth, -halfHeight, -halfWidth),
                _position + new Vector3(halfWidth, halfHeight, halfWidth));
        }
        
        public void Move(GameTime gameTime, Vector3 moveVector, float rotation, bool jump)
        {
            var oldPosition = _position;

            var moveOffset = Vector3.Transform(moveVector * _speed, Matrix.CreateRotationY(rotation));

            _rotation = rotation;

            _position += new Vector3(moveOffset.X, 0, 0);

            CreateBoundingBox();

            if (_world.CheckCollision(_boundingBox))
            {
                _position = oldPosition;
            }
            else
            {
                oldPosition = _position;
            }

            var elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;


            if (_downfallSpeed == 0 && jump)
            {
                _downfallSpeed = -_jumpSpeed;
            }
            else
            {
                _downfallSpeed += G * elapsedSeconds;
            }

            _position += new Vector3(0,  -_downfallSpeed * elapsedSeconds, 0);

            CreateBoundingBox();

            if (_world.CheckCollision(_boundingBox))
            {
                _position = oldPosition;
                _downfallSpeed = 0;
            }
            else
            {
                oldPosition = _position;
            }

            _position += new Vector3(0, 0, moveOffset.Z);

            CreateBoundingBox();

            if (_world.CheckCollision(_boundingBox))
            {
                _position = oldPosition;
            }
            else
            {
                oldPosition = _position;
            }
        }
    }
}
