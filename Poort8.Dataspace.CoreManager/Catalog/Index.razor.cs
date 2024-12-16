using Microsoft.AspNetCore.Components;
using Poort8.Dataspace.CoreManager.Services;
using Poort8.Dataspace.OrganizationRegistry;
namespace Poort8.Dataspace.CoreManager.Catalog;

public partial class Index : ComponentBase
{
    private const string _noLogoUrl = "https://fakeimg.pl/50x50?text=Logo&font=bebas";
    [Inject]
    public required IOrganizationRegistry OrganizationRegistry { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required OpenApiService OpenApiService { get; set; }
    [Inject]
    public required ILogger<Index> Logger { get; set; }
    public List<DataSourceViewModel> DataSources { get; private set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var organizations = await OrganizationRegistry.ReadOrganizations();

        foreach (var organization in organizations)
        {
            if (!string.IsNullOrEmpty(organization.AdditionalDetails.CapabilitiesUrl))
            {
                var logoUrl = string.IsNullOrEmpty(organization.AdditionalDetails.LogoUrl) ? _noLogoUrl : organization.AdditionalDetails.LogoUrl;

                try
                {
                    var dataSource = await OpenApiService.GetDataSourceViewModel(
                                organization.AdditionalDetails.CapabilitiesUrl,
                                organization.Name,
                                logoUrl);

                    if (dataSource != null)
                        DataSources.Add(dataSource);
                }
                catch (Exception)
                {
                    Logger.LogError("P8.err - Could not add catalog datasource: {dataSourceUrl}", organization.AdditionalDetails.CapabilitiesUrl);
                }
            }
        }
    }

    private void GoToScalar(DataSourceViewModel dataSource)
    {
        NavigationManager.NavigateTo("/api-reference?url=" + dataSource.Url, true);
    }

    public class DataSourceViewModel
    {
        public required string Type { get; set; }
        public string? LogoUrl { get; set; }
        public required string Organization { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public MarkupString? DescriptionMarkdown { get; set; }
        public required string Version { get; set; }
        public required string Swagger { get; set; }
        public string? Terms { get; set; }
        public string? License { get; set; }
        public string? Contact { get; set; }
        public string? Docs { get; set; }
        public required string Host { get; set; }
        public required string Url { get; set; }
    }
}
