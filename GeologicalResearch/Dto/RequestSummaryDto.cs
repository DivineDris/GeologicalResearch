using System.ComponentModel.DataAnnotations;

namespace GeologicalResearch.Dto;

public record class RequestSummaryDto(
    int Id,
    string RequestDescription,
    string Client,
    string? BrigadeName,
    string StatusName,
    DateTime StartDate,
    DateTime? FinishDate,
    string? RequestNote
);
