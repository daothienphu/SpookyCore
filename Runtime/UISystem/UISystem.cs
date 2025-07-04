using System;
using System.Collections.Generic;
using SpookyCore.Runtime.Systems;
using UnityEngine;

namespace SpookyCore.Runtime.UI
{
    public class UISystem : MonoSingleton<UISystem>
    {
        #region Struct

        private enum UIVisibilityState
        {
            Visible,
            Invisible,
        }
        
        private class UICache
        {
            private readonly List<IViewPresenter> _cachedHudUIs = new();
            private readonly List<IViewPresenter> _cachedPopupUIs = new();
            private readonly List<IViewPresenter> _cachedOverlayUIs = new();
            private readonly List<IViewPresenter> _cachedWidgets = new();
            private readonly Dictionary<Type, List<IViewPresenter>> _cachedWorldSpaceUIs = new();

            public void Cache(IViewPresenter viewPresenter)
            {
                if (viewPresenter.GetLayer() == UILayer.WorldSpace)
                {
                    if (!_cachedWorldSpaceUIs.ContainsKey(viewPresenter.GetType()))
                    {
                        _cachedWorldSpaceUIs.Add(viewPresenter.GetType(), new());
                    }
                    _cachedWorldSpaceUIs[viewPresenter.GetType()].Add(viewPresenter);
                    return;
                }
                
                var cacheList = viewPresenter.GetLayer() switch
                {
                    UILayer.Hud => _cachedHudUIs,
                    UILayer.Popup => _cachedPopupUIs,
                    UILayer.Overlay => _cachedOverlayUIs,
                    UILayer.Widget => _cachedWidgets,
                    _ => _cachedHudUIs
                } ?? new List<IViewPresenter>();
                
                cacheList.Add(viewPresenter);
            }

            public bool TryGetUI<T>(out IViewPresenter viewPresenter, UIVisibilityState visibilityState) where T : IViewPresenter
            {
                var type = typeof(T);
                if (_cachedWorldSpaceUIs.TryGetValue(type, out var uiList))
                {
                    foreach (var ui in uiList)
                    {
                        if ((visibilityState == UIVisibilityState.Visible && ui.IsVisible()) ||
                            (visibilityState == UIVisibilityState.Invisible && !ui.IsVisible()))
                        {
                            viewPresenter = ui;
                            return true;
                        }
                    }

                    viewPresenter = null;
                    return false;
                }

                return TryGetFromList<T>(_cachedWidgets, out viewPresenter, visibilityState) || 
                       TryGetFromList<T>(_cachedHudUIs, out viewPresenter, visibilityState) || 
                       TryGetFromList<T>(_cachedPopupUIs, out viewPresenter, visibilityState) || 
                       TryGetFromList<T>(_cachedOverlayUIs, out viewPresenter, visibilityState);
            }

            public bool TryGetAllUI<T>(out List<IViewPresenter> viewPresenters) where T : IViewPresenter
            {
                var type = typeof(T);
                if (_cachedWorldSpaceUIs.TryGetValue(type, out var uiList))
                {
                    viewPresenters = uiList;
                    return true;
                }

                viewPresenters = null;
                return false;
            }
            
            private bool TryGetFromList<T>(List<IViewPresenter> viewPresenters,
                out IViewPresenter viewPresenter, UIVisibilityState visibilityState) where T : IViewPresenter
            {
                var type = typeof(T);
                foreach (var ui in viewPresenters)
                {
                    if (ui.GetType() == type)
                    {
                        if ((visibilityState == UIVisibilityState.Visible && ui.IsVisible()) ||
                            (visibilityState == UIVisibilityState.Invisible && !ui.IsVisible()))
                        {
                            viewPresenter = ui;
                            return true;
                        }
                        viewPresenter = null;
                        return false;
                    }
                }
                
                viewPresenter = null;
                return false;
            }

            public List<IViewPresenter> GetHUDs()
            {
                var res = new List<IViewPresenter>();
                foreach (var ui in _cachedHudUIs)
                {
                    if (ui.GetLayer() == UILayer.Hud)
                    {
                        res.Add(ui);
                    }
                }

                return res;
            }
        }
        
        #endregion

        #region Fields

        [SerializeField] private UIPrefabsConfig _uiLayersConfig;
        [SerializeField] private Transform _hudLayerParent;
        [SerializeField] private Transform _popupLayerParent;
        [SerializeField] private Transform _overlayLayerParent;
        [SerializeField] private Transform _worldSpaceLayerParent;
        public bool IsReady { get; private set; }

        private UICache _cachedUI;
        private Stack<IViewPresenter> _popupStack = new();
        private bool _enableLog;

        #endregion
        
        #region Life Cycle

        protected override void OnStart()
        {
            _cachedUI = new UICache();
            
            CacheAllUIPrefabs(_uiLayersConfig.HudLayer, _hudLayerParent);
            CacheAllUIPrefabs(_uiLayersConfig.PopupLayer, _popupLayerParent);
            CacheAllUIPrefabs(_uiLayersConfig.OverlayLayer, _overlayLayerParent);
            CacheAllUIPrefabs(_uiLayersConfig.WorldSpaceLayer, _worldSpaceLayerParent);

            foreach (var hud in _cachedUI.GetHUDs())
            {
                hud.Init();
                hud.ShowView();
            }
            
            IsReady = true;
        }

        #endregion

        #region Public Methods

        public bool IsUIVisible<TViewPresenter>() where TViewPresenter : IViewPresenter
        {
            return _cachedUI.TryGetUI<TViewPresenter>(out var viewPresenter, UIVisibilityState.Visible);
        }
        
        public IViewPresenter ShowUI<TViewPresenter>(BaseModel model) where TViewPresenter : IViewPresenter
        {
            if (_cachedUI.TryGetUI<TViewPresenter>(out var viewPresenter, UIVisibilityState.Invisible))
            {
                viewPresenter.Init(model);
                viewPresenter.ShowView();

                if (viewPresenter.GetLayer() == UILayer.Popup)
                {
                    _popupStack.Push(viewPresenter);
                }
            }
            else
            {
                Debug.Log($"Cannot show UI, UI not found: {typeof(TViewPresenter)}.");
            }
            return viewPresenter;
        }

        public bool TryGetHudElement<T>(out IViewPresenter hudElement) where T : IViewPresenter
        {
            return _cachedUI.TryGetUI<T>(out hudElement, UIVisibilityState.Visible);
        }

        public void HideCurrentPopup()
        {
            if (_popupStack.Count > 0)
            {
                var popup = _popupStack.Pop();
                popup.HideView();
            }
        }
        
        public void HideUI<TViewPresenter>(bool showWarning = true) where TViewPresenter : IViewPresenter
        {
            if (_cachedUI.TryGetUI<TViewPresenter>(out var viewPresenter, UIVisibilityState.Visible))
            {
                viewPresenter.HideView();
            }
            else if (showWarning)
            {
                Debug.Log($"Cannot hide UI, UI not found: {typeof(TViewPresenter)}.");
            }
        }

        public void HideAllUIs<TViewPresenter>() where TViewPresenter : IViewPresenter
        {
            if (_cachedUI.TryGetAllUI<TViewPresenter>(out var viewPresenters))
            {
                foreach (var viewPresenter in viewPresenters)
                {
                    viewPresenter.HideView();
                }
            }
        }

        #endregion

        #region Private Methods

        private void CacheAllUIPrefabs(List<GameObject> prefabs, Transform parent)
        {
            foreach (var prefab in prefabs)
            {
                var instance = Instantiate(prefab);
                instance.transform.SetParent(parent, false);
                
                var rectTransform = instance.GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                var viewPresenter = instance.GetComponent<IViewPresenter>();
                if (viewPresenter == null)
                {
                    Debug.LogError($"Please assign a view presenter script to the prefab: \"{prefab.name}\".");
                    continue;
                }
                
                if (viewPresenter.GetLayer() == UILayer.Popup)
                {
                    viewPresenter.HideView();    
                }
                
                _cachedUI.Cache(viewPresenter);
            }
        }

        #endregion
    }
}