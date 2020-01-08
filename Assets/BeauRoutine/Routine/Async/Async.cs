/*
 * Copyright (C) 2016-2018. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    7 Jan 2020
 * 
 * File:    Async.cs
 * Purpose: Utility functions for performing async calls.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using BeauRoutine.Internal;

namespace BeauRoutine
{
    /// <summary>
    /// Asynchronous operations.
    /// </summary>
    static public class Async
    {
        #region Invoke

        /// <summary>
        /// Invokes the given function on the main thread
        /// at the end of the frame.
        /// </summary>
        static public void InvokeAsync(Action inAction)
        {
            Manager m = Manager.Get();
            if (m != null)
            {
                m.InitializeAsync();
                m.Scheduler.Dispatcher.EnqueueInvoke(inAction);
            }
        }

        /// <summary>
        /// Invokes the given function on the main thread
        /// at the end of the frame.
        /// </summary>
        static public void InvokeAsync<T>(Action<T> inAction, T inArgument)
        {
            InvokeAsync(() => inAction(inArgument));
        }

        /// <summary>
        /// Invokes the given function on the main thread.
        /// If not on the main thread, blocks until executed.
        /// </summary>
        static public void Invoke(Action inAction)
        {
            Manager m = Manager.Get();
            if (m != null)
            {
                m.InitializeAsync();
                m.Scheduler.Dispatcher.InvokeBlocking(inAction);
            }
        }

        /// <summary>
        /// Invokes the given function on the main thread.
        /// If not on the main thread, blocks until executed.
        /// </summary>
        static public void Invoke<T>(Action<T> inAction, T inArgument)
        {
            Invoke(() => inAction(inArgument));
        }

        #endregion // Invoke

        #region Combine

        /// <summary>
        /// Waits for the other handles to complete.
        /// </summary>
        static public AsyncHandle Combine(AsyncPriority inPriority, params AsyncHandle[] inHandles)
        {
            return Combine(inPriority, inHandles);
        }

        /// <summary>
        /// Waits for the other handles to complete.
        /// </summary>
        static public AsyncHandle Combine(AsyncHandle[] inHandles, AsyncPriority inPriority)
        {
            if (inHandles == null || inHandles.Length == 0)
            {
                return AsyncHandle.Null;
            }

            return Schedule(CombineImpl((AsyncHandle[]) inHandles.Clone()), inPriority);
        }

        /// <summary>
        /// Waits for the other handles to complete.
        /// </summary>
        static public AsyncHandle Combine(List<AsyncHandle> inHandles, AsyncPriority inPriority)
        {
            if (inHandles == null || inHandles.Count == 0)
            {
                return AsyncHandle.Null;
            }

            return Schedule(CombineImpl(inHandles.ToArray()), inPriority);
        }

        static private IEnumerator CombineImpl(AsyncHandle[] inHandles)
        {
            for (int i = inHandles.Length - 1; i >= 0; --i)
            {
                while (inHandles[i].IsRunning())
                {
                    yield return null;
                }
            }
        }

        #endregion // Combine

        #region Schedule

        /// <summary>
        /// Schedules an action to be executed asynchronously.
        /// </summary>
        static public AsyncHandle Schedule(Action inAction, AsyncPriority inPriority)
        {
            Manager m = Manager.Get();
            if (m != null)
            {
                m.InitializeAsync();
                return m.Scheduler.Schedule(inAction, inPriority);
            }
            return AsyncHandle.Null;
        }

        /// <summary>
        /// Schedules an enumerated procedure to be executed asynchronously.
        /// </summary>
        static public AsyncHandle Schedule(IEnumerator inEnumerator, AsyncPriority inPriority)
        {
            Manager m = Manager.Get();
            if (m != null)
            {
                m.InitializeAsync();
                return m.Scheduler.Schedule(inEnumerator, inPriority);
            }
            return AsyncHandle.Null;
        }

        #endregion // Schedule

        #region For

        /// <summary>
        /// Executes a set of operations on the indices between from (inclusive) and to (exclusive).
        /// </summary>
        static public AsyncHandle For(int inFrom, int inTo, Routine.IndexOperation inOperation, AsyncPriority inPriority)
        {
            if (inOperation == null || inFrom >= inTo)
                return AsyncHandle.Null;

            return Schedule(ForImpl(inFrom, inTo, inOperation), inPriority);
        }

        static private IEnumerator ForImpl(int inFrom, int inTo, Routine.IndexOperation inOperation)
        {
            for (int i = inFrom; i < inTo; ++i)
            {
                inOperation(i);
                yield return null;
            }
        }

        #endregion // For

        #region Sequence

        /// <summary>
        /// Executes a set of actions.
        /// </summary>
        static public AsyncHandle Sequence(IEnumerable<Action> inActions, AsyncPriority inPriority)
        {
            if (inActions == null)
                return AsyncHandle.Null;

            return Schedule(SequenceImpl(inActions), inPriority);
        }

        static private IEnumerator SequenceImpl(IEnumerable<Action> inActions)
        {
            foreach (var action in inActions)
            {
                action();
                yield return null;
            }
        }

        /// <summary>
        /// Executes a set of actions.
        /// </summary>
        static public AsyncHandle Sequence(AsyncPriority inPriority, params Action[] inActions)
        {
            return Sequence(inActions, inPriority);
        }

        #endregion // Sequence

        #region ForEach

        /// <summary>
        /// Executes an operation on each element in a set.
        /// </summary>
        static public AsyncHandle ForEach<T>(IEnumerable<T> inElements, Routine.ElementOperation<T> inOperation, AsyncPriority inPriority)
        {
            if (inElements == null || inOperation == null)
            {
                return AsyncHandle.Null;
            }

            return Schedule(ForEachImpl<T>(inElements, inOperation), inPriority);
        }

        /// <summary>
        /// Executes an operation on each element in a set, additionally passing in the element index.
        /// </summary>
        static public AsyncHandle ForEach<T>(IEnumerable<T> inElements, Routine.IndexedElementOperation<T> inOperation, AsyncPriority inPriority)
        {
            if (inElements == null || inOperation == null)
            {
                return AsyncHandle.Null;
            }

            return Schedule(ForEachImpl<T>(inElements, inOperation), inPriority);
        }

        static private IEnumerator ForEachImpl<T>(IEnumerable<T> inElements, Routine.ElementOperation<T> inOperation)
        {
            foreach (var element in inElements)
            {
                inOperation(element);
                yield return null;
            }
        }

        static private IEnumerator ForEachImpl<T>(IEnumerable<T> inElements, Routine.IndexedElementOperation<T> inOperation)
        {
            int idx = 0;
            foreach (var element in inElements)
            {
                inOperation(idx++, element);
                yield return null;
            }
        }

        /// <summary>
        /// Executes an operation on each element in an array.
        /// </summary>
        static public AsyncHandle ForEach<T>(T[] inElements, Routine.ElementOperation<T> inOperation, AsyncPriority inPriority)
        {
            if (inElements == null || inElements.Length == 0 || inOperation == null)
            {
                return AsyncHandle.Null;
            }

            return Schedule(ForEachImpl<T>(inElements, inOperation), inPriority);
        }

        /// <summary>
        /// Executes an operation on each element in an array, additionally passing in the element index.
        /// </summary>
        static public AsyncHandle ForEach<T>(T[] inElements, Routine.IndexedElementOperation<T> inOperation, AsyncPriority inPriority)
        {
            if (inElements == null || inElements.Length == 0 || inOperation == null)
            {
                return AsyncHandle.Null;
            }

            return Schedule(ForEachImpl<T>(inElements, inOperation), inPriority);
        }

        static private IEnumerator ForEachImpl<T>(T[] inElements, Routine.ElementOperation<T> inOperation)
        {
            for (int i = 0; i < inElements.Length; ++i)
            {
                inOperation(inElements[i]);
                yield return null;
            }
        }

        static private IEnumerator ForEachImpl<T>(T[] inElements, Routine.IndexedElementOperation<T> inOperation)
        {
            for (int i = 0; i < inElements.Length; ++i)
            {
                inOperation(i, inElements[i]);
                yield return null;
            }
        }

        #endregion // ForEach
    }
}