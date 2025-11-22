using UnityEngine.Events;
using UnityEngine;

namespace Mahas.ListView
{
    public class ListListeners
    {
        /// <summary>
        /// Event triggered when a new ListElement is created (instantiated) in the view.
        /// </summary>
        public readonly UnityEvent<ViewListElement> OnCreate = new();
        
        /// <summary>
        /// Event triggered when a new IListData item is spawned in the view.
        /// </summary>
        public readonly UnityEvent<ViewListElement> OnSpawn = new();
        
        /// <summary>
        /// Event triggered when an IListData item is recycled from the view.
        /// </summary>
        public readonly UnityEvent<ViewListElement> OnRecycle = new();
        
        /// <summary>
        /// Event triggered during the content movement process, providing the current position as a Vector2.
        /// </summary>
        public readonly UnityEvent<Vector2> OnContentMove = new();
        
        //=========================================//
        // INTERNAL METHODS
        //=========================================//
        
        internal void InvokeSpawn(ViewListElement element) => OnSpawn?.Invoke(element);
        internal void InvokeRecycle(ViewListElement element) => OnRecycle?.Invoke(element);
        internal void InvokeCreate(ViewListElement element) => OnCreate?.Invoke(element);
        internal void InvokeContentMove(Vector2 position) => OnContentMove?.Invoke(position);

        //=========================================//
        // PRIVATE METHODS
        //=========================================//

    }
}