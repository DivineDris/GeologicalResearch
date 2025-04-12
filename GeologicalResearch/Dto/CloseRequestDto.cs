using System.ComponentModel.DataAnnotations;

namespace GeologicalResearch.Dto;
//DTO для передачи данных в запрос закрытия заявки
public record class CloseRequestDto(
    string? RequestNote
);

