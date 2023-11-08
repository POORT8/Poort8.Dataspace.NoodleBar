using Microsoft.AspNetCore.Components;

namespace Poort8.Dataspace.CoreManager.Components.Molecules;

public partial class TableMolecule<TItem>
{
    [Parameter]
    public RenderFragment? TableHeader { get; set; }

    [Parameter]
    public RenderFragment<TItem>? RowTemplate { get; set; }

    [Parameter]
    public IReadOnlyList<TItem>? Items { get; set; }

    [Parameter]
    public EventCallback<TItem> RowClick { get; set; }
}