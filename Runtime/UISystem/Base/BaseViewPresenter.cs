using UnityEngine;

namespace SpookyCore.UISystem
{
    public abstract class BaseViewPresenter<TModel> : MonoBehaviour, IViewPresenter where TModel: BaseModel
    {
        #region Fields

        [field: SerializeField] public UILayer Layer;
        protected TModel _model;
        protected bool _isVisible;
        [field: SerializeField] protected CanvasGroup _mainCanvasGroup;
        //private bool _initialized;

        #endregion

        #region Life Cycle
        
        // private void OnEnable()
        // {
        //     if (_initialized)
        //     {
        //         OnViewPresenterReady();
        //     }
        // }
        //
        // private void OnDisable()
        // {
        //     OnViewPresenterPreDispose();
        // }

        private void Update()
        {
            OnUpdate();
        }

        #endregion

        #region Public Methods

        public UILayer GetLayer()
        {
            return Layer;
        }
        
        public virtual void Init(BaseModel model)
        {
            _model = (TModel)model;
            //_initialized = true;
            OnViewPresenterReady();
        }
        
        public bool IsVisible()
        {
            return _isVisible;
        }
        
        public virtual void ShowView()
        {
            _isVisible = true;
            _mainCanvasGroup.alpha = 1;
            _mainCanvasGroup.interactable = true;
            _mainCanvasGroup.blocksRaycasts = true;
        }

        public virtual void HideView()
        {
            _isVisible = false;
            _mainCanvasGroup.alpha = 0;
            _mainCanvasGroup.interactable = false;
            _mainCanvasGroup.blocksRaycasts = false;
            OnViewPresenterPreDispose();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Ready is called every OnEnable calls after the ViewPresenter model is initialized.
        /// Init -> OnPresenterReady -> ShowView
        /// </summary>
        public abstract void OnViewPresenterReady();

        /// <summary>
        /// PreDispose is called every OnDisable calls.
        /// HideView -> OnPresenterPreDispose
        /// </summary>
        protected abstract void OnViewPresenterPreDispose();

        protected abstract void OnUpdate();

        #endregion
    }
}