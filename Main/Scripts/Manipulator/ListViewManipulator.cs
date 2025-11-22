using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Mahas.ListView
{
    public class ListViewManipulator
    {
        private ListViewManipulatorProcess _currentProcess;
        private readonly MonoBehaviour _coroutineHolder;
        private readonly ListViewBrain _viewBrain;
        private readonly RectTransform _viewport;
        private readonly RectTransform _content;
        private readonly ScrollRect _scrollRect;
        private Rect _targetRect;
        
        internal ContentDirectionType ContentDirectionType { get; }
        
        internal ListViewManipulator(
            RectTransform content, 
            RectTransform viewport, 
            ScrollRect scrollRect,
            ListViewBrain viewBrain,
            MonoBehaviour coroutineHolder,
            ContentDirectionType contentDirectionType)
        {
            _content = content;
            _viewport = viewport;
            _viewBrain = viewBrain;
            _scrollRect = scrollRect;
            _coroutineHolder = coroutineHolder;
            ContentDirectionType = contentDirectionType;
        }

        public ListViewManipulatorProcessBuilder MoveTo<TData>(TData data, int index, float duration) where TData : IListData
        {
            return new ListViewManipulatorProcessBuilder(this, duration);
        } 
        
        public ListViewManipulatorProcessBuilder ScrollTo<TData>(TData data, float duration) where TData : IListData
        {
            int dataIndex = _viewBrain.DataProvider.GetDataIndex(data);
            return ScrollTo(dataIndex, duration);
        }
        
        public ListViewManipulatorProcessBuilder ScrollTo(int index, float duration)
        {
            var virtualCards = _viewBrain.GetVirtualCards();
            if (index >= virtualCards.Count || index < 0)
            {
                Debug.LogError($"ListViewManipulator - ScrollTo: Index {index} is out of range. Collection size is {virtualCards.Count}.");
                return new ListViewManipulatorProcessBuilder(this, duration);
            }
            
            _targetRect = virtualCards[index].Rect;
            return new ListViewManipulatorProcessBuilder(this, duration);
        }

        internal void StartNewProcess(ListViewManipulatorProcess process)
        {
            if (_targetRect == default)
            {
                Debug.LogError("ListViewManipulator - Target is not set");
                return;
            }

            PrepareNewProcess(process);
            _currentProcess.Process();
        }
        
        internal async Task StartNewProcessAsync(ListViewManipulatorProcess process, CancellationToken cancellationToken)
        {
            PrepareNewProcess(process);
            await _currentProcess.ProcessAsync(cancellationToken);

        }

        internal Coroutine StartNewProcessCoroutine(ListViewManipulatorProcess process, MonoBehaviour parentHolder)
        {
            PrepareNewProcess(process);
            MonoBehaviour holder = parentHolder != null ? parentHolder : _coroutineHolder;
            return _currentProcess.ProcessCoroutine(holder);
        }

        private void PrepareNewProcess(ListViewManipulatorProcess process)
        {
            _currentProcess?.Interrupt();
            _currentProcess = process;
            _scrollRect.velocity = Vector2.zero;
            _currentProcess.Setup(_content, _viewport, _targetRect);
        }

    }
}