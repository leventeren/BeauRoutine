﻿/*
 * Copyright (C) 2016-2018. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    11 May 2018
 * 
 * File:    SplineLerpSpace.cs
 * Purpose: Enum describing how interpolation percentages
            should be applied to a spline.
*/

namespace BeauRoutine.Splines
{
    /// <summary>
    /// How a spline should be interpolated.
    /// </summary>
    public enum SplineLerp : byte
    {
        /// <summary>
        /// The duration of each segment is equal.
        /// Interpolation is evaluated in time space.
        /// </summary>
        Vertex,

        /// <summary>
        /// The duration of each segment is proportional
        /// to its direct distance relative to the total direct distance.
        /// Interpolation is evaluated in vertex distance space.
        /// </summary>
        Direct,

        /// <summary>
        /// The duration of each segment is proportional
        /// to its precise distance relative to the total precise distance.
        /// Interpolation is evaluated in an estimated arc-length space.
        /// </summary>
        Precise
    }
}
