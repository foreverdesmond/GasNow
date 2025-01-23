public interface IBusinessFactory
{
    /// <summary>
    /// Creates an instance of the specified type <typeparamref name="T"/> using the provided dependencies.
    /// </summary>
    /// <typeparam name="T">The type of the object to create.</typeparam>
    /// <param name="dependencies">The dependencies to pass to the constructor of the type <typeparamref name="T"/>.</param>
    /// <returns>An instance of type <typeparamref name="T"/>.</returns>
    T Create<T>(params object[] dependencies) where T : class;
}

