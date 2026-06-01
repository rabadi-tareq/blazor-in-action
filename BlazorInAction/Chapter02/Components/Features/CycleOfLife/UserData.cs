namespace Chapter02.Components.Features.CycleOfLife;

public partial class CycleOfLifePage
{
    private class UserData
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
