using GeologicalResearch.Data;
using GeologicalResearch.Dto;
using GeologicalResearch.Exceptions;
using GeologicalResearch.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeologicalResearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController(GRDataContext dbContext) : ControllerBase
    {

        //Метод который вызывает GET запрос. Создает отчет за указанный месяц. 
        // Включает количество выполненных заявок + выполненные заявки + количество времени в часах потраченное на выполнение заявки (от момента создания до закрытия заявки).
        [HttpGet("monthly")]
        public async Task<ActionResult<List<BrigadeReportDto>>> GetReport(int year, int  month)
        {
            var requests = await dbContext.Requests
            .Include(request=>request.Brigade)
            .Include(request=>request.Status)
            .Where(request=>request.StatusId == 3
                && request.FinishDate != null
                && request.FinishDate.Value.Year == year
                && request.FinishDate.Value.Month == month).ToListAsync();
            if(requests.Count == 0)
               throw new NotFoundException("Невозможно составить отчет. Нет заявок за данный период", "Report error. Requests not found");
            var groupedRequests = requests.GroupBy(request => request.Brigade)
            .Select(groupedRequest => new BrigadeReportDto
            (
                groupedRequest.Key!.Id,
                groupedRequest.Key.BrigadeName,
                groupedRequest.Select(request=>request.ToRequestReportDto()).ToList(),
                groupedRequest.Count()
            ))
            .OrderBy(brigadeReport => brigadeReport.BrigadeId)
            .ToList();
            return Ok(groupedRequests);
        }
    }
}
