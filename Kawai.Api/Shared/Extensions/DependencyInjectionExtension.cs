using System.Reflection;

namespace Kawai.Api.Shared.Extensions;

public static class DependencyInjectionExtension
{
    /// <summary>
    /// Mendaftarkan semua interface dan class implementasi yang ada di assembly terpisah
    /// dengan asumsi:
    /// - Interface di namespace "Kawai.Domain.Interfaces" dan diawali "I"
    /// - Implementasi di namespace "Kawai.Data.Repositories" dengan nama tanpa "I"
    /// </summary>
    /// <param name="services">IServiceCollection dari DI container</param>
    public static void AddRepositoriesAuto(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var domainAssembly = typeof(Kawai.Domain.Interfaces.IWarehouseRepository).Assembly;
        var repoAssembly = typeof(Kawai.Data.Repositories.WarehouseRepository).Assembly;

        var interfaceTypes = domainAssembly.GetTypes()
            .Where(t => t.IsInterface && t.Namespace != null && t.Namespace.StartsWith("Kawai.Domain.Interfaces"));

        var implementationTypes = repoAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Namespace != null && t.Namespace.StartsWith("Kawai.Data.Repositories"));

        foreach (var interfaceType in interfaceTypes)
        {
            var expectedClassName = interfaceType.Name.StartsWith("I")
                ? interfaceType.Name.Substring(1)
                : interfaceType.Name;

            var implementationType = implementationTypes
                .FirstOrDefault(c =>
                    c.Name.Equals(expectedClassName, StringComparison.OrdinalIgnoreCase) &&
                    interfaceType.IsAssignableFrom(c));

            if (implementationType != null)
                services.AddScoped(interfaceType, implementationType);
        }
    }

}
