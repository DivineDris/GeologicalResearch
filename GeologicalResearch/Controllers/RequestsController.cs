using System.Threading.Tasks;
using GeologicalResearch.Data;
using GeologicalResearch.Dto;
using GeologicalResearch.Exceptions;
using GeologicalResearch.Mapping;
using GeologicalResearch.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeologicalResearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController (GRDataContext dbContext) : ControllerBase
    {
        [HttpPost("new")]
        public async Task<ActionResult<Request>> PostNewRequest(CreateRequestDto createRequestDto)
        {
            Request request = createRequestDto.ToEntity();
            await dbContext.Requests.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return Ok(request.ToRequestDetailsDto());
        }

        [HttpGet]
        public async Task<ActionResult<Request>> GetAllRequests()
        {
            var requests = await dbContext.Requests
            .Include(request=>request.Brigade)
            .Include(request=>request.Status)
            .Select(request => request.ToRequestSummaryDto())
            .AsNoTracking().ToListAsync();

            return requests.Count() == 0 ? throw new NotFoundException("Заявки не найдены",  "Requests not found") : Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequestById(int id)
        {
            Request? request = await dbContext.Requests.FindAsync(id);
            return request is null ? throw new NotFoundException ($"Заявка с id:{id} не найдена", "Request not found") : Ok(request.ToRequestDetailsDto());
        }

        [HttpPut("{id}/assignBrigade")]
        public async Task<ActionResult<Request>> PutAssignBrigadeForRequest(int id, AssignBrigadeDto assignBrigadeDto)
        {
            if(assignBrigadeDto.BrigadeId < 1 || assignBrigadeDto.BrigadeId > 3)
                throw new ValidationException("Ошибка валидации передаваемых данных", "Invalid values");

             var existingRequest = await dbContext.Requests.FindAsync(id);
             if(existingRequest is null)
                throw new NotFoundException ($"Заявка с id:{id} не найдена", "Request not found");
            
            dbContext.Entry(existingRequest)
                        .CurrentValues
                        .SetValues(assignBrigadeDto.ToEntity(existingRequest));
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("{id}/update")]
        public async Task<ActionResult<Request>> PutUpdateRequestById(int id, UpdateRequestDto updateRequestDto)
        {
            if(updateRequestDto.BrigadeId > 3 ||
            updateRequestDto.StatusId > 3 ||
            updateRequestDto.StartDate > updateRequestDto.FinishDate)
                throw new ValidationException("Ошибка валидации передаваемых данных", "Invalid values");
            var existingRequest = await dbContext.Requests.FindAsync(id);
            
            if(existingRequest is null)
                throw new NotFoundException ($"Заявка с id:{id} не найдена", "Request not found");
            dbContext.Entry(existingRequest)
                        .CurrentValues
                        .SetValues(updateRequestDto.ToEntity(existingRequest));
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("{id}/close")]
        public async Task<ActionResult<Request>> PutCloseRequestById(int id, CloseRequestDto closeRequestDto)
        {
            var existingRequest = await dbContext.Requests.FindAsync(id);
            if (existingRequest is null)
                throw new NotFoundException ($"Заявка с id:{id} не найдена", "Request not found");
            if(existingRequest.StartDate > DateTime.Now)
                throw new ValidationException("Дата открытия заявки больше даты закрытия заявки", "Start date > Finish date");
            dbContext.Entry(existingRequest)
                        .CurrentValues
                        .SetValues(closeRequestDto.ToEntity(existingRequest));
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}/delete")]
        public async Task<ActionResult<Request>> DeleteRequestById(int id)
        {
            if(await dbContext.Requests.FindAsync(id) is null)
                throw new NotFoundException ($"Заявка с id:{id} не найдена", "Request not found");

            await dbContext.Requests.Where(request => request.Id == id).ExecuteDeleteAsync();
            return NoContent();
        }

    }
}
