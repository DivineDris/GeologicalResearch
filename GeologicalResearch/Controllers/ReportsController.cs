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

        //A function that triggers a GET request. Creates a report for the specified month.  
        //Includes the number of completed requests + completed requests + the amount of time in hours spent on completing the request (from the moment of creation to the closing of the request).
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
               throw new NotFoundException("Unable to generate report. No requests for this period", "Report error. Requests not found");
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
