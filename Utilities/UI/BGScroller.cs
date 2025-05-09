using UnityEngine;
using UnityEngine.UI;
using System;

namespace SpookyCore.Utilities.UI
{
    public class BGScroller : MonoBehaviour
    {
        [SerializeField] private RawImage _img;
        [SerializeField] private float _offsetX = 0.1f, _offsetY = 0.1f;
        [SerializeField] private float _loadInOutSpeed = 1f;
        [SerializeField] private float _sizeW = 15, _sizeH = 15;
        public static event Action<BGUIStates> BeforeBGUILoad;
        public static event Action<BGUIStates> AfterBGUILoad;
        private BGUIStates _state;

        void Start()
        {
            _img.uvRect = new Rect(Vector2.zero, Vector2.zero);
            ChangeState(BGUIStates.LoadIn);
        }

        void Update()
        {
            if (_state == BGUIStates.LoadIn)
            {
                Load();
                if (DoneLoadingIn())
                {
                    ChangeState(BGUIStates.Scrolling);
                }
            }
            else if (_state == BGUIStates.Scrolling)
            {
                ScrollBG();
            }
            else if (_state == BGUIStates.LoadOut)
            {
                Load(loadIn: false);
                if (DoneLoadingOut())
                {

                    ChangeState(BGUIStates.Idle);
                }
            }
        }

        void ChangeState(BGUIStates state)
        {
            _state = state;

            switch (state)
            {
                case BGUIStates.LoadIn:
                    BeforeBGUILoad?.Invoke(BGUIStates.LoadIn);
                    break;
                case BGUIStates.LoadOut:
                    BeforeBGUILoad?.Invoke(BGUIStates.LoadIn);
                    break;
                case BGUIStates.Scrolling:
                    AfterBGUILoad?.Invoke(BGUIStates.LoadIn);
                    break;
                case BGUIStates.Idle:
                    AfterBGUILoad?.Invoke(BGUIStates.LoadOut);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        void Load(bool loadIn = true)
        {
            var size = _img.uvRect.size;
            size.x = Mathf.Clamp(size.x + (loadIn ? 1 : -1) * _sizeW * Time.deltaTime * _loadInOutSpeed, 0, _sizeW);
            size.y = Mathf.Clamp(size.y + (loadIn ? 1 : -1) * _sizeH * Time.deltaTime * _loadInOutSpeed, 0, _sizeH);
            _img.uvRect = new Rect(_img.uvRect.position, size);
        }

        bool DoneLoadingIn()
        {
            var size = _img.uvRect.size;
            return size.x >= _sizeW && size.y >= _sizeH;
        }

        bool DoneLoadingOut()
        {
            var size = _img.uvRect.size;
            return size.x <= 0 && size.y <= 0;
        }

        void ScrollBG()
        {
            _img.uvRect = new Rect(_img.uvRect.position + new Vector2(_offsetX, _offsetY) * Time.deltaTime,
                _img.uvRect.size);
        }

        public void LoadOutBG()
        {
            BeforeBGUILoad?.Invoke(BGUIStates.LoadOut);
            ChangeState(BGUIStates.LoadOut);
        }
    }

    public enum BGUIStates
    {
        LoadIn,
        LoadOut,
        Scrolling,
        Idle
    }
}