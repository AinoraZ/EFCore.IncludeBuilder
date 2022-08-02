using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Ainoraz.EFCore.IncludeBuilder.Builders.Interfaces;

/// <summary>
/// Provides alternative syntax for including navigation properties on source query.
/// </summary>
/// <typeparam name="TBase">Entity type of source query.</typeparam>
/// <typeparam name="TCurrent">Type of the current entity, on which inclusions are applied.</typeparam>
/// <typeparam name="TReturn">Type of IncludeBuilder that will be returned for further chaining.</typeparam>
public interface IIncludeBuilder<TBase, TCurrent, TReturn>
    where TBase : class
    where TReturn : IIncludeBuilder<TBase, TCurrent, TReturn>
{
    /// <summary>
    ///   Specifies navigational properties to include in the query results.
    ///   <para>
    ///     If you wish to include additional nested navigation properties, use the nested builder action.
    ///   </para>
    /// </summary>
    /// <example>
    ///   Example usage:
    ///   <code>
    ///     query
    ///         .UseIncludeBuilder()
    ///         .Include(q => q.Properties, builder => builder
    ///             .Include(p => p.FirstNestedProperties)
    ///             .Include(p => p.SecondNestedProperty)
    ///         )
    ///         .Build()
    ///   </code>
    /// </example>
    /// <typeparam name="TNext">Type of the collection of entities to be included in query result.</typeparam>
    /// <param name="navigationPropertyPath">A lambda expression for navigating from current entity to desired one for inclusion.</param>
    /// <param name="builder">
    ///   Action for configuring inclusion of nested navigation properties, continuing from the one selected by the expression.
    ///   If left empty or null, no nested includes are configured.
    /// </param>
    /// <returns>Current entity include builder, with given inclusions applied.</returns>
    TReturn Include<TNext>(
        Expression<Func<TCurrent, IEnumerable<TNext>>> navigationPropertyPath,
        Action<INestedIncludeBuilder<TBase, TNext>>? builder = null);

    /// <summary>
    ///   Specifies navigational properties to include in the query results.
    ///   <para>
    ///     If you wish to include additional nested navigation properties, use the nested builder action.
    ///   </para>
    /// </summary>
    /// <example>
    ///   Example usage:
    ///   <code>
    ///     query
    ///         .UseIncludeBuilder()
    ///         .Include(q => q.Property, builder => builder
    ///             .Include(p => p.FirstNestedProperties)
    ///             .Include(p => p.SecondNestedProperty)
    ///         )
    ///         .Build()
    ///   </code>
    /// </example>
    /// <typeparam name="TNext">Type of the entity to be included in query result.</typeparam>
    /// <param name="navigationPropertyPath">A lambda expression for navigating from current entity to desired one for inclusion.</param>
    /// <param name="builder">
    ///   Action for configuring inclusion of nested navigation properties, continuing from the one selected by the expression.
    ///   If left empty or null, no nested includes are configured.
    /// </param>
    /// <returns>Current entity include builder, with given inclusions applied.</returns>
    TReturn Include<TNext>(
        Expression<Func<TCurrent, TNext>> navigationPropertyPath,
        Action<INestedIncludeBuilder<TBase, TNext>>? builder = null);
}
