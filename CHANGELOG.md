### Version 0.10.1
**13 September 2019**

Added amortization helper methods. Several bug fixes.

#### Features

* Added amortization methods for splitting work across multiple frames
* Added frame milliseconds budget concept

#### Fixes

* Fixed case where ``ITweenData.OnTweenEnd`` would not be called when canceling a tween when no cancel mode is set
** This fixes a case where cancelling an euler rotation tween would sometimes result in future euler rotation tweens on the same object starting with the wrong rotation
* Fixed WaitForEndOfFrame not dispatching properly
* Fixed RoutineGroupRef property drawer not respecting property label

#### Improvements

* Switched Routine.Inline result to class to avoid inevitable boxing
* Added ``TweenUtil.EvaluateMirrored`` for evaluating a mirrored version of an easing curve

#### Breaking Changes

* Renamed chunked version of ``Routine.ForEachParallel`` to ``Routine.ForEachParallelChunked``

### Version 0.10.0
**30 April 2019**

Added Unity package manager support. Introduced Extensions and Splines packages. New tween types, bug fixes, and shortcut methods.

#### Features

* Added new tweens for
** Camera
** Material
** Transform
** SkinnedMeshRenderer
** Scrollbar
** Slider
** ScrollRect
** Light
* Added two new packages: BeauRoutine-Extensions and BeauRoutine-Splines
* Added ``GetLock`` to retrieve a lock on a BeauRoutine. A BeauRoutine will not execute if it is locked.
* Added ``RealtimeUpdate`` phase. This executes at a consistent rate (``Time.unscaledTime``)
* Added shortcut function ``Play`` to tweens
* Added support for the Unity package manager

#### Fixes

* Fixed NullReferenceException when calling SetUpdateRoutine while application is closing
* Fixed memory leak in ``Routine.Delay``
* Fixed memory leak in ``UnityEvent.WaitForInvoke``
* Fixed improper timing when yielding a ThinkUpdate during ThinkUpdate
* Fixed ``TransformUtil.SetRotation`` only applying to local rotation

#### Improvements

* Tween instances are now pooled
* Expanded allowed collection types for ``Routine.ForEach`` and ``Routine.ForEachParallel``
* Added shortcut for common Future create-and-execute pattern
* Resolved obsolete UnityEngine.WWW warnings
* Added optional "up" parameter for ``Transform.LookAt`` tweens

### Version 0.9.1
**10 April 2018**

Added support for ThinkUpdate and CustomUpdate phases. Fixed critical bug when yielding a BeauRoutine to an update phase.

#### Features

* Added ``ThinkUpdate`` and ``CustomUpdate`` phases. These will execute at a configurable rate
* Added ``Paused`` property to ``RoutineIdentity`` to pause all BeauRoutines on a GameObject
* Added ``RoutineBootstrap`` component. This will configure BeauRoutine settings on Awake

#### Fixes

* Fixed critical issue where BeauRoutines that yielded to a different phase could be skipped during iteration
* Fixed case where profiling information could be reported inaccurately after a Manual update
* Fixed potential NullReferenceException if shutting down BeauRoutine during an update phase

#### Improvements

* Script execution order for BeauRoutine is now fixed to 20000, to ensure more consistent timing between projects

#### Breaking Changes

* Added ``RoutineUpdates.SetUpdateRoutineGenerator`` to avoid ambiguous argument types. Previously this was an overload of SetUpdateRoutine, but it has been made into a distinct function.

#### Documentation

* Documented ThinkUpdate and CustomUpdate phases
* Documented allocations associated with BeauRoutine's debug profiling
* Corrected spelling errors
* Corrected an unfinished section

### Version 0.9.0
**15 March 2018**

First numbered version. All Unity coroutine features are now implemented and supported, and the documentation has been updated to be more thorough.

#### Features

* Added ability to change a BeauRoutine's phase. Previous versions would always execute during ``LateUpdate``, but this can now be switched to ``FixedUpdate``, ``Update``, ``LateUpdate``, or ``Manual``
* Added ability to attach an exception handler to a BeauRoutine with ``OnException``
* Yielding a ``WaitForEndOfFrame`` or ``WaitForFixedUpdate`` now corresponds to the same behavior in Unity coroutines
* Added extension methods to set update functions for a MonoBehaviour
* Added ability to manually update BeauRoutines
* Added ability to report progress on a Future
* Added ability to manually add a time delay to a BeauRoutine with ``DelayBy``

#### Fixes

* Fixed ``TweenUtil.LerpDecay(float, float, float)`` using the wrong parameter for part of the calculation
* Custom IEnumerators now throw a ``NotSupportedException`` instead of a ``NotImplementedException`` to match language specification

#### Improvements

* Expanded ``IFuture`` interface to incorporate more shared functionality
* Added ``UnityWebRequest`` support to all ``Future.Download`` utilities
* General performance improvements

#### Breaking Changes

* Renamed ``RectTransformState.State()`` to ``RectTransformState.Create()``

#### Documentation

* Documented update phase feature
* Added Technical Notes section
* Expanded table of contents
* Reorganized Advanced Features section
* Added more functions to Reference section
* Added two more examples to Tips and Tricks section