﻿using Nimbus.DependencyResolution;

namespace Nimbus.Interceptors.Inbound
{
    internal interface IInboundInterceptorFactory
    {
        IInboundInterceptor[] CreateInterceptors(IDependencyResolverScope scope, object handler, object message);

        IInboundInterceptor[] CreateGlobalInterceptors(IDependencyResolverScope scope);

        IInboundInterceptor[] CreateHandlerInterceptors(IDependencyResolverScope scope, object handler, object message);
    }
}