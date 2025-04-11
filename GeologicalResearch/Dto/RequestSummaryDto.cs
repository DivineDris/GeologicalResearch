using System.ComponentModel.DataAnnotations;

namespace GeologicalResearch.Dto;

public record class RequestSummaryDto(
    [Required]int Id,
    [Required]string RequestDescription,
    string BrigadeName,
    string StatusName,
    [Required] DateTime StartDate,
    DateTime? FinishDate,
    string? RequestNote
);
