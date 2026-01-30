namespace GeologicalResearch.Dto;
//DTO for transferring report data on request
public record class RequestReportDto (
    int RequestId,
    string RequestDescription,
    int TimeSpent
);
