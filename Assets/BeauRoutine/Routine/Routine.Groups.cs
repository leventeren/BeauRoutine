﻿/*
 * Copyright (C) 2016-2017. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    21 Nov 2016
 * 
 * File:    Routine.Groups.cs
 * Purpose: API for pausing, resuming, and adjusting time scale
 *          for routine groups.
*/

using System;
using BeauRoutine.Internal;

namespace BeauRoutine
{
    public partial struct Routine
    {
        /// <summary>
        /// Maximum number of groups allowed.
        /// </summary>
        public const int MAX_GROUPS = 32;

        /// <summary>
        /// Pauses routines in the given groups.
        /// </summary>
        static public void PauseGroups(int inGroupMask)
        {
            Manager m = GetManager();
            if (m != null)
                m.PauseGroups(inGroupMask);
        }

        /// <summary>
        /// Resumes routines in the given groups.
        /// </summary>
        static public void ResumeGroups(int inGroupMask)
        {
            Manager m = GetManager();
            if (m != null)
                m.ResumeGroups(inGroupMask);
        }

        /// <summary>
        /// Returns the pause state of the given group.
        /// </summary>
        static public bool GetGroupPaused(int inGroup)
        {
            Manager m = GetManager();
            if (m != null)
            {
                if (inGroup >= MAX_GROUPS)
                    throw new ArgumentException(string.Format("Invalid group index {0} - only {1} are allowed", inGroup, MAX_GROUPS));
                return m.GetPaused(inGroup);
            }
            return false;
        }
        
        /// <summary>
        /// Gets the time scale for the given group.
        /// </summary>
        static public float GetGroupTimeScale(int inGroup)
        {
            Manager m = GetManager();
            if (m != null)
            {
                if (inGroup >= MAX_GROUPS)
                    throw new ArgumentException(string.Format("Invalid group index {0} - only {1} are allowed", inGroup, MAX_GROUPS));
                return m.GetTimescale(inGroup);
            }
            return 1.0f;
        }

        /// <summary>
        /// Sets the time scale for the given group.
        /// </summary>
        static public void SetGroupTimeScale(int inGroup, float inScale)
        {
            Manager m = GetManager();
            if (m != null)
            {
                if (inGroup >= MAX_GROUPS)
                    throw new ArgumentException(string.Format("Invalid group index {0} - only {1} are allowed", inGroup, MAX_GROUPS));
                m.SetTimescale(inGroup, inScale);
            }
        }

        /// <summary>
        /// Resets the time scale for the given group.
        /// </summary>
        static public void ResetGroupTimeScale(int inGroup)
        {
            Manager m = GetManager();
            if (m != null)
            {
                if (inGroup >= MAX_GROUPS)
                    throw new ArgumentException(string.Format("Invalid group index {0} - only {1} are allowed", inGroup, MAX_GROUPS));
                m.SetTimescale(inGroup, 1.0f);
            }
        }

        /// <summary>
        /// Returns the group mask for the given group.
        /// </summary>
        static public int GetGroupMask(int inGroup)
        {
            return 1 << inGroup;
        }

        /// <summary>
        /// Returns the group mask for the given groups.
        /// </summary>
        static public int GetGroupMask(int inGroupA, params int[] inGroups)
        {
            int mask = 1 << inGroupA;
            for (int i = 0; i < inGroups.Length; ++i)
                mask |= 1 << inGroups[i];
            return mask;
        }
    }
}
