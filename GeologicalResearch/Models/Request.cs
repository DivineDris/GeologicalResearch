using System;

namespace GeologicalResearch.Models;

public class Request
{
    public int Id { get; set; }
    public required string RequestDescription { get; set;}
    public int StatusId { get; set; }
    public Status? Status{ get; set; }
    public int BrigadeId{ get; set; }
    public Brigade? Brigade { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public string? RequestNote { get; set; }

}
