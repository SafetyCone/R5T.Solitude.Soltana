using System;

using Microsoft.Extensions.DependencyInjection;

using R5T.Dacia;

using InMemorySolutionFileOperator = R5T.Soltana.IVisualStudioSolutionFileOperator;


namespace R5T.Solitude.Soltana
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="VisualStudioSolutionFileOperator"/> implementation of <see cref="IVisualStudioSolutionFileOperator"/> as an <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddSoltanaVisualStudioSolutionFileOperator(this IServiceCollection services,
            ServiceAction<InMemorySolutionFileOperator> addInMemorySolutionFileOperator,
            ServiceAction<IVisualStudioSolutionFileOperator> addSolutionFileSerializer)
        {
            services
                .AddSingleton<IVisualStudioSolutionFileOperator, VisualStudioSolutionFileOperator>()
                .RunServiceAction(addInMemorySolutionFileOperator)
                .RunServiceAction(addSolutionFileSerializer)
                ;

            return services;
        }

        /// <summary>
        /// Adds the <see cref="VisualStudioSolutionFileOperator"/> implementation of <see cref="IVisualStudioSolutionFileOperator"/> as an <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static ServiceAction<IVisualStudioSolutionFileOperator> AddSoltanaVisualStudioSolutionFileOperatorAction(this IServiceCollection services,
            ServiceAction<InMemorySolutionFileOperator> addInMemorySolutionFileOperator,
            ServiceAction<IVisualStudioSolutionFileOperator> addSolutionFileSerializer)
        {
            var serviceAction = new ServiceAction<IVisualStudioSolutionFileOperator>(() => services.AddSoltanaVisualStudioSolutionFileOperator(
                addInMemorySolutionFileOperator,
                addSolutionFileSerializer));
            return serviceAction;
        }
    }
}
