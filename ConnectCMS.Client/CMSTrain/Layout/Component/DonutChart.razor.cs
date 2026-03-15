using ApexCharts;
using Microsoft.AspNetCore.Components;

namespace CMSTrain.Client.Layout.Component;

public partial class DonutChart<T> : ComponentBase where T : class
{
    [Parameter]
    public List<T> Items { get; set; } = new();

    [Parameter] 
    public Func<T, string> XValue { get; set; }

    [Parameter]
    public Func<T, decimal?> YValue { get; set; }

    private ApexChartOptions<T> BarChartOptions { get; set; } = new();

    protected override void OnInitialized()
    {
        BarChartOptions = new ApexChartOptions<T>
        {
            PlotOptions = new PlotOptions
            {
                Pie = new PlotOptionsPie
                {
                    Donut = new PlotOptionsDonut
                    {
                        Labels = new DonutLabels
                        {
                            Total = new DonutLabelTotal 
                            { 
                                Show = true,
                                FontSize = "24px",
                                Color = "#D807B8",
                                Formatter = @"function (w) {
                                    return w.globals.seriesTotals.reduce((a, b) => a + b, 0);
                                }"
                            }
                        }
                    }
                }
            }
        };
    }
}