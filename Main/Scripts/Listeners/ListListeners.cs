using UnityEngine.Events;
using UnityEngine;

namespace Mahas.ListView
{
    public class ListListeners
    {
        /// <summary>
        /// Event triggered when a new ListElement is created (instantiated) in the view.
        /// </summary>
        public readonly UnityEvent<ListViewElement> OnCreate = new();
        
        /// <summary>
        /// Event triggered when a new IListData item is spawned in the view.
        /// </summary>
        public readonly UnityEvent<ListViewElement> OnSpawn = new();
        
        /// <summary>
        /// Event triggered when an IListData item is recycled from the view.
        /// </summary>
        public readonly UnityEvent<ListViewElement> OnRecycle = new();
        
        /// <summary>
        /// Event triggered during the content movement process, providing the current position as a Vector2.
        /// </summary>
        public readonly UnityEvent<Vector2> OnContentMove = new();
        
        //=========================================//
        // INTERNAL METHODS
        //=========================================//
        
        internal void InvokeSpawn(ListViewElement element) => OnSpawn?.Invoke(element);
        internal void InvokeRecycle(ListViewElement element) => OnRecycle?.Invoke(element);
        internal void InvokeCreate(ListViewElement element) => OnCreate?.Invoke(element);
        internal void InvokeContentMove(Vector2 position) => OnContentMove?.Invoke(position);

        //=========================================//
        // PRIVATE METHODS
        //=========================================//

    }
}