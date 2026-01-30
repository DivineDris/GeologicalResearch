using System.ComponentModel.DataAnnotations;

namespace GeologicalResearch.Dto;
//DTO for data transfer in assignBrigade request
public record class AssignBrigadeDto (
    [Required]int BrigadeId
);
