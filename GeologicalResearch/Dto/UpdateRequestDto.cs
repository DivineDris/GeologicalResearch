using System.ComponentModel.DataAnnotations;

namespace GeologicalResearch.Dto;

public record class UpdateRequestDto(
    string? RequestDescription,
    string? Client,
    int? BrigadeId,
    int? StatusId,
    DateTime? StartDate,
    DateTime? FinishDate,
    string? RequestNote
);
