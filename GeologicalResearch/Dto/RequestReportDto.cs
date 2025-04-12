namespace GeologicalResearch.Dto;
//Dto для передачи данных отчета по заявке
public record class RequestReportDto (
    int RequestId,
    string RequestDescription,
    int TimeSpent
);
