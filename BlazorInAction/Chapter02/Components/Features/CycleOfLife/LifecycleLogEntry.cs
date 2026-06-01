namespace Chapter02.Components.Features.CycleOfLife;

public partial class CycleOfLifePage
{
    private record LifecycleLogEntry
    {
        public required string Phase { get; init; }
        public required string Message { get; init; }
        public DateTime Timestamp { get; init; }

        public string? Color { get; set; }
    }
}
