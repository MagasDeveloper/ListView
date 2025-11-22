using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using UnityEngine;
using System;

namespace Mahas.ListView
{
    public class ListViewManipulatorProcess
    {
        private readonly AnimationCurve _animationCurve;

        private readonly bool _isTimeScaled;
        private readonly float _duration;
        private readonly float _delay;
        private readonly float _offset;

        private readonly ContentDirectionType _contentDirectionType;
        private readonly AlignmentType _alignmentType;
        
        private readonly Action _onStartAction;
        private readonly Action _onCompleteAction;
        private readonly Action _onInterruptAction;

        private MonoBehaviour _coroutineHolder;
        private Coroutine _runningCoroutine;
        private RectTransform _viewport;
        private RectTransform _content;

        private Rect _targetRect;
        private bool _isInterrupted;

        private Vector2 _startContentPosition;
        private Vector2 _endContentPosition;
        
        public float CompleteProgress { get; private set; }
        public bool IsCompleted => CompleteProgress >= 1f;

        public ListViewManipulatorProcess(float duration, 
            bool isTimeScaled, 
            float delay, 
            float offset, 
            AnimationCurve animationCurve, 
            Action onStartAction, 
            Action onCompleteAction,
            Action onInterruptAction, 
            AlignmentType alignmentType,
            ContentDirectionType contentDirectionType)
        {
            _duration = duration;
            _isTimeScaled = isTimeScaled;
            _delay = delay;
            _offset = offset;
            _animationCurve = animationCurve;
            _onStartAction = onStartAction;
            _onCompleteAction = onCompleteAction;
            _onInterruptAction = onInterruptAction;
            _alignmentType = alignmentType;
            _contentDirectionType = contentDirectionType;
        }

        public void Setup(RectTransform content, RectTransform viewport, Rect target)
        {
            _content = content;
            _viewport = viewport;
            _targetRect = target;

            _startContentPosition = content.anchoredPosition;
            CalculateEndPosition();
            
        }

        public void Process()
        {
            _ = ProcessAsync(default);
        }

        public void Interrupt()
        {
            _isInterrupted = true;
            if (_runningCoroutine != null)
            {
                _coroutineHolder.StopCoroutine(_runningCoroutine);
                _runningCoroutine = null;
            }
        }
        
        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            if (_delay > 0f)
            {
                await Task.Delay(TimeSpan.FromSeconds(_delay), cancellationToken);
            }
            
            _onStartAction?.Invoke();
            
            float elapsedTime = 0f;
            while (elapsedTime < _duration)
            {
                if (_isInterrupted || cancellationToken.IsCancellationRequested)
                {
                    _onInterruptAction?.Invoke();
                    _isInterrupted = false;
                    return;
                }
                
                elapsedTime += _isTimeScaled ? Time.deltaTime : Time.unscaledDeltaTime;
                CompleteProgress = Mathf.Clamp01(elapsedTime / _duration);
                
                float curveValue = _animationCurve?.Evaluate(CompleteProgress) ?? CompleteProgress;
                UpdatePosition(curveValue);
                
                await Task.Yield();
            }
            
            CompleteProgress = 1f;
            _onCompleteAction?.Invoke();
        }
        
        public Coroutine ProcessCoroutine(MonoBehaviour holder)
        {
            _coroutineHolder = holder;
            _runningCoroutine = holder.StartCoroutine(ProcessCoroutineInternal());
            return _runningCoroutine;
        }
        
        private IEnumerator ProcessCoroutineInternal()
        {
            if (_delay > 0f)
            {
                if (_isTimeScaled)
                    yield return new WaitForSeconds(_delay);
                else
                    yield return new WaitForSecondsRealtime(_delay);
            }

            _onStartAction?.Invoke();
            float time = 0f;

            while (time < _duration)
            {
                float t = Mathf.Clamp01(time / _duration);
                float curveValue = _animationCurve?.Evaluate(t) ?? t;

                UpdatePosition(curveValue);

                if (_isTimeScaled)
                    time += Time.deltaTime;
                else
                    time += Time.unscaledDeltaTime;

                yield return null;
            }

            UpdatePosition(1f);
            _onCompleteAction?.Invoke();
        }

        private void UpdatePosition(float curveValue)
        {
            curveValue = Mathf.Clamp01(curveValue);
            if (_contentDirectionType == ContentDirectionType.Horizontal)
            {
                float x = Mathf.Lerp(_startContentPosition.x, _endContentPosition.x, curveValue);
                _content.anchoredPosition = new Vector2(x, _startContentPosition.y);
            }
            else
            {
                float y = Mathf.Lerp(_startContentPosition.y, _endContentPosition.y, curveValue);
                _content.anchoredPosition = new Vector2(_startContentPosition.x, y);
            }
        }

        private void CalculateEndPosition()
        {
            if (_contentDirectionType == ContentDirectionType.Horizontal)
            {
                float targetX = _targetRect.xMin;
                if (_alignmentType == AlignmentType.Center)
                {
                    targetX -= (_viewport.rect.width - _targetRect.width) / 2f;
                }
                else if (_alignmentType == AlignmentType.End)
                {
                    targetX -= _viewport.rect.width - _targetRect.width;
                }
                targetX += _offset;
                targetX = Mathf.Clamp(targetX, 0f, _content.rect.width - _viewport.rect.width);
                _endContentPosition = new Vector2(-targetX, _endContentPosition.y);
            }
            else
            {
                float targetY = -_targetRect.yMax;
                if (_alignmentType == AlignmentType.Center)
                {
                    targetY += (_viewport.rect.height - _targetRect.height) / 2f;
                }
                else if (_alignmentType == AlignmentType.End)
                {
                    targetY += _viewport.rect.height - _targetRect.height;
                }
                targetY -= _offset;
                targetY = Mathf.Clamp(targetY, -(_content.rect.height - _viewport.rect.height), 0f);
                _endContentPosition = new Vector2(_endContentPosition.x, -targetY);
            }
        }
        
    }
}