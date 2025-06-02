namespace SpookyCore.Runtime.UI
{
    public interface IViewPresenter
    {
        UILayer GetLayer();
        void Init(BaseModel model = null);
        bool IsVisible();
        void ShowView();
        void HideView();
    }
}