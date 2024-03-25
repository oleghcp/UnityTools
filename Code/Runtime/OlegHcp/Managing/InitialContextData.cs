using System;

namespace OlegHcp.Managing
{
    internal class InitialContextData
    {
        private IInitialContext<IService> _context;
        private IService _service;

        public IService Service => _service ?? (_service = _context.GetOrCreateInstance());

        public InitialContextData(IInitialContext<IService> context)
        {
            _context = context;
        }

        public void ClearInstance(bool dispose)
        {
            if (dispose && _service is IDisposable disposable)
                disposable.Dispose();

            _service = null;
        }
    }
}
