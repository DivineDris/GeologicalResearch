namespace GeologicalResearch.Dto;
//DTO that transmits the report data on the brigade
public record class BrigadeReportDto(
    int BrigadeId,
    string BrigadeName,
    List<RequestReportDto> Requests,
    int AmountOfFinishedRequests
);
