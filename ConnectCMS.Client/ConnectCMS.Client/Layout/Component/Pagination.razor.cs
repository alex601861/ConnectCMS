using CMSTrain.Client.Models.Constants;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Layout.Component;

public partial class Pagination
{
    [Parameter] public int PageCount { get; set; }

    [Parameter] public int TotalRecords { get; set; }
    
    [Parameter] public int SelectedPageSize { get; set; } = Constants.Pagination.Size;
    
    [Parameter] public EventCallback<int> SelectedPageSizeChanged { get; set; }
    
    [Parameter] public int SelectedPageNumber { get; set; } = 1;
    
    [Parameter] public EventCallback<int> SelectedPageNumberChanged { get; set; }

    private int StartRecord => ((SelectedPageNumber - 1) * SelectedPageSize) + 1;
    
    private int EndRecord => Math.Min(SelectedPageNumber * SelectedPageSize, TotalRecords);
    
    private async Task OnPageSizeChanged(int pageSize)
    {
        SelectedPageSize = pageSize;
        SelectedPageNumber = 1; 
        await SelectedPageSizeChanged.InvokeAsync(pageSize);
        await SelectedPageNumberChanged.InvokeAsync(1);
    }

    private async Task OnPageNumberChanged(int pageNumber)
    {
        SelectedPageNumber = pageNumber;
        await SelectedPageNumberChanged.InvokeAsync(pageNumber);
    }
}