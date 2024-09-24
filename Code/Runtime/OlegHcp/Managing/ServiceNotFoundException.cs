﻿using System;

namespace OlegHcp.Managing
{
    public class ServiceNotFoundException : InvalidOperationException
    {
        public ServiceNotFoundException(string serviceName)
            : base($"Service {serviceName} is not registered in initial context.")
        {

        }
    }
}
