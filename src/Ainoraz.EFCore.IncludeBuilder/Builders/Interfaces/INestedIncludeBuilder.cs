namespace Ainoraz.EFCore.IncludeBuilder.Builders.Interfaces;

/// <summary>
/// <inheritdoc/>
/// <para>
///   Used for configuring nested includes.
/// </para>
/// </summary>
/// <typeparam name="TBase">Entity type of source query.</typeparam>
/// <typeparam name="TCurrent">Type of the current entity, on which inclusions are applied.</typeparam>
public interface INestedIncludeBuilder<TBase, TCurrent> :
    IIncludeBuilder<TBase, TCurrent, INestedIncludeBuilder<TBase, TCurrent>>
    where TBase : class
{
}
