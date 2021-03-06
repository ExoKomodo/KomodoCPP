﻿using Komodo.Lib.Math;

using BoundingBox = Microsoft.Xna.Framework.BoundingBox;

namespace Komodo.Core.Physics
{
    /// <summary>
    /// A box shape to be used in physics simulation.
    /// </summary>
    public class Box : IPhysicsShape
    {
        #region Public
        
        #region Constructors
        public Box(float width, float height, float depth, float mass)
        {
            Width = width;
            Height = height;
            Depth = depth;
            Mass = mass;
        }

        public Box(Vector3 dimensions, float mass)
        {
            Width = dimensions.X;
            Height = dimensions.Y;
            Depth = dimensions.Z;
            Mass = mass;
        }
        #endregion

        #region Members
        /// <summary>
        /// Depth of the Box.
        /// </summary>
        /// <remarks>
        /// Updates MomentOfInertia when changed.
        /// </remarks>
        public float Depth
        {
            get
            {
                return _depth;
            }
            set
            {
                _depth = value;
                UpdateMomentOfInertia();
            }
        }

        /// <summary>
        /// Height of the Box.
        /// </summary>
        /// <remarks>
        /// Updates MomentOfInertia when changed.
        /// </remarks>
        public float Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                UpdateMomentOfInertia();
            }
        }

        /// <summary>
        /// Mass of the Box.
        /// </summary>
        /// <remarks>
        /// Updates MomentOfInertia when changed.
        /// </remarks>
        public float Mass
        {
            get
            {
                return _mass;
            }
            set
            {
                if (value > 0)
                {
                    _mass = value;
                    UpdateMomentOfInertia();
                }
            }
        }

        /// <summary>
        /// Moment of inertia for the Box.
        /// </summary>
        public Vector3 MomentOfInertia { get; private set; }

        /// <summary>
        /// Width of the Box.
        /// </summary>
        /// <remarks>
        /// Updates MomentOfInertia when changed.
        /// </remarks>
        public float Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                UpdateMomentOfInertia();
            }
        }
        #endregion

        #region Member Methods
        public IPhysicsShape GetScaledShape(Vector3 scale)
        {
            return new Box(Width * scale.X, Height * scale.Y, Depth * scale.Z, Mass);
        }
        #endregion

        #endregion

        #region Private

        #region Members
        private float _depth { get; set; }
        private float _height { get; set; }
        private float _mass { get; set; }
        private float _width { get; set; }
        #endregion

        #region Member Methods
        /// <summary>
        /// Updates the moment of intertia. Called whenever relevant fields have been updated.
        /// </summary>
        private void UpdateMomentOfInertia()
        {
            float x = Mass * (Height * Height + Depth * Depth) / 12f;
            float y = Mass * (Width * Width + Depth * Depth) / 12f;
            float z = Mass * (Width * Width + Height * Height) / 12f;
            MomentOfInertia = new Vector3(x, y, z);
        }
        #endregion

        #endregion
    }
}