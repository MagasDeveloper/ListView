using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

namespace Mahas.ListView
{
    public class ListViewManipulatorProcessBuilder
    {
        private readonly ListViewManipulator _manipulator;
        private AnimationCurve _animationCurve;

        private readonly float _duration;
        private bool _isTimeScaled;
        private float _offset;
        private float _delay;

        private AlignmentType _alignmentType;
        private Action _onInterruptAction;
        private Action _onCompleteAction;
        private Action _onStartAction;

        internal ListViewManipulatorProcessBuilder(ListViewManipulator manipulator, float duration)
        {
            _manipulator = manipulator;
            _duration = duration;
        }
        
        /// <summary>
        /// Sets the animation curve for the process.
        /// </summary>
        /// <param name="curve">The `AnimationCurve` to be used for the process.</param>
        /// <returns>
        /// Returns the current instance of `ListViewManipulatorProcessBuilder` to allow method chaining.
        /// </returns>
        public ListViewManipulatorProcessBuilder SetAnimationCurve(AnimationCurve curve)
        {
            _animationCurve = curve;
            return this;
        }
        
        /// <summary>
        /// Sets the delay for the process.
        /// </summary>
        /// <param name="delay">The delay duration in seconds.</param>
        /// <returns>
        /// Returns the current instance of `ListViewManipulatorProcessBuilder` to allow method chaining.
        /// </returns>
        public ListViewManipulatorProcessBuilder SetDelay(float delay)
        {
            _delay = delay;
            return this;
        }
        
        /// <summary>
        /// Sets the offset for the process.
        /// </summary>
        /// <param name="offset">The offset value to be applied to the process.</param>
        /// <returns>
        /// Returns the current instance of `ListViewManipulatorProcessBuilder` to allow method chaining.
        /// </returns>
        public ListViewManipulatorProcessBuilder SetOffset(float offset)
        {
            _offset = offset;
            return this;
        }
        
        /// <summary>
        /// Sets the alignment type for the process.
        /// </summary>
        /// <param name="alignmentType">The alignment type to be applied to the process.</param>
        /// <returns>
        /// Returns the current instance of `ListViewManipulatorProcessBuilder` to allow method chaining.
        /// </returns>
        public ListViewManipulatorProcessBuilder SetAlignment(AlignmentType alignmentType)
        {
            _alignmentType = alignmentType;
            return this;
        }
        
        /// <summary>
        /// Sets whether the process should be time-scaled.
        /// </summary>
        /// <param name="isTimeScaled">A boolean indicating if the process should be affected by time scaling.</param>
        /// <returns>
        /// Returns the current instance of `ListViewManipulatorProcessBuilder` to allow method chaining.
        /// </returns>
        public ListViewManipulatorProcessBuilder SetTimeScaled(bool isTimeScaled)
        {
            _isTimeScaled = isTimeScaled;
            return this;
        }

        /// <summary>
        /// Registers an action to be executed when the process completes.
        /// </summary>
        /// <param name="action">The action to execute upon process completion.</param>
        /// <returns>
        /// Returns the current instance of `ListViewManipulatorProcessBuilder` to allow method chaining.
        /// </returns>
        public ListViewManipulatorProcessBuilder OnComplete(Action action)
        {
            _onCompleteAction = action;
            return this;
        }

        /// <summary>
        /// Registers an action to be executed when the process starts.
        /// </summary>
        /// <param name="action">The action to execute upon process start.</param>
        /// <returns>
        /// Returns the current instance of `ListViewManipulatorProcessBuilder` to allow method chaining.
        /// </returns>
        public ListViewManipulatorProcessBuilder OnStart(Action action)
        {
            _onStartAction = action;
            return this;
        }
        
        /// <summary>
        /// Registers an action to be executed when the process is interrupted.
        /// </summary>
        /// <param name="action">The action to execute upon process interruption.</param>
        /// <returns>
        /// Returns the current instance of `ListViewManipulatorProcessBuilder` to allow method chaining.
        /// </returns>
        public ListViewManipulatorProcessBuilder OnInterrupt(Action action)
        {
            _onInterruptAction = action;
            return this;
        }

        /// <summary>
        /// Initiates the process by building it and starting it using the associated manipulator.
        /// </summary>
        public void Play()
        {
            var process = BuildProcess();
            _manipulator.StartNewProcess(process);
        }

        /// <summary>
        /// Asynchronously initiates the process by building it and starting it using the associated manipulator.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task PlayAsync(CancellationToken cancellationToken = default)
        {
            var process = BuildProcess();
            await _manipulator.StartNewProcessAsync(process, cancellationToken);
        }
        
        /// <summary>
        /// Initiates the process as a coroutine by building it and starting it using the associated manipulator.
        /// </summary>
        /// <param name="holder">The `MonoBehaviour` instance that will hold the coroutine. If null, the default holder is used.</param>
        /// <returns>
        /// Returns a `Coroutine` that represents the running process.
        /// </returns>
        public Coroutine PlayCoroutine(MonoBehaviour holder = null)
        {
            var process = BuildProcess();
            return _manipulator.StartNewProcessCoroutine(process, holder);
        }

        private ListViewManipulatorProcess BuildProcess()
        {
            return new ListViewManipulatorProcess(
                _duration, 
                _isTimeScaled, 
                _delay, 
                _offset, 
                _animationCurve,
                _onStartAction, 
                _onCompleteAction, 
                _onInterruptAction, 
                _alignmentType,
                _manipulator.ContentDirectionType);
        }
        
    }
}