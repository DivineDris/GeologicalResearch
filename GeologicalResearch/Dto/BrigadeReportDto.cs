namespace GeologicalResearch.Dto;

public record class BrigadeReportDto(
    int BrigadeId,
    string BrigadeName,
    List<RequestReportDto> Requests,
    int AmountOfFinishedRequests
);
