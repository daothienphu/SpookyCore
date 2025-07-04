namespace SpookyCore.Runtime.Systems
{
    public abstract class PureSingleton<T> where T : IBootstrapSystem, new()
    {
        public static T Instance { get; private set; }

        protected PureSingleton()
        {
        }

        public static T Create()
        {
            if (Instance != null)
            {
                return Instance;
            }
            
            Instance = new T();

            return Instance;
        }
    }
}