using System.ComponentModel.DataAnnotations;

namespace GeologicalResearch.Dto;
//DTO для передачи данных в assignBrigade запрос
public record class AssignBrigadeDto (
    [Required]int BrigadeId
);
