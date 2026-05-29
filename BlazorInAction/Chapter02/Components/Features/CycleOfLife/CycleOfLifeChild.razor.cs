using Microsoft.AspNetCore.Components;

namespace Chapter02.Components.Features.CycleOfLife;

public partial class CycleOfLifeChild : IDisposable
{
    [Parameter] public string ParentMessage { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> OnChildEvent { get; set; }

    private int childRenderCount;
    private string previousMessage = string.Empty;

    protected override void OnInitialized()
    {
        previousMessage = ParentMessage;
    }

    protected override void OnParametersSet()
    {
        // Demonstrates detecting parameter changes
        if (previousMessage != ParentMessage)
        {
            previousMessage = ParentMessage;
            // Could trigger additional logic here
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        childRenderCount++;
    }

    private async Task NotifyParent()
    {
        await OnChildEvent.InvokeAsync($"Child event triggered at {DateTime.Now:HH:mm:ss}");
    }

    public void Dispose()
    {
        // Cleanup child component resources
    }
}
