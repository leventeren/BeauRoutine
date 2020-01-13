﻿/*
 * Copyright (C) 2016-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    4 Apr 2017
 * 
 * File:    RoutineUnityHost.cs
 * Purpose: Host behavior. Contains hooks for executing BeauRoutines.
 */

#if UNITY_EDITOR

#if UNITY_2017_2_OR_NEWER
#define USE_PAUSE_STATE_EVENT
#endif // UNITY_2017_2_OR_NEWER

using UnityEditor;

#endif // UNITY_EDITOR

using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace BeauRoutine.Internal
{
    [AddComponentMenu("")]
    internal sealed class RoutineUnityHost : MonoBehaviour
    {
        static private WaitForFixedUpdate s_CachedWaitForFixedUpdate = Routine.WaitForFixedUpdate();
        static private WaitForEndOfFrame s_CachedWaitForEndOfFrame = Routine.WaitForEndOfFrame();

        private Manager m_Manager;

        private Coroutine m_WaitForFixedUpdateRoutine;
        private Coroutine m_WaitForEndOfFrameRoutine;

        public void Initialize(Manager inManager)
        {
            m_Manager = inManager;

            if (m_WaitForEndOfFrameRoutine == null)
                m_WaitForEndOfFrameRoutine = StartCoroutine(ApplyWaitForEndOfFrame());

            RegisterCallbacks();
        }

        public void Shutdown()
        {
            StopYieldInstructions();
            m_Manager = null;

            DeregisterCallbacks();
        }

        private void RegisterCallbacks()
        {
            #if UNITY_EDITOR

            #if USE_PAUSE_STATE_EVENT
            EditorApplication.pauseStateChanged += OnEditorPauseChanged;
            #else
            EditorApplication.playmodeStateChanged += OnEditorPausedNoArgs;
            #endif // USE_PAUSE_STATE_EVENT

            #endif // UNITY_EDITOR
        }

        private void DeregisterCallbacks()
        {
            #if UNITY_EDITOR

            #if USE_PAUSE_STATE_EVENT
            EditorApplication.pauseStateChanged -= OnEditorPauseChanged;
            #else
            EditorApplication.playmodeStateChanged -= OnEditorPausedNoArgs;
            #endif // USE_PAUSE_STATE_EVENT

            #endif // UNITY_EDITOR
        }

        #region Unity Events

        private void OnApplicationQuit()
        {
            if (m_Manager != null)
            {
                StopYieldInstructions();
                m_Manager.OnApplicationQuit();
                m_Manager = null;
                Routine.Shutdown();
                DeregisterCallbacks();
            }
        }

        private void OnApplicationPause(bool inbPaused)
        {
            if (m_Manager != null)
            {
                m_Manager.SetAsyncPaused(inbPaused);
            }
        }

        private void OnDestroy()
        {
            if (m_Manager != null)
            {
                StopYieldInstructions();
                m_Manager = null;
                Routine.Shutdown();
            }
        }

        private void LateUpdate()
        {
            Manager m = m_Manager;
            float deltaTime = Time.deltaTime;

            if (m != null)
            {
                // lateupdate
                m.Update(deltaTime, RoutinePhase.LateUpdate);
                if (m.Fibers.GetYieldCount(YieldPhase.WaitForLateUpdate) > 0)
                    m.UpdateYield(deltaTime, YieldPhase.WaitForLateUpdate);
            }
        }

        private void Update()
        {
            m_Manager.MarkFrameStart();

            Manager m = m_Manager;
            float deltaTime = Time.deltaTime;
            float timestamp = Time.unscaledTime;

            if (m != null)
            {
                // update phase
                m.Update(deltaTime, RoutinePhase.Update);
                if (m.Fibers.GetYieldCount(YieldPhase.WaitForUpdate) > 0)
                    m.UpdateYield(deltaTime, YieldPhase.WaitForUpdate);

                // thinkupdate
                float thinkCustomDelta;
                if (m.AdvanceThinkUpdate(deltaTime, timestamp, out thinkCustomDelta))
                {
                    m.Update(thinkCustomDelta, RoutinePhase.ThinkUpdate);
                    if (m.Fibers.GetYieldCount(YieldPhase.WaitForThinkUpdate) > 0)
                        m.UpdateYield(thinkCustomDelta, YieldPhase.WaitForThinkUpdate);
                }

                // customupdate
                if (m.AdvanceCustomUpdate(deltaTime, timestamp, out thinkCustomDelta))
                {
                    m.Update(thinkCustomDelta, RoutinePhase.CustomUpdate);
                    if (m.Fibers.GetYieldCount(YieldPhase.WaitForCustomUpdate) > 0)
                        m.UpdateYield(thinkCustomDelta, YieldPhase.WaitForCustomUpdate);
                }

                // realtimeupdate
                float realDeltaTime = Time.unscaledDeltaTime;
                m.Update(realDeltaTime, RoutinePhase.RealtimeUpdate);
                if (m.Fibers.GetYieldCount(YieldPhase.WaitForRealtimeUpdate) > 0)
                    m.UpdateYield(realDeltaTime, YieldPhase.WaitForRealtimeUpdate);
            }
        }

        private void FixedUpdate()
        {
            m_Manager.MarkFrameStart();

            if (m_Manager != null)
            {
                // fixedupate
                m_Manager.Update(Time.deltaTime, RoutinePhase.FixedUpdate);
            }
        }

        #if UNITY_EDITOR

        #if USE_PAUSE_STATE_EVENT

        private void OnEditorPauseChanged(PauseState inState)
        {
            OnApplicationPause(inState == PauseState.Paused);
        }

        #else

        private void OnEditorPausedNoArgs()
        {
            OnApplicationPause(EditorApplication.isPaused);
        }

        #endif // USE_PAUSE_STATE_EVENT

        #endif // UNITY_EDITOR

        #endregion

        #region Yield Instructions

        public void WaitForFixedUpdate()
        {
            if (m_WaitForFixedUpdateRoutine == null)
                m_WaitForFixedUpdateRoutine = StartCoroutine(ApplyWaitForFixedUpdate());
        }

        private IEnumerator ApplyWaitForFixedUpdate()
        {
            while (m_Manager.Fibers.GetYieldCount(YieldPhase.WaitForFixedUpdate) > 0)
            {
                yield return s_CachedWaitForFixedUpdate;
                m_Manager.UpdateYield(Time.deltaTime, YieldPhase.WaitForFixedUpdate);
            }
            m_WaitForFixedUpdateRoutine = null;
        }

        private IEnumerator ApplyWaitForEndOfFrame()
        {
            while (true)
            {
                yield return s_CachedWaitForEndOfFrame;
                if (m_Manager.Fibers.GetYieldCount(YieldPhase.WaitForEndOfFrame) > 0)
                {
                    m_Manager.UpdateYield(Time.deltaTime, YieldPhase.WaitForEndOfFrame);
                }

                m_Manager.UpdateAsync();
                m_Manager.MarkFrameEnd();
            }
        }

        private void StopYieldInstructions()
        {
            if (m_WaitForFixedUpdateRoutine != null)
            {
                StopCoroutine(m_WaitForFixedUpdateRoutine);
                m_WaitForFixedUpdateRoutine = null;
            }

            if (m_WaitForEndOfFrameRoutine != null)
            {
                StopCoroutine(m_WaitForEndOfFrameRoutine);
                m_WaitForEndOfFrameRoutine = null;
            }
        }

        #endregion
    }
}