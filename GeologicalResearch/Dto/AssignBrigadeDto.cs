using System.ComponentModel.DataAnnotations;

namespace GeologicalResearch.Dto;

public record class AssignBrigadeDto (
    [Required]int BrigadeId
);
