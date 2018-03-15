using System;
using System.Threading.Tasks;

namespace SystemService
{
    public class ServiceRunnerConfig<T> where T : ISystemService
    {
        public Func<Task<T>> ServiceFactory { get; set; }
        public Func<T, Task> OnStart { get; set; }
        public Func<T, Task> OnStop { get; set; }
        public Func<Exception, Task> OnError { get; set; }

        public static ServiceRunnerConfig<T> CreateConfig(Func<Task<T>> serviceFactory, Func<T, Task> onStart = null, Func<T, Task> onStop = null, Func<Exception, Task> onError = null)
        {
            return new ServiceRunnerConfig<T>()
            {
                ServiceFactory = serviceFactory,
                OnStart = onStart,
                OnStop = onStop,
                OnError = onError
            };
        }
    }
}
