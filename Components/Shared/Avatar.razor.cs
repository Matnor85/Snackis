using Microsoft.AspNetCore.Components;

namespace Snackis.Components.Shared;

public partial class Avatar
{
    [Parameter] public string? ImagePath { get; set; }
    [Parameter] public string Name { get; set; } = "";
    [Parameter] public int Size { get; set; } = 48;

    private string Initial => string.IsNullOrEmpty(Name) ? "?" : Name[..1].ToUpper();
}
