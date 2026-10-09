using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IssueTracker.Api.DTOs;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class CreateTicketDto
{
    [Required(ErrorMessage = "Titel måste anges och får inte enbart innehålla blanksteg.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Titel måste vara mellan 5 och 100 tecken.")]
    public string Title { get; init; } = string.Empty;

    [Required(ErrorMessage = "Beskrivning måste anges och får inte enbart innehålla blanksteg.")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Beskrivning måste vara mellan 10 och 1000 tecken.")]
    public string Description { get; init; } = string.Empty;

    [Required(ErrorMessage = "Prioritet måste anges.")]
    [AllowedValues("Low", "Normal", "High", ErrorMessage = "Prioritet måste vara Low, Normal eller High.")]
    public string Priority { get; init; } = string.Empty;
}
