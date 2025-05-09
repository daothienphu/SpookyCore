using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.EntitySystem
{
    public interface IAreaCast
    {
        List<EntityBase> GetTargets(Vector2 center, Vector2 size, float orientation);

        List<EntityBase> GetTargets(Vector2 center, Vector2 size, float orientation, LayerMask layerMask);
    }
}