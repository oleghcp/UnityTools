using System;
using System.Collections.Generic;
using System.Linq;
using OlegHcp.Tools;

namespace OlegHcp.Managing
{
    public interface IStateTracker
    {
        event Action StateChanged_Event;
        IEnumerable<Type> GetServiceTypesForCurrentState();
    }

    public class StateServiceLocator : BaseServiceLocator
    {
        private IStateTracker _stateTracker;
        private HashSet<Type> _requiredTypes = new HashSet<Type>();
        private List<Type> _unnecessaryTypes = new List<Type>();

        public StateServiceLocator(IInitialContext context, IStateTracker stateTracker, bool throwIfNotFound = true) 
            : base(context, throwIfNotFound)
        {
            _stateTracker = stateTracker;
            stateTracker.StateChanged_Event += OnStateChanged;
        }

        private protected override TService GetInternal<TService>()
        {
            Type serviceType = typeof(TService);

            if (ServiceCache.TryGetValue(serviceType, out IService service))
                return (TService)service;

            if (ThrowIfNotFound)
                throw new ServiceNotFoundException($"Service {typeof(TService).Name} for current state not found.");

            return null;
        }

        private void OnStateChanged()
        {
            _requiredTypes.Clear();
            _unnecessaryTypes.Clear();

            _requiredTypes.UnionWith(_stateTracker.GetServiceTypesForCurrentState());
            _unnecessaryTypes.AddRange(ServiceCache.Keys.Where(item => !_requiredTypes.Contains(item)));

            foreach (Type serviceType in _unnecessaryTypes)
            {
                ServiceCache.Remove(serviceType, out IService service);
                if (service is IDisposable disposable)
                    disposable.Dispose();
            }

            _requiredTypes.RemoveWhere(item => ServiceCache.ContainsKey(item));

            foreach (Type serviceType in _requiredTypes)
            {
                if (Context.TryGetOrCreateInstance(serviceType, out IService service))
                    ServiceCache.Add(serviceType, service);
                else if (ThrowIfNotFound)
                    throw ThrowErrors.ServiceNotRegistered(serviceType);
            }
        }
    }
}
