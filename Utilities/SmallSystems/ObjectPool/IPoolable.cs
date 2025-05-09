namespace SpookyCore.Utilities
{
    public interface IPoolable
    {
        void OnGettingFromPool(bool getPreviewVersion);
        void OnReturningToPool();
    }
}