using System.Threading.Tasks;

namespace SpookyCore.Runtime.Systems
{
    public interface IBootstrapSystem
    {
        Task OnBootstrapAsync(BootstrapContext context);
    }
}