using System.Collections.Generic;
using UnityEngine;

namespace Mahas.ListView
{
    public class ListViewport
    {
        private readonly Dictionary<IListData, BaseListCard> _visibleElementsMap = new();
        private readonly SortedList<int, ViewListElement> _visibleElements = new();
        private readonly ListListeners _listeners;
        private readonly RectOffset _paddings;

        public IList<ViewListElement> VisibleElements => _visibleElements.Values;
        
        /// <summary>
        /// The RectTransform associated with this ListView, representing its position and size in the UI.
        /// </summary>
        public RectTransform RectTransform { get; private set; }
        
        /// <summary>
        /// Size of the view in local space.
        /// </summary>
        public Vector2 Size => RectTransform.rect.size;

        /// <summary>
        /// Paddings applied to the visibility area of the viewport.
        /// </summary>
        public RectOffset VisibilityPaddings => _paddings;

        internal ListViewport(RectTransform rectTransform, ListListeners listeners, RectOffset paddings)
        {
            RectTransform = rectTransform;
            _listeners = listeners;
            _paddings = paddings;
        }
        
        //=========================================//
        // PUBLIC METHODS
        //=========================================//
        
        /// <summary>
        /// Determines if the given <typeparamref name="TData"/> item is visible in the view.
        /// </summary>
        /// <param name="data">The data item to check for visibility.</param>
        /// <returns>True if the item is visible; otherwise, false.</returns>
        public bool IsVisible<TData>(TData data) where TData : IListData
        {
            return _visibleElementsMap.ContainsKey(data);
        }
        
        /// <summary>
        /// Attempts to retrieve the card instance of type <typeparamref name="TCard"/> associated with the given data item.
        /// </summary>
        /// <typeparam name="TData">Type of the data item, must implement <see cref="IListData"/>.</typeparam>
        /// <typeparam name="TCard">Type of the card, must inherit from <see cref="BaseListCard"/>.</typeparam>
        /// <param name="data">The data item to look up.</param>
        /// <param name="element">The card instance if found and of the correct type; otherwise, null.</param>
        /// <returns>True if the card instance is found and of type <typeparamref name="TCard"/>; otherwise, false.</returns>
        public bool TryGetDataInstance<TData, TCard>(TData data, out TCard element) where TData : IListData where TCard : BaseListCard
        {
            if (_visibleElementsMap.TryGetValue(data, out var card))
            {
                element = card as TCard;
                return element != null;
            }
            
            element = null;
            return false;
        }
        
        //=========================================//
        // PROTECTED METHODS
        //=========================================//
        
        //=========================================//
        // INTERNAL METHODS
        //=========================================//
        
        internal void AddVisibleElement(ViewListElement element)
        {
            _visibleElements[element.Index] = element;
            _visibleElementsMap[element.Data] = element.Card;
            _listeners.InvokeSpawn(element);
        }
        
        internal void RemoveVisibleElement(ViewListElement element)
        {
            _visibleElements.Remove(element.Index);
            _visibleElementsMap.Remove(element.Data);
            _listeners.InvokeRecycle(element);
        }
        
        internal bool ContainsInViewport(VirtualListCard card, Vector2 contentPosition)
        {
            float viewportWidth = RectTransform.rect.width;
            float viewportHeight = RectTransform.rect.height;

            Vector2 offset = -contentPosition;

            float vxMin = offset.x;
            float vxMax = offset.x + viewportWidth;
            float vyMin = offset.y - viewportHeight;
            float vyMax = offset.y;

            vxMin += _paddings.left;
            vxMax -= _paddings.right;
            vyMin += _paddings.bottom;
            vyMax -= _paddings.top;

            float cxMin = card.Rect.x;
            float cxMax = card.Rect.x + card.Rect.width;
            float cyMin = card.Rect.y - card.Rect.height;
            float cyMax = card.Rect.y;

            return !(vxMax < cxMin || vxMin > cxMax || vyMax < cyMin || vyMin > cyMax);
        }
        
        //=========================================//
        // PRIVATE METHODS
        //=========================================//
        
    }
}