namespace GeologicalResearch.Dto;

public record class RequestReportDto (
    int RequestId,
    string RequestDescription,
    int TimeSpent
);
