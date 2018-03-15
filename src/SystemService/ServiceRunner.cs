using System;
using System.Threading;
using System.Threading.Tasks;

namespace SystemService
{
    public class ServiceRunner<T> : IDisposable where T : ISystemService
    {
        private T _systemService;
        private ServiceRunnerConfig<T> _runnerConfig;
        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        public ServiceRunner(ServiceRunnerConfig<T> runnerConfig)
        {
            _runnerConfig = runnerConfig;
            System.Runtime.Loader.AssemblyLoadContext.Default.Unloading += Default_Unloading;
            Console.CancelKeyPress += Default_Unloading;
        }

        public async Task Run()
        {
            try
            {
                _systemService = await _runnerConfig.ServiceFactory();
                if (_runnerConfig.OnStart != null)
                {
                    await _runnerConfig.OnStart(_systemService);
                }
                await _systemService.Start(_cancellationToken.Token);
                while (true)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), _cancellationToken.Token);
                }
            }
            catch (TaskCanceledException) {}
            catch (Exception ex)
            {
                if (_runnerConfig.OnError != null)
                {
                    await _runnerConfig.OnError(ex);
                }
            }
            finally
            {
                await Task.FromResult(0);
            }
        }

        public async void Dispose()
        {
            await _systemService.Stop();
            if (_runnerConfig.OnStop != null)
            {
                await _runnerConfig.OnStop(_systemService);
            }
        }

        private void Default_Unloading(System.Runtime.Loader.AssemblyLoadContext obj)
        {
            _cancellationToken.Cancel();
            System.Runtime.Loader.AssemblyLoadContext.Default.Unloading -= Default_Unloading;
        }

        private void Default_Unloading(object sender, ConsoleCancelEventArgs eventArgs)
        {
            _cancellationToken.Cancel();
            eventArgs.Cancel = true;
        }
    }
}
