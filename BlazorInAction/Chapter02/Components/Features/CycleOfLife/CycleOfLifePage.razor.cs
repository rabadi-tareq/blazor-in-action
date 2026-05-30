using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Chapter02.Components.Features.CycleOfLife;

public partial class CycleOfLifePage : IAsyncDisposable
{
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    private List<LifecycleLogEntry> lifecycleLog = new List<LifecycleLogEntry>();
    private UserData? userData;
    private bool isLoading = true;
    private bool showChildComponent;
    private string currentPhase = "Initializing";

    private int renderCount;
    private int parametersSetCount;
    private int afterRenderCount;

    private IJSObjectReference? jsModule;
    private DotNetObjectReference<CycleOfLifePage>? dotNetReference;
    private CancellationTokenSource? cancellationTokenSource;

    /// <summary>
    /// Phase 1: Constructor - Called when the component instance is created.
    /// Used for: Simple field initialization, no async operations.
    /// </summary>
    public CycleOfLifePage()
    {
        LogLifecycleEvent("Constructor", "Component instance created");
        cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// Phase 2: SetParametersAsync - Called when parameters are set from parent.
    /// Used for: Custom parameter handling, early validation.
    /// </summary>
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        LogLifecycleEvent("SetParametersAsync", $"Setting parameters (Count: {++parametersSetCount})");

        // Custom parameter processing can happen here before calling base
        await base.SetParametersAsync(parameters);

        LogLifecycleEvent("SetParametersAsync", "Parameters set complete");
    }

    /// <summary>
    /// Phase 3: OnInitialized - Called once when component is initialized.
    /// Used for: Synchronous initialization logic.
    /// </summary>
    protected override void OnInitialized()
    {
        LogLifecycleEvent("OnInitialized", "Synchronous initialization started");
        currentPhase = "Initialized";

        // Synchronous initialization work
        userData = new UserData 
        { 
            Name = "Loading...", 
            Email = "Loading...",
            LastUpdated = DateTime.Now 
        };

        LogLifecycleEvent("OnInitialized", "Synchronous initialization complete");
    }

    /// <summary>
    /// Phase 4: OnInitializedAsync - Called once after OnInitialized.
    /// Used for: Async initialization (API calls, data loading, etc.)
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        LogLifecycleEvent("OnInitializedAsync", "Async initialization started");
        currentPhase = "Loading Data";

        try
        {
            // Simulate async data loading (e.g., API call)
            await LoadUserDataAsync();

            LogLifecycleEvent("OnInitializedAsync", "Data loaded successfully");
        }
        catch (Exception ex)
        {
            LogLifecycleEvent("OnInitializedAsync", $"Error: {ex.Message}");
        }
        finally
        {
            isLoading = false;
            currentPhase = "Ready";
        }
    }

    /// <summary>
    /// Phase 5: OnParametersSet - Called after parameters are set.
    /// Used for: Reacting to parameter changes with synchronous logic.
    /// </summary>
    protected override void OnParametersSet()
    {
        LogLifecycleEvent("OnParametersSet", "Parameters have been set, component updating");
        currentPhase = "Parameters Set";
    }

    /// <summary>
    /// Phase 6: OnParametersSetAsync - Called after OnParametersSet.
    /// Used for: Async operations needed when parameters change.
    /// </summary>
    protected override async Task OnParametersSetAsync()
    {
        LogLifecycleEvent("OnParametersSetAsync", "Async parameter processing");

        // Async work that depends on parameters
        await Task.CompletedTask;
    }

    /// <summary>
    /// Phase 7: OnAfterRender - Called after component has rendered.
    /// Used for: JS interop, DOM manipulation, measuring rendered content.
    /// </summary>
    protected override void OnAfterRender(bool firstRender)
    {
        afterRenderCount++;

        if (firstRender)
        {
            LogLifecycleEvent("OnAfterRender", "First render complete - DOM is ready");
            currentPhase = "First Render Complete";
        }
        else
        {
            LogLifecycleEvent("OnAfterRender", $"Re-render complete (Count: {renderCount})");
            currentPhase = "Re-rendered";
        }
    }

    /// <summary>
    /// Phase 8: OnAfterRenderAsync - Called after OnAfterRender.
    /// Used for: Async JS interop, initializing JS libraries.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            LogLifecycleEvent("OnAfterRenderAsync", "Initializing JavaScript interop");

            try
            {
                // Initialize JS module (example of JS interop best practice)
                jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>(
                    "import", 
                    "./Components/Features/CycleOfLife/CycleOfLifePage.razor.js",
                    cancellationTokenSource!.Token);

                // Create .NET reference for JS callbacks
                dotNetReference = DotNetObjectReference.Create<CycleOfLifePage>(this);

                await jsModule.InvokeVoidAsync("initialize", dotNetReference);

                LogLifecycleEvent("OnAfterRenderAsync", "JavaScript interop initialized");
            }
            catch (Exception ex)
            {
                LogLifecycleEvent("OnAfterRenderAsync", $"JS Init Error: {ex.Message}");
            }
        }

        renderCount++;
    }

    /// <summary>
    /// ShouldRender - Controls whether the component should re-render.
    /// Used for: Performance optimization by preventing unnecessary renders.
    /// </summary>
    protected override bool ShouldRender()
    {
        // In this demo, we always render, but in production you might add logic like:
        // return hasDataChanged || forceRender;

        LogLifecycleEvent("ShouldRender", $"Determining if render needed (returning true)");
        return true;
    }

    private async Task LoadUserDataAsync()
    {
        LogLifecycleEvent("LoadUserData", "Fetching user data...");

        // Simulate API call with cancellation support
        await Task.Delay(1500, cancellationTokenSource!.Token);

        userData = new UserData
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            LastUpdated = DateTime.Now
        };

        LogLifecycleEvent("LoadUserData", "User data loaded");
    }

    private async Task RefreshDataAsync()
    {
        LogLifecycleEvent("RefreshData", "Manually refreshing data");
        currentPhase = "Refreshing";
        isLoading = true;

        await LoadUserDataAsync();

        isLoading = false;
        currentPhase = "Ready";

        // Force re-render
        StateHasChanged();
    }

    private void ToggleChildVisibility()
    {
        showChildComponent = !showChildComponent;
        LogLifecycleEvent("ToggleChild", $"Child component visibility: {showChildComponent}");
    }

    private void HandleChildEvent(string message)
    {
        LogLifecycleEvent("ChildEvent", $"Received from child: {message}");
    }

    private void ClearLog()
    {
        lifecycleLog.Clear();
        LogLifecycleEvent("ClearLog", "Log cleared");
    }

    private void LogLifecycleEvent(string phase, string message)
    {
        lifecycleLog.Add(new LifecycleLogEntry
        {
            Phase = phase,
            Message = message,
            Timestamp = DateTime.Now
        });

        // Keep log size manageable
        if (lifecycleLog.Count > 50)
        {
            lifecycleLog.RemoveAt(0);
        }
    }

    /// <summary>
    /// JS Interop callback example
    /// </summary>
    [JSInvokable]
    public void OnJavaScriptCallback(string message)
    {
        LogLifecycleEvent("JSCallback", $"Received from JavaScript: {message}");
        StateHasChanged();
    }

    /// <summary>
    /// Phase 9: Dispose/DisposeAsync - Called when component is being removed.
    /// Used for: Cleanup of resources, event unsubscription, cancellation tokens.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        LogLifecycleEvent("DisposeAsync", "Component disposing - cleaning up resources");

        try
        {
            // Cancel any ongoing operations
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();

            // Dispose JS interop resources
            if (jsModule is not null)
            {
                await jsModule.InvokeVoidAsync("dispose");
                await jsModule.DisposeAsync();
            }

            // Dispose .NET reference
            dotNetReference?.Dispose();

            LogLifecycleEvent("DisposeAsync", "Cleanup complete");
        }
        catch (Exception ex)
        {
            LogLifecycleEvent("DisposeAsync", $"Disposal error: {ex.Message}");
        }
    }

    private record LifecycleLogEntry
    {
        public required string Phase { get; init; }
        public required string Message { get; init; }
        public DateTime Timestamp { get; init; }
    }

    private class UserData
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
