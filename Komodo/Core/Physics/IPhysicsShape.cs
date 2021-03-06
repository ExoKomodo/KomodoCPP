﻿using Komodo.Lib.Math;

namespace Komodo.Core.Physics
{
    /// <summary>
    /// Interface for all physics shapes.
    /// </summary>
    public interface IPhysicsShape
    {
        #region Members
        /// <summary>
        /// Mass of the given shape.
        /// </summary>
        float Mass { get; set; }
        /// <summary>
        /// Moment of inertia for the given shape. This is usually a calculated value based on mass, location of vertices, etc.
        /// </summary>
        Vector3 MomentOfInertia { get; }
        #endregion

        #region Member Methods
        IPhysicsShape GetScaledShape(Vector3 scale);
        #endregion
    }
}
