using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

namespace Mahas.ListView.Samples
{
    [Serializable]
    public struct ButtonVisual
    {
        public Color _enableColor;
        public Color _lockColor;
    }
    
    [RequireComponent(typeof(Button))]
    public class UiButton : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private Button _button;
        
        [Header("Image")]
        [SerializeField] private Image _image;
        [SerializeField] private ButtonVisual _imageVisual;
        
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private ButtonVisual _textVisual;

        private Action _onSuccessClickAction;
        private Action _onLockClickAction;
        
        private bool _isLock;
        
        private void OnValidate()
        {
            _button ??= GetComponent<Button>();
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void Start()
        {
            UpdateVisual();
            _button.onClick.AddListener(OnClick);
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void SetLockState(bool isLock)
        {
            _isLock = isLock;
            UpdateVisual();
        }
        
        public void SetSuccessClickAction(Action onClick)
        {
            _onSuccessClickAction = onClick; 
        }
        
        public void SetLockClickAction(Action onClick)
        {
            _onLockClickAction = onClick;
        }
        
        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
        }

        private void OnClick()
        {
            if (_isLock)
            {
                _onLockClickAction?.Invoke();
                return;
            }
            _onSuccessClickAction?.Invoke();
        }
        
        private void UpdateVisual()
        {
            if (_isLock)
            {
                _image.color = _imageVisual._lockColor;
                _text.color = _textVisual._lockColor;
            }
            else
            {
                _image.color = _imageVisual._enableColor;
                _text.color = _textVisual._enableColor;
            }
        }
        
    }
}