using ApexCharts;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Layout.Component;

public partial class BarChart<T> : ComponentBase where T : class
{
    [Parameter] public List<T> Items { get; set; } = [];
    
    [Parameter] public Func<T, string> XValue { get; set; }
    
    [Parameter] public Func<IEnumerable<T>, Decimal?> YValue { get; set; }

    [Parameter] public bool IsHorizontallyAligned { get; set; } = true;

    [Parameter] public string Height { get; set; } = "320";

    [Parameter] public string Width { get; set; } = "600";

    [Parameter] public string Title { get; set; } = "Value";

    [Parameter] public object MaxYValue { get; set; } = "5";

    [Parameter] public bool ShowLegend { get; set; } = true;
    
    private ApexChartOptions<T> Options { get; set; }

    protected override void OnInitialized()
    {
        Options = new ApexChartOptions<T>
        {
            PlotOptions = new PlotOptions
            {
                Bar = new PlotOptionsBar
                {
                    Horizontal = IsHorizontallyAligned,
                    Distributed = true,
                    ColumnWidth = "10%"
                }
            },
            Yaxis =
            [
                new YAxis
                {
                    Max = MaxYValue
                }
            ],
            Legend = new()
            {
                Show = ShowLegend
            },
            Colors = new List<string> { "#005399", "#ff0000", "#00cc29", "#0bc5ea", "#775DD0" },
        };
    }
}