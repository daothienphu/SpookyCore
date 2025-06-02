using System.Threading.Tasks;

namespace SpookyCore.Runtime.Systems
{
    public abstract class PureSingleton<T> : IBootstrapSystem where T : PureSingleton<T>, new()
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
            Instance.OnInitialize();

            return Instance;
        }

        public virtual void OnInitialize()
        {
            
        }
        
        public Task OnBootstrapAsync(BootstrapContext context)
        {
            return Task.CompletedTask;
        }
    }
}