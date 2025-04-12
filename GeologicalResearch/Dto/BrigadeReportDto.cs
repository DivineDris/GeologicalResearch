namespace GeologicalResearch.Dto;
//Dto который передает данные отчета по бригаде
public record class BrigadeReportDto(
    int BrigadeId,
    string BrigadeName,
    List<RequestReportDto> Requests,
    int AmountOfFinishedRequests
);
