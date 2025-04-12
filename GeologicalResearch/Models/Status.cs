using System;

namespace GeologicalResearch.Models;

public class Status //Модель статуса выполнения
{
    public int Id { get; set; }
    public required string StatusName { get; set;}
}
