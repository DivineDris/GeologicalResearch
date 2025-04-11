using System.Text.Json;

namespace GeologicalResearch.Dto;

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
