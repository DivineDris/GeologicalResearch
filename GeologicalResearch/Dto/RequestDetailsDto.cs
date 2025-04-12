using System.ComponentModel.DataAnnotations;

namespace GeologicalResearch.Dto;

public record class RequestDetailsDto(
    [Required]int Id,
    [Required]string RequestDescription,
    int? BrigadeId,
    [Required]int StatusId,
    [Required] DateTime StartDate,
    DateTime? FinishDate,
    string? RequestNote
);
