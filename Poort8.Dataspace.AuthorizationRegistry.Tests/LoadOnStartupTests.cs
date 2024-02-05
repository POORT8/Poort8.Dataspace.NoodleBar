using Microsoft.Extensions.DependencyInjection;
using Poort8.Dataspace.AuthorizationRegistry.Tests.Data;
using Poort8.Dataspace.AuthorizationRegistry.Tests.Fakes;

namespace Poort8.Dataspace.AuthorizationRegistry.Tests;
public class LoadOnStartupTests
{
    [Fact]
    public async Task LoadPolciesOnStartupTest()
    {
        //Create a FakeRepository and seed it with policies
        var fakeRepository = new FakeRepository();
        var policy = TestData.CreateNewPolicy();
        await fakeRepository.CreatePolicy(policy);

        //Now create the AR with the seeded database
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IRepository>(fakeRepository);
        serviceCollection.AddSingleton<IAuthorizationRegistry, AuthorizationRegistry>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var ar = serviceProvider.GetRequiredService<IAuthorizationRegistry>();

        var policies = await ar.ReadPolicies();
        Assert.NotEmpty(policies);

        var allowed = await ar.Enforce(policy.SubjectId, policy.ResourceId, policy.Action);
        Assert.True(allowed);
    }

    [Fact]
    public async Task LoadPolciesAndGroupsOnStartupTest()
    {
        //Create a FakeRepository and seed it with policies
        var fakeRepository = new FakeRepository();

        var organization = TestData.CreateNewOrganization(nameof(LoadPolciesAndGroupsOnStartupTest), 1);
        var employee = TestData.CreateNewEmployee(nameof(LoadPolciesAndGroupsOnStartupTest), 1);
        organization.Employees.Add(employee);
        await fakeRepository.CreateOrganization(organization);

        var policy = TestData.CreateNewPolicy();
        policy.SubjectId = organization.Identifier;
        await fakeRepository.CreatePolicy(policy);

        //Now create the AR with the seeded database
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IRepository>(fakeRepository);
        serviceCollection.AddSingleton<IAuthorizationRegistry, AuthorizationRegistry>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var ar = serviceProvider.GetRequiredService<IAuthorizationRegistry>();

        var policies = await ar.ReadPolicies();
        Assert.NotEmpty(policies);

        var organizations = await ar.ReadOrganizations();
        Assert.NotEmpty(organizations);

        var allowed = await ar.Enforce(organization.Identifier, policy.ResourceId, policy.Action);
        Assert.True(allowed);

        allowed = await ar.Enforce(employee.EmployeeId, policy.ResourceId, policy.Action);
        Assert.True(allowed);
    }
}
