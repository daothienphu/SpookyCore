﻿namespace SpookyCore.Runtime.Systems
{
    public interface IPoolable
    {
        void OnGettingFromPool(bool getPreviewVersion);
        void OnReturningToPool();
    }
}