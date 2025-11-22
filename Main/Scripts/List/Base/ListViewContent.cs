using UnityEngine;

namespace Mahas.ListView
{
    public class ListViewContent
    {
        public float Spacing { get; private set; }
        public readonly RectTransform RectTransform;
        public readonly ContentDirectionType Direction;
        public RectOffset Paddings { get; private set; }
        

        internal ListViewContent(ContentDirectionType direction, RectTransform content, float spacing, RectOffset paddings)
        {
            RectTransform = content;
            Direction = direction;
            Paddings = paddings;
            Spacing = spacing;

            Vector2 targetPivot = default;
            switch (direction)
            {
                case ContentDirectionType.Horizontal:
                    targetPivot = new Vector2(0f, 0.5f);
                    break;
                case ContentDirectionType.Vertical:
                    targetPivot = new Vector2(0.5f, 1f);
                    break;
            }

            RectTransform.pivot = targetPivot;
            RectTransform.anchorMin = RectTransform.anchorMax = targetPivot;
        }
        
        public void SetPaddings(int leftPadding, int rightPadding, int topPadding, int bottomPadding)
        {
            Paddings = new RectOffset(leftPadding, rightPadding, topPadding, bottomPadding);
        }
        
        public void SetHorizontalPaddings(int leftPadding, int rightPadding)
        {
            Paddings = new RectOffset(leftPadding, rightPadding, Paddings.top, Paddings.bottom);
        }
        
        public void SetVerticalPaddings(int topPadding, int bottomPadding)
        {
            Paddings = new RectOffset(Paddings.left, Paddings.right, topPadding, bottomPadding);
        }
        
        public void SetLeftPadding(int leftPadding)
        {
            Paddings = new RectOffset(leftPadding, Paddings.right, Paddings.top, Paddings.bottom);
        }
        
        public void SetRightPadding(int rightPadding)
        {
            Paddings = new RectOffset(Paddings.left, rightPadding, Paddings.top, Paddings.bottom);
        }
        
        public void SetTopPadding(int topPadding)
        {
            Paddings = new RectOffset(Paddings.left, Paddings.right, topPadding, Paddings.bottom);
        }
        
        public void SetBottomPadding(int bottomPadding)
        {
            Paddings = new RectOffset(Paddings.left, Paddings.right, Paddings.top, bottomPadding);
        }
        
        public void SetSpacing(float spacing)
        {
            Spacing = spacing;
        }
        
    }
}