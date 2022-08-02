namespace Ainoraz.EFCore.IncludeBuilder.Builders.Interfaces;

public interface INestedIncludeBuilder<TBase, TCurrent> :
    IIncludeBuilder<TBase, TCurrent, INestedIncludeBuilder<TBase, TCurrent>>
    where TBase : class
{
}
