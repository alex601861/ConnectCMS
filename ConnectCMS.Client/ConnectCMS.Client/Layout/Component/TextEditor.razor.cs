using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.RichTextEditor;

namespace CMSTrain.Client.Layout.Component;

public partial class TextEditor
{
    [Parameter]
    public string Value { get; set; } = "";

    [Parameter]
    public string Description { get; set; } = "Description";
    
    [Parameter]
    public string Height { get; set; } = "200px";

    [Parameter] 
    public string CssClass { get; set; } = "rich-text-editor";
    
    private int SaveInterval { get; set; } = 5000;

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    private string GetPlaceHolder() => $"Enter some text as your {Description.ToLower()} here...";
    
    public async Task OnValueChanged(Syncfusion.Blazor.RichTextEditor.ChangeEventArgs args)
    {
        Value = args.Value;
        
        await ValueChanged.InvokeAsync(args.Value);
    }
    
    private readonly List<ToolbarItemModel> _tools =
    [
        new ToolbarItemModel() { Command = ToolbarCommand.Bold },
        new ToolbarItemModel() { Command = ToolbarCommand.Italic },
        new ToolbarItemModel() { Command = ToolbarCommand.Underline },
        new ToolbarItemModel() { Command = ToolbarCommand.Separator },
        new ToolbarItemModel() { Command = ToolbarCommand.Formats },
        new ToolbarItemModel() { Command = ToolbarCommand.Alignments },
        new ToolbarItemModel() { Command = ToolbarCommand.OrderedList },
        new ToolbarItemModel() { Command = ToolbarCommand.UnorderedList },
        new ToolbarItemModel() { Command = ToolbarCommand.FontColor },
        new ToolbarItemModel() { Command = ToolbarCommand.BackgroundColor },
        new ToolbarItemModel() { Command = ToolbarCommand.Undo },
        new ToolbarItemModel() { Command = ToolbarCommand.Redo },
        new ToolbarItemModel() { Command = ToolbarCommand.CreateTable },
        new ToolbarItemModel() { Command = ToolbarCommand.SuperScript },
        new ToolbarItemModel() { Command = ToolbarCommand.SubScript },
        new ToolbarItemModel() { Command = ToolbarCommand.FontName },
        new ToolbarItemModel() { Command = ToolbarCommand.FontSize },
        new ToolbarItemModel() { Command = ToolbarCommand.UpperCase },
        new ToolbarItemModel() { Command = ToolbarCommand.LowerCase },
        new ToolbarItemModel() { Command = ToolbarCommand.StrikeThrough },
        new ToolbarItemModel() { Command = ToolbarCommand.Blockquote },
        new ToolbarItemModel() { Command = ToolbarCommand.CreateLink },
        new ToolbarItemModel() { Command = ToolbarCommand.SourceCode },
        
    ];
}