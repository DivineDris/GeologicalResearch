using System.Text.Json;

namespace GeologicalResearch.Dto;
//Dto для передачи данных об ошибке
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
