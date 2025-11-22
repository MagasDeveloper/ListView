using UnityEngine;

namespace Mahas.ListView
{
    public class VirtualListCard
    {
        public Rect Rect { get; private set; }
        public int Index { get; private set; }
        public Vector2 Position => new (Rect.x, Rect.y);
        public bool IsVisible { get; private set; }

        //=========================================//
        // PUBLIC METHODS
        //=========================================//
        
        public void Setup(Rect rect, int index)
        {
            Rect = rect;
            Index = index;
            IsVisible = false;
        }

        public void SetVisible(bool isVisible)
        {
            IsVisible = isVisible;
        }
        
        //=========================================//
        // PRIVATE METHODS
        //=========================================//
        
    }
}