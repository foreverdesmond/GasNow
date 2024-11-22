using GasNow.Module;
using AutoMapper;
using StackExchange.Redis;

public class BusinessFactory : IBusinessFactory
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
    public BusinessFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Creates an instance of the specified type <typeparamref name="T"/> using the provided dependencies.
    /// </summary>
    /// <typeparam name="T">The type of the object to create.</typeparam>
    /// <param name="dependencies">The dependencies to pass to the constructor of the type <typeparamref name="T"/>.</param>
    /// <returns>An instance of type <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no matching constructor is found for the specified type with the given dependencies.</exception>
    public T Create<T>(params object[] dependencies) where T : class
    {
        var constructors = typeof(T).GetConstructors();

        var matchedConstructor = constructors
            .Where(c => c.GetParameters().Length == dependencies.Length)
            .FirstOrDefault(c =>
            {
                var parameters = c.GetParameters();
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (!parameters[i].ParameterType.IsAssignableFrom(dependencies[i].GetType()))
                    {
                        return false;
                    }
                }
                return true;
            });

        if (matchedConstructor == null)
        {
            throw new InvalidOperationException($"No matching constructor found for type {typeof(T).Name} with the given dependencies.");
        }

        return (T)matchedConstructor.Invoke(dependencies);
    }
}