using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Mahas.ListView
{
    [RequireComponent(typeof(ScrollRect))]
    public class ListView : MonoBehaviour
    {
        [Header("Variants")]
        [SerializeField] private PrefabListVariants _prefabListVariants;
        
        [Header("Viewport")]
        [SerializeField] private ViewportSettings _viewportSettings;
        
        [Header("Content")]
        [SerializeField] private ContentSettings _contentSettings;

        [Header("Other")] 
        [SerializeField] private OtherSettings _otherSettings;

        internal ListViewBrain ViewBrain { get; set; }
        private ScrollRect _scrollRect;
        
        
        public readonly ListListeners Listeners = new();
        public ListViewManipulator Manipulator;
        public ListViewContent Content { get; private set; }
        public ListViewport Viewport { get; private set; }
        
        public bool KeepSiblingOrder => _otherSettings.KeepSiblingOrder;
        public bool IsEnableGizmo => _otherSettings.IsEnableGizmo;
        
        //=========================================//
        // UNITY METHODS
        //=========================================//

        private void OnValidate()
        {
            _scrollRect ??= GetComponent<ScrollRect>();
        }

        public virtual void OnEnable()
        {
            _scrollRect.onValueChanged.AddListener(OnContentMove);
        }

        public virtual void OnDisable()
        {
            _scrollRect.onValueChanged.RemoveListener(OnContentMove);
        }

        private void Awake()
        {
            Initialize();
        }
        
        //=========================================//
        // PROTECTED METHODS
        //=========================================//
        
        private void Initialize()
        {
            if (_scrollRect.viewport == null)
            {
                Debug.LogError("ReactList: ScrollRect viewport is not assigned!", this);
                return;
            }
            
            Viewport = new ListViewport(_scrollRect.viewport, Listeners, _viewportSettings.VisibilityPaddings);
            Content = new ListViewContent(_contentSettings.Direction, _scrollRect.content, _contentSettings.Spacing, _contentSettings.Paddings);
            ViewBrain = new ListViewBrain(Content, Viewport, _prefabListVariants.Variants, Listeners, KeepSiblingOrder);
            Manipulator = new ListViewManipulator(_scrollRect.content, _scrollRect.viewport, _scrollRect, ViewBrain, this, _contentSettings.Direction);
        }
        
        //=========================================//
        // PUBLIC METHODS
        //=========================================//
        
        /// <summary>
        /// Sets the custom instantiator used by the RectList content to create card instances.
        /// </summary>
        /// <param name="instantiator">The instantiator implementing <see cref="IObjectInstantiator"/>.</param>
        public void SetInstantiator(IObjectInstantiator instantiator)
        {
            ViewBrain.SetInstantiator(instantiator);
        }

        /// <summary>
        /// Enables or disables keeping sibling index for list elements.
        /// </summary>
        /// <param name="isActive">If true, elements will keep their sibling index; otherwise sibling ordering may change.</param>
        public void SetKeepSiblingIndex(bool isActive)
        {
            ViewBrain.SetKeepSiblingIndex(isActive);
        }

        /// <summary>
        /// Sets up the data collection that will be displayed in the list.
        /// </summary>
        /// <param name="data">The collection of data items to display.</param>
        /// <param name="forceUpdate">If true, forces an immediate update of the list view after setting the data.</param>
        public void SetupData(IEnumerable<IListData> data, bool forceUpdate = true)
        {
            ViewBrain.SetupData(data);
            if (forceUpdate)
            {
                ViewBrain.TryUpdate(true);
            }
        }
        
        /// <summary>
        /// Removes a specific data item from the list view.
        /// </summary>
        /// <param name="data">The data item to remove.</param>
        /// <param name="forceUpdate">
        /// If true, forces an immediate update of the list view after removing the data.
        /// </param>
        public void RemoveData(IListData data, bool forceUpdate = true)
        {
            ViewBrain.RemoveData(data);
            if (forceUpdate)
            {
                ViewBrain.TryUpdate(true);
            }
        }
        
        /// <summary>
        /// Clears all data from the list view.
        /// </summary>
        /// <param name="forceUpdate">
        /// If true, forces an immediate update of the list view after clearing the data.
        /// </param>
        public void ClearData(bool forceUpdate = true)
        {
            ViewBrain.ClearData();
            if (forceUpdate)
            {
                ViewBrain.TryUpdate(true);
            }
        }

        /// <summary>
        /// Forces an immediate update of the list view, recalculating visible elements.
        /// </summary>
        public void ForceUpdate()
        {
            ViewBrain.TryUpdate(true);
        }

        /// <summary>
        /// Refreshes all currently visible elements in the list view.
        /// Calls the <c>Refresh</c> method on each visible card to update its display.
        /// </summary>
        public void Refresh()
        {
            foreach (var element in Viewport.VisibleElements)
            {
                element.Card.Refresh();
            }
        }
        
        //=========================================//
        // PRIVATE METHODS
        //=========================================//

        private void OnContentMove(Vector2 value)
        {
            ViewBrain.TryUpdate();
        }

        private void OnDrawGizmos()
        {
            if (!IsEnableGizmo)
                return;
            
            DrawContentSize();
            DrawViewport();
            DrawCards();

            void DrawContentSize()
            {
                if (!Application.isPlaying)
                    return;
                
                Vector3[] corners = new Vector3[4];
                Content.RectTransform.GetWorldCorners(corners);

                Vector3 center = (corners[0] + corners[2]) * 0.5f;
                Vector3 size = new Vector3(
                    Vector3.Distance(corners[0], corners[3]),
                    Vector3.Distance(corners[0], corners[1]),
                    0f);

                Gizmos.color = new Color(255, 204, 0);
                Gizmos.DrawWireCube(center, size);
            }

            void DrawCards()
            {
                if (!Application.isPlaying)
                    return;
                
                IReadOnlyCollection<VirtualListCard> virtualCards = ViewBrain.GetVirtualCards();
                if (virtualCards == null)
                    return;

                var content = Content.RectTransform;
                Vector2 topLeftLocal = new Vector2(-content.rect.width * content.pivot.x, content.rect.height * (1f - content.pivot.y));

                int index = 0;
                foreach (var card in virtualCards)
                {
                    
                    Vector2 cardCenterLocal = topLeftLocal + new Vector2(
                        card.Rect.center.x,
                        card.Rect.center.y - card.Rect.height
                    );

                    Vector3 worldCenter = content.TransformPoint(cardCenterLocal);
                    Vector3 size = new Vector3(card.Rect.width, card.Rect.height, 0f);

                    Gizmos.matrix = Matrix4x4.TRS(worldCenter, content.rotation, content.lossyScale);
                    Gizmos.color = card.IsVisible 
                        ? new Color(0f, 1f, 0f) 
                        : new Color(1f, 0.1f, 0f);
                    
                    Gizmos.DrawWireCube(Vector3.zero, size);

                    Gizmos.color = card.IsVisible
                        ? new Color(0f, 1f, 0f, 0.4f)
                        : new Color(1f, 0.1f, 0f, 0.4f);
                    
                    Gizmos.DrawCube(Vector3.zero, size);

                    if (ViewBrain.DataProvider.Items.ElementAt(index) is IHaveMessageForGizmo debugMessage)
                    {
                        GUIStyle style = new GUIStyle
                        {
                            alignment = TextAnchor.MiddleCenter,
                            fontSize = 14,
                            normal = { textColor = Color.white }
                        };

                        Handles.Label(worldCenter, debugMessage.GetMessage(), style);
                    }

                    index++;
                }

            }

            void DrawViewport()
            {
                var viewport = _scrollRect.viewport;
                var rect = viewport.rect;
                var pads = _viewportSettings.VisibilityPaddings;

                float innerW = Mathf.Max(0f, rect.width  - pads.horizontal);
                float innerH = Mathf.Max(0f, rect.height - pads.vertical);

                Vector2 innerMinLocal = new Vector2(rect.xMin + pads.left, rect.yMin + pads.bottom);
                Vector2 innerCenterLocal = innerMinLocal + new Vector2(innerW * 0.5f, innerH * 0.5f);

                Vector3 innerCenterWorld = viewport.TransformPoint(innerCenterLocal);
                Vector3 innerSizeLocal = new Vector3(innerW, innerH, 0f);

                Gizmos.matrix = Matrix4x4.TRS(innerCenterWorld, viewport.rotation, viewport.lossyScale);

                Gizmos.color = new Color(0f, 1f, 0.75f);
                Gizmos.DrawWireCube(Vector3.zero, innerSizeLocal);
                
            }
        }

        
    }
}

