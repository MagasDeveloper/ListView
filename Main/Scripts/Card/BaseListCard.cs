using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mahas.ListView
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class BaseListCard : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        
        internal bool IsNewlyCreated { get; private set; }

        internal RectTransform RectTransform => _rectTransform;
        
        private Task _recycleTask;
        
        /// <summary>
        /// Gets the size of the card as a <see cref="Vector2"/> representing the width and height.
        /// </summary>
        public Vector2 Size => _rectTransform.sizeDelta;
        
        //=========================================//
        // UNITY METHODS
        //=========================================//

        private void OnValidate()
        {
            _rectTransform ??= GetComponent<RectTransform>();
        }
        
        //=========================================//
        // PROTECTED METHODS
        //=========================================//
        
        /// <summary>
        /// Called when the card is created. Override to implement custom initialization logic.
        /// </summary>
        protected virtual void OnCreate() { }
        
        /// <summary>
        /// Called to refresh the card. Override to update card visuals or data.
        /// </summary>
        protected virtual void OnRefresh() {}
        
        /// <summary>
        /// Called when the card is released. Override to implement custom cleanup logic.
        /// </summary>
        protected virtual void OnDelete() {}

        /// <summary>
        /// Called when the card moves inside the window and is spawned.
        /// Override to implement logic for when the card enters the view.
        /// </summary>
        protected abstract void OnSpawn();
        
        /// <summary>
        /// Called when the card moves outside the window and is despawned.
        /// Override to implement logic for when the card exits the view.
        /// </summary>
        protected virtual void OnRecycle() {}

        protected virtual Task ProcessRecycle(CancellationToken token)
        {
            return Task.CompletedTask;
        }
        
        //=========================================//
        // INTERNAL METHODS
        //=========================================//

        internal abstract Type GetDataType();
        internal abstract void SetData(IListViewData viewData);
        internal void InvokeCreate() => OnCreate();
        internal void InvokeSpawn() => OnSpawn();
        internal void InvokeRecycle() => OnRecycle();
        internal void Refresh() => OnRefresh();

        internal Task InvokeProcessRecycle(CancellationToken cancellationToken) => ProcessRecycle(cancellationToken);
        internal void InvokeDelete() => OnDelete();
        internal void SetAsNew() => IsNewlyCreated = true;
        internal void UnsetAsNew() => IsNewlyCreated = false;

        internal void SetSiblingIndex(int index)
        {
            RectTransform.SetSiblingIndex(index);
        }

        internal void ApplyVirtualRect(Rect virtualRect, RectTransform content)
        {
            if (IsNewlyCreated)
            {
                NormalizeCardTransform();
            }
            
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, virtualRect.width);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   virtualRect.height);

            Vector2 parentSize = content.rect.size;
            Vector2 parentMin  = -content.pivot * parentSize;
            Vector2 anchorCenter = parentMin + Vector2.Scale((RectTransform.anchorMin + RectTransform.anchorMax) * RectTransform.pivot, parentSize);
            Vector2 parentTopLeft = new Vector2(content.rect.xMin, content.rect.yMax);
            
            Vector2 childPivotLocal = parentTopLeft + new Vector2(
                virtualRect.x + RectTransform.pivot.x * virtualRect.width,
                virtualRect.y - RectTransform.pivot.y * virtualRect.height
            );

            RectTransform.anchoredPosition = childPivotLocal - anchorCenter;
        }
        
        
        //=========================================//
        // PRIVATE METHODS
        //=========================================//
        
        private void NormalizeCardTransform()
        {
            var center = new Vector2(0.5f, 0.5f);
            RectTransform.anchorMin = center;
            RectTransform.anchorMax = center;
            RectTransform.pivot     = center;

            RectTransform.anchoredPosition3D = Vector3.zero;
            RectTransform.localScale = Vector3.one;
            RectTransform.localRotation = Quaternion.identity;
        }
        
        
    }
}