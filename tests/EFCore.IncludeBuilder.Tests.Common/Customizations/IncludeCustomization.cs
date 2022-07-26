using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace Ainoraz.EFCore.IncludeBuilder.Tests.Common.Customizations;

public class IncludeCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));

        fixture.Behaviors.Add(new OmitOnRecursionBehavior(recursionDepth: 1));

        fixture.Customize(new AutoMoqCustomization());
    }
}