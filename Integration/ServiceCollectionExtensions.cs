using CabaVS.Shared.Abstractions.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CabaVS.Shared.EFCore.UnitOfWork.Integration
{
    public static class ServiceCollectionIntegration
    {
        public static void AddEFCoreUnitOfWork(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null) throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}