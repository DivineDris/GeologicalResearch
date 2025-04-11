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
        [HttpGet("monthly")]
        public ActionResult<List<BrigadeReportDto>> Get(int year, int  month)
        {
            var requests = dbContext.Requests
            .Include(request=>request.Brigade)
            .Include(request=>request.Status)
            .Where(request=>request.StatusId == 3
                && request.FinishDate != null
                && request.FinishDate.Value.Month == month
                && request.FinishDate.Value.Year == year).ToList();
            if(requests.Count == 0)
               throw new NotFoundException("Не возможно составить отчет. Нет заявок за данный период", "Report error. Requests not found");
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
