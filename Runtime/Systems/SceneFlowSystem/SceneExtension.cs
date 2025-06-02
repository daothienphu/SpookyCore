using UnityEngine.SceneManagement;

namespace SpookyCore.Runtime.Systems
{
    public static class SceneExtensions
    {
        public static SceneID ToSceneID(this Scene scene)
        {
            foreach (SceneID id in System.Enum.GetValues(typeof(SceneID)))
            {
                if (scene.name.Contains(id.ToString()))
                    return id;
            }

            return SceneID.Bootstrap;
        }
    }
}