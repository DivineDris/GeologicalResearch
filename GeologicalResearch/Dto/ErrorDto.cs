using System.Text.Json;

namespace GeologicalResearch.Dto;
//DTO for transferring error data
public record class ErrorDto(
    int StatusCode,
    string Message
)
{
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
