using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine.Pool;
using UnityEngine;
using System.Linq;
using System;

namespace Mahas.ListView
{
    public class ListViewBrain
    {
        private const float UpdateStep = 1f;
        
        internal readonly ListDataProvider DataProvider = new();
        
        private readonly ObjectPool<VirtualListCard> _virtualCardsPool = new (
            createFunc: () => new VirtualListCard(),
            actionOnGet: null,
            actionOnRelease: null,
            actionOnDestroy: null,
            collectionCheck: false,
            defaultCapacity: 20);
        
        private readonly ObjectPool<ListViewElement> _elementsPool = new (
            createFunc: () => new ListViewElement(),
            actionOnGet: null,
            actionOnRelease: null,
            actionOnDestroy: null,
            collectionCheck: false,
            defaultCapacity: 20);
        
        private readonly Dictionary<int, ListViewElement> _activeElementsMap = new(60);
        private readonly List<VirtualListCard> _virtualCards = new(10);
        
        private readonly ListViewContent ViewContent;
        private readonly BaseListCard[] _cardPrefabs;
        private readonly ListListeners _listeners;
        private readonly ObjectPoolMap _poolMap;
        private readonly ListViewport Viewport;
        
        private List<VirtualListCard> _visibleNow = new(10);
        private List<VirtualListCard> _visiblePrev = new(10);

        private ICardInstantiator _instantiator;

        private Vector2 _lastContentPos;
        private Vector2 _defaultCardSize;
        
        private int _lastVisibleStartIndex;
        private bool _isStaticListElements;
        private bool _keepSiblingOrder;
        private bool _hasLastPos;

        internal ListViewBrain(ListViewContent viewContent, ListViewport viewport, PrefabListVariant[] cardVariants, ListListeners listeners, bool keepSiblingOrder)
        {
            ViewContent = viewContent;
            Viewport = viewport;
            _listeners = listeners;
            _instantiator = new CardInstantiator();
            _keepSiblingOrder = keepSiblingOrder;
            _cardPrefabs = cardVariants.Select(x => x.Prefab).ToArray();
            _poolMap = new ObjectPoolMap(cardVariants, _instantiator, viewContent.RectTransform, 10);
        }
        
        //=========================================//
        // INTERNAL METHODS
        //=========================================//
        
        internal void SetupData(IEnumerable<IListViewData> data)
        {
            DataProvider.SetupData(data);
        }
        
        internal void AddData(IListViewData viewData)
        {
            DataProvider.AddData(viewData);
        }
        
        internal void RemoveData(IListViewData viewData)
        {
            DataProvider.RemoveData(viewData);
        }
        
        internal void ClearData()
        {
            DataProvider.Clear();
        }
        
        internal void Rebuild()
        {
            _hasLastPos = false;
            _lastVisibleStartIndex = 0;

            foreach (var kvp in _activeElementsMap)
            {
                var index = kvp.Key;
                var virtualCard = _virtualCards[index];
                HardRecycle(virtualCard);
            }
            _activeElementsMap.Clear();

            _visibleNow.Clear();
            _visiblePrev.Clear();

            _isStaticListElements = IsStaticListElements();
            if (_isStaticListElements)
            {
                _defaultCardSize = GetDefaultCardSize();
            }

            BakeCardRects();
            ResizeContent();
        }

        internal void TryUpdate(bool forceUpdate = false, bool canRetry = true)
        {
            Vector2 contentPos = ViewContent.RectTransform.anchoredPosition;
            if (!forceUpdate && _hasLastPos)
            {
                float deltaSqr = (contentPos - _lastContentPos).sqrMagnitude;
                if (deltaSqr < UpdateStep)
                    return;
            }

            _lastContentPos = contentPos;
            _hasLastPos = true;
            
            (_visiblePrev, _visibleNow) = (_visibleNow, _visiblePrev);
            _visibleNow.Clear();
            
            int startIndex = 0;
            if (!forceUpdate && _lastVisibleStartIndex > 0)
            {
                startIndex = Mathf.Max(0, _lastVisibleStartIndex - 1);
            }

            bool hasVisibleAnyCard = false;
            for (int i = startIndex; i < _virtualCards.Count; i++)
            {
                var virtualCard = _virtualCards[i];

                if (Viewport.ContainsInViewport(virtualCard, contentPos))
                {
                    _visibleNow.Add(virtualCard);
                    if (!_visiblePrev.Contains(virtualCard))
                    {
                        OnCardBecameVisible(virtualCard);
                    }

                    hasVisibleAnyCard = true;
                }
                else
                {
                    if (hasVisibleAnyCard)
                        break;
                }
            }

            if (_visibleNow.Count > 0)
            {
                _lastVisibleStartIndex = _visibleNow[0].Index;
            }

            foreach (var vc in _visiblePrev)
            {
                if (!_visibleNow.Contains(vc))
                {
                    _ = OnCardBecameInvisible(vc, false);
                }
            }
            
            if (_virtualCards.Count > 0 && _visibleNow.Count == 0 && canRetry)
            {
                _lastVisibleStartIndex = 0;
                TryUpdate(true, false);
            }

            _listeners.InvokeContentMove(ViewContent.RectTransform.anchoredPosition);
        }
        
        internal IReadOnlyList<VirtualListCard> GetVirtualCards()
        {
            return _virtualCards;
        }

        internal void SetInstantiator(ICardInstantiator instantiator)
        {
            _instantiator = instantiator;
        }

        internal void SetKeepSiblingIndex(bool isActive)
        {
            _keepSiblingOrder = isActive;
        }

        //=========================================//
        // PRIVATE METHODS
        //=========================================//
        
        private Rect[] GetCardsRect()
        {
            var data = DataProvider.Items;
            int count = data.Count;
            var rects = new Rect[count];

            var paddings = ViewContent.Paddings;
            float spacing = ViewContent.Spacing;

            bool horizontal = ViewContent.Direction == ContentDirectionType.Horizontal;
            float startOffset = horizontal ? paddings.left : paddings.top;
            
            Vector2 axis = horizontal ? Vector2.right : Vector2.down;
            float pos = startOffset;

            if (_isStaticListElements)
            {
                float step = (horizontal ? _defaultCardSize.x : _defaultCardSize.y) + spacing;

                for (int i = 0; i < count; i++)
                {
                    SetRect(i, new Vector2(startOffset + i * step, -(startOffset + i * step)), _defaultCardSize);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    Vector2 size;
                    if (data[i] is IHaveCustomListSize customListSize)
                    {
                        size = customListSize.GetCustomSize();
                    }
                    else
                    {
                        size = _poolMap.GetPrefabSize(data[i].GetType());
                    }
                    Vector2 offset = axis * pos;
                    SetRect(i, offset, size);
                    pos += (horizontal ? size.x : size.y) + spacing;
                }
            }
            
            return rects;


            void SetRect(int index, Vector2 offset, Vector2 size)
            {
                switch (ViewContent.Direction)
                {
                    case ContentDirectionType.Horizontal:
                        rects[index] = new Rect(offset.x, -ViewContent.Paddings.top, size.x, size.y);
                        break;

                    case ContentDirectionType.Vertical:
                        rects[index] = new Rect(ViewContent.Paddings.left, offset.y, size.x, size.y);
                        break;
                }
            }
        }

        private void ResizeContent()
        {
            if (DataProvider.Items == null)
            {
                return;
            }

            Vector2 contentSize = default;
            int count = DataProvider.Items.Count;
            var data = DataProvider.Items;
            
            if (!_isStaticListElements)
            {
                Vector2 rawSize = new Vector2();
                foreach (var item in data)
                {
                    Vector2 size;
                    if (item is IHaveCustomListSize customListSize)
                    {
                        size = customListSize.GetCustomSize();
                    }
                    else
                    {
                        size = _poolMap.GetPrefabSize(item.GetType());
                    }

                    rawSize += size;
                }
                BuildVectorSize(rawSize);
            }
            else
            {
                BuildVectorSize(_defaultCardSize * count);
            }
            
            var paddings = ViewContent.Paddings;
            contentSize += new Vector2(paddings.left + paddings.right, paddings.top + paddings.bottom);
            ViewContent.RectTransform.sizeDelta = contentSize;

            void BuildVectorSize(Vector2 size)
            {
                float totalSpacing = ViewContent.Spacing * (count - 1);
                switch (ViewContent.Direction)
                {
                    case ContentDirectionType.Horizontal:
                        contentSize.x = size.x + totalSpacing;
                        contentSize.y = GetDefaultCardSize().y;
                        break;
                    case ContentDirectionType.Vertical:
                        contentSize.x = GetDefaultCardSize().x;
                        contentSize.y = size.y + totalSpacing;
                        break;
                }
            }
        }

        private bool IsStaticListElements()
        {
            Type firstType = null;

            foreach (var item in DataProvider.Items)
            {
                if (item == null)
                    continue;

                var itemType = item.GetType();
                if (firstType == null)
                {
                    firstType = itemType;
                }
                else if (itemType != firstType)
                {
                    return false;
                }
                
                if (item is IHaveCustomListSize)
                    return false;
            }

            return true;
        }

        private Vector2 GetDefaultCardSize()
        {
            return _cardPrefabs[0].Size;
        }

        private void BakeCardRects()
        {
            foreach (var vc in _virtualCards)
            {
                _virtualCardsPool.Release(vc);
            }
            _virtualCards.Clear();
            
            Rect[] cardsRect = GetCardsRect();
            for (int i = 0; i < cardsRect.Length; i++)
            {
                var virtualCard = _virtualCardsPool.Get();
                virtualCard.Setup(cardsRect[i], i);
                _virtualCards.Add(virtualCard);
            }
        }

        private void OnCardBecameVisible(VirtualListCard virtualCard)
        {
            int index = virtualCard.Index;
            IListViewData viewData = DataProvider.Items[index];

            if (_activeElementsMap.TryGetValue(index, out var element))
            {
                element.CancelRecycle();
                var instance = element.Card;
                instance.ApplyVirtualRect(virtualCard.Rect, ViewContent.RectTransform);
                instance.SetData(viewData);

                instance.InvokeSpawn();
                virtualCard.SetState(VirtualListCardState.Enabled);

                TrySortSiblingIndex();
                return;
            }

            var card = _poolMap.Get(viewData.GetType());
            card.ApplyVirtualRect(virtualCard.Rect, ViewContent.RectTransform);
    
            var newElement = _elementsPool.Get();
            newElement.Initialize(card, viewData, index);

            if (card.IsNewlyCreated)
            {
                _listeners.InvokeCreate(newElement);
                card.InvokeCreate();
                card.UnsetAsNew();
            }

            card.SetData(viewData);
            card.InvokeSpawn();

            Viewport.AddVisibleElement(newElement);
            _activeElementsMap[index] = newElement;

            virtualCard.SetState(VirtualListCardState.Enabled);
            TrySortSiblingIndex();
        }

        private async Task OnCardBecameInvisible(VirtualListCard virtualCard, bool immediate)
        {
            int index = virtualCard.Index;
            var element = _activeElementsMap[index];

            bool fullyRecycled = true;

            if (!immediate)
            {
                element.CancelRecycle();
                element.RecycleCTS = new CancellationTokenSource();

                try
                {
                    virtualCard.SetState(VirtualListCardState.WaitingForRecycle);
                    await SafeRecycle(element, element.RecycleCTS.Token);
                }
                catch (OperationCanceledException)
                {
                    fullyRecycled = false;
                }
            }

            if (!fullyRecycled)
                return;

            Viewport.RemoveVisibleElement(element);
            _activeElementsMap.Remove(index);
            _poolMap.Release(element.Card);
            virtualCard.SetState(VirtualListCardState.Disabled);
            TrySortSiblingIndex();
        }

        private void TrySortSiblingIndex()
        {
            if (!_keepSiblingOrder)
                return;

            int index = 0;
            foreach (var element in Viewport.VisibleElements)
            {
                element.Card.SetSiblingIndex(index++);
            }
            
        }
        
        private async Task SafeRecycle(ListViewElement element, CancellationToken token)
        {
            try
            {
                await element.Card.InvokeProcessRecycle(token);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                Debug.LogError($"ListView - ProcessRecycle exception in card {element.Index}: {ex}");
            }
        }
        
        private void HardRecycle(VirtualListCard virtualCard)
        {
            int index = virtualCard.Index;
            var element = _activeElementsMap[index];

            element.CancelRecycle();
            Viewport.RemoveVisibleElement(element);
            _poolMap.Release(element.Card);
            virtualCard.SetState(VirtualListCardState.Disabled);
        }
        
    }
}