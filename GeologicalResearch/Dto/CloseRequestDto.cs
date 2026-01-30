using System.ComponentModel.DataAnnotations;

namespace GeologicalResearch.Dto;
//DTO for transferring data to a request to close an application
public record class CloseRequestDto(
    string? RequestNote
);

