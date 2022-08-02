using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Ainoraz.EFCore.IncludeBuilder.Builders.Interfaces;

public interface IIncludeBuilder<TBase, TCurrent, TReturn>
    where TBase : class
    where TReturn : IIncludeBuilder<TBase, TCurrent, TReturn>
{
    /// <summary>
    ///   Specifies related entities to include in the query results.
    ///   <para>
    ///     If you wish to include additional nested entities, use the builder action.
    ///   </para>
    /// </summary>
    /// <example>
    ///   For example:
    ///   <code>
    ///     query
    ///         .UseIncludeBuilder()
    ///         .Include(q => q.Properties, builder => builder
    ///             .Include(p => p.NestedProperty1)
    ///             .Include(p => p.NestedProperty2)
    ///         )
    ///         .Build()
    ///   </code>
    /// </example>
    /// <typeparam name="TNext">Type of the entity enumerable to be included.</typeparam>
    /// <param name="navigationPropertyPath">A lambda expression for navigating from current entity to desired one.</param>
    /// <param name="builder">
    ///   Action for configuring inclusion of nested entities, continuing from the one selected by the expression.
    ///   If left empty or null, no further includes are configured.
    /// </param>
    /// <returns>Current entity builder, with given inclusions applied.</returns>
    TReturn Include<TNext>(
        Expression<Func<TCurrent, IEnumerable<TNext>>> navigationPropertyPath,
        Action<INestedIncludeBuilder<TBase, TNext>>? builder = null);

    /// <summary>
    ///   Specifies related entities to include in the query results.
    ///   <para>
    ///     If you wish to include additional nested entities, use the builder action.
    ///   </para>
    /// </summary>
    /// <example>
    ///   For example:
    ///   <code>
    ///     query
    ///         .UseIncludeBuilder()
    ///         .Include(q => q.Property1, builder => builder
    ///             .Include(p => p.NestedProperty1)
    ///             .Include(p => p.NestedProperty2)
    ///         )
    ///         .Build()
    ///   </code>
    /// </example>
    /// <typeparam name="TNext">Type of the entity to be included.</typeparam>
    /// <param name="navigationPropertyPath">A lambda expression for navigating from current entity to desired one.</param>
    /// <param name="builder">
    ///   Action for configuring inclusion of nested entities, continuing from the one selected by the expression.
    ///   If left empty or null, no further includes are configured.
    /// </param>
    /// <returns>Current entity builder, with given inclusions applied.</returns>
    TReturn Include<TNext>(
        Expression<Func<TCurrent, TNext>> navigationPropertyPath,
        Action<INestedIncludeBuilder<TBase, TNext>>? builder = null);
}
