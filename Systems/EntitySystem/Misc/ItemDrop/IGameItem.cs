﻿namespace SpookyCore.EntitySystem
{
    public interface IGameItem
    {
        void AddGameItemData(GameItemSO itemSoSo);
        void CollectItem(EntityBase collector);
    }
}