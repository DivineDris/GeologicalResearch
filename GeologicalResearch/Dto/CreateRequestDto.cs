using System.ComponentModel.DataAnnotations;

namespace GeologicalResearch.Dto;

public record class CreateRequestDto (
[Required][StringLength(200)]string RequestDescription,
[Required]string Client
);
