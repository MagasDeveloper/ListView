using UnityEngine;

namespace Mahas.ListView
{
    public class VirtualListCard
    {
        public Rect Rect { get; private set; }
        public int Index { get; private set; }
        public Vector2 Position => new (Rect.x, Rect.y);
        public VirtualListCardState State { get; private set; } 

        //=========================================//
        // PUBLIC METHODS
        //=========================================//
        
        internal void Setup(Rect rect, int index)
        {
            Rect = rect;
            Index = index;
            SetState(VirtualListCardState.Disabled);
        }
        
        internal void SetState(VirtualListCardState state)
        {
            State = state;
        }
        
        //=========================================//
        // PRIVATE METHODS
        //=========================================//
        
    }
}