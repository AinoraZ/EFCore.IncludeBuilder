using AutoFixture;
using AutoFixture.Xunit2;

namespace Ainoraz.EFCore.IncludeBuilder.Tests.Common.Customizations;

public class IncludeAutoDataAttribute : AutoDataAttribute
{
    public IncludeAutoDataAttribute() : base(GenerateFixture)
    {
    }

    public static IFixture GenerateFixture()
    {
        var fixture = new Fixture();
        var customization = new IncludeCustomization();

        customization.Customize(fixture);

        return fixture;
    }
}