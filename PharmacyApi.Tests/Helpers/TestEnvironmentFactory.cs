using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Moq;

namespace PharmacyApi.Tests.Helpers;

public static class TestEnvironmentFactory
{
    public static IWebHostEnvironment Create(string? rootPath = null)
    {
        rootPath ??= Path.Combine(Path.GetTempPath(), "PharmacyApiTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(rootPath);

        var environment = new Mock<IWebHostEnvironment>();
        environment.Setup(e => e.ContentRootPath).Returns(rootPath);
        environment.Setup(e => e.EnvironmentName).Returns(Environments.Development);
        environment.Setup(e => e.ApplicationName).Returns("PharmacyApi.Tests");
        environment.Setup(e => e.WebRootPath).Returns(Path.Combine(rootPath, "wwwroot"));

        return environment.Object;
    }
}
