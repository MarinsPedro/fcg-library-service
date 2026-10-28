using System.Reflection;
using System.Xml.Linq;
using FiapCloudGames.Library.Domain.Entries;

namespace FiapCloudGames.Library.ArchitectureTests;

public sealed class ArchitectureRulesTests
{
    [Fact(DisplayName = "Verificar referencias - devem seguir a direcao da arquitetura limpa")]
    public void Check_ProjectReferences_FollowCleanArchitectureDirection()
    {
        AssertProjectReferences("Domain");
        AssertProjectReferences("Contracts");
        AssertProjectReferences("Application", "Domain");
        AssertProjectReferences("Infrastructure", "Application", "Domain");
        AssertProjectReferences("Presentation", "Application");
        AssertProjectReferences("Api", "Infrastructure", "Presentation");
        AssertProjectReferences("Migrations", "Infrastructure");
        AssertProjectReferences("Worker", "Application", "Contracts", "Infrastructure");
    }

    [Fact(DisplayName = "Verificar testes unitarios - devem referenciar apenas Domain e Application")]
    public void Check_UnitTests_ReferenceOnlyDomainAndApplication()
    {
        var root = FindRepositoryRoot();
        var projectPath = Path.Combine(
            root,
            "tests",
            "FiapCloudGames.Library.UnitTests",
            "FiapCloudGames.Library.UnitTests.csproj");
        var project = XDocument.Load(projectPath);
        var references = project
            .Descendants("ProjectReference")
            .Select(element => Path.GetFileNameWithoutExtension(
                (string?)element.Attribute("Include")))
            .OfType<string>()
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "FiapCloudGames.Library.Application",
                "FiapCloudGames.Library.Domain"
            ],
            references);
    }

    [Fact(DisplayName = "Verificar dominio - nao deve depender de camadas externas")]
    public void Check_Domain_DoesNotReferenceOuterLayersOrFrameworks()
    {
        AssertDoesNotReference(
            typeof(LibraryEntry).Assembly,
            "Application",
            "Infrastructure",
            "Presentation",
            "MassTransit",
            "Microsoft.AspNetCore",
            "Microsoft.EntityFrameworkCore");
    }

    [Fact(DisplayName = "Verificar application - nao deve depender de infraestrutura HTTP ou EF")]
    public void Check_Application_DoesNotReferenceOuterLayersOrHttpAndDataFrameworks()
    {
        AssertDoesNotReference(
            typeof(FiapCloudGames.Library.Application.DependencyInjection).Assembly,
            "Infrastructure",
            "Presentation",
            "MassTransit",
            "Microsoft.AspNetCore",
            "Microsoft.EntityFrameworkCore");
    }

    [Fact(DisplayName = "Verificar contracts - deve permanecer independente")]
    public void Check_Contracts_RemainIndependent()
    {
        AssertDoesNotReference(
            typeof(FiapCloudGames.Library.Contracts.Events.V1.GameGrantRequestedEventV1).Assembly,
            "Application",
            "Domain",
            "Infrastructure",
            "Presentation",
            "MassTransit",
            "Microsoft.AspNetCore",
            "Microsoft.EntityFrameworkCore");
    }

    private static void AssertDoesNotReference(
        Assembly assembly,
        params string[] forbiddenParts)
    {
        var references = assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty);
        var violations = references
            .Where(reference => forbiddenParts.Any(part =>
                reference.Contains(part, StringComparison.Ordinal)))
            .ToArray();

        Assert.Empty(violations);
    }

    private static void AssertProjectReferences(
        string layer,
        params string[] expectedLayers)
    {
        var root = FindRepositoryRoot();
        var projectPath = Path.Combine(
            root,
            "src",
            $"FiapCloudGames.Library.{layer}",
            $"FiapCloudGames.Library.{layer}.csproj");
        var references = XDocument.Load(projectPath)
            .Descendants("ProjectReference")
            .Select(element => (string?)element.Attribute("Include"))
            .Where(include => include?.Contains(
                "FiapCloudGames.Library.",
                StringComparison.Ordinal) == true)
            .Select(Path.GetFileNameWithoutExtension)
            .Select(name => name?.Split('.').Last())
            .Where(name => name is not null)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expectedLayers.Order(StringComparer.Ordinal), references);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null &&
               !File.Exists(Path.Combine(
                   directory.FullName,
                   "FiapCloudGames.Library.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new DirectoryNotFoundException(
                "A raiz do repositorio nao foi encontrada.");
    }
}
