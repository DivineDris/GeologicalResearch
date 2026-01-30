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
    public class RequestsController (GRDataContext dbContext) : ControllerBase //Операции с заявками
    {
        //A function that triggers a POST request. Creates a new application.
        [HttpPost("new")]
        public async Task<ActionResult<Request>> PostNewRequest(CreateRequestDto createRequestDto)
        {
            Request request = createRequestDto.ToEntity();
            await dbContext.Requests.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return Ok(request.ToRequestDetailsDto());
        }
        //A function that triggers a GET request. Returns all created requests.
        [HttpGet]
        public async Task<ActionResult<Request>> GetAllRequests()
        {
            var requests = await dbContext.Requests
            .Include(request=>request.Brigade)
            .Include(request=>request.Status)
            .Select(request => request.ToRequestSummaryDto())
            .AsNoTracking().ToListAsync();

            return requests.Count() == 0 ? throw new NotFoundException("Requests not found",  "Requests not found") : Ok(requests);
        }
        //A function that triggers a GET request. Returns 1 request for the specified id.
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequestById(int id)
        {
            Request? request = await dbContext.Requests.FindAsync(id);
            return request is null ? throw new NotFoundException ($"Request id:{id} not found", "Request not found") : Ok(request.ToRequestDetailsDto());
        }
        //A function that triggers a PUT request. Modifies the request data. Needed to assign a brigade to the request.
        [HttpPut("{id}/assignBrigade")]
        public async Task<ActionResult<Request>> PutAssignBrigadeForRequest(int id, AssignBrigadeDto assignBrigadeDto)
        {
            if(assignBrigadeDto.BrigadeId < 1 || assignBrigadeDto.BrigadeId > 3)
                throw new ValidationException($"Error validating transmitted data. BrigadeId cannot be = {assignBrigadeDto.BrigadeId}", "Invalid values");

             var existingRequest = await dbContext.Requests.FindAsync(id);
             if(existingRequest is null)
                throw new NotFoundException ($"Request with id:{id} not found", "Request not found");
            
            dbContext.Entry(existingRequest)
                        .CurrentValues
                        .SetValues(assignBrigadeDto.ToEntity(existingRequest));
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
        //A function that triggers a PUT request. Changes the request data. Needed to assign a brigade to the request.
        [HttpPut("{id}/update")]
        public async Task<ActionResult<Request>> PutUpdateRequestById(int id, UpdateRequestDto updateRequestDto)
        {
            if(updateRequestDto.BrigadeId > 3)
                throw new ValidationException($"Error validating transmitted data. BrigadeId cannot be = {updateRequestDto.BrigadeId}", "Invalid values");
            if(updateRequestDto.StatusId > 3)
                    throw new ValidationException($"Error validating transmitted data. StatusId cannot be = {updateRequestDto.StatusId}", "Invalid values");
            if(updateRequestDto.StartDate > updateRequestDto.FinishDate)
                throw new ValidationException("Error validating transmitted data. FinishDate cannot be earlier than StartDate.", "Invalid values");
            var existingRequest = await dbContext.Requests.FindAsync(id);
            
            if(existingRequest is null)
                throw new NotFoundException ($"Request with id:{id} not found", "Request not found");
            dbContext.Entry(existingRequest)
                        .CurrentValues
                        .SetValues(updateRequestDto.ToEntity(existingRequest));
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
        //A function that triggers a PUT request. Changes the request data. Needed to close the request (i.e., mark the application as completed).
        [HttpPut("{id}/close")]
        public async Task<ActionResult<Request>> PutCloseRequestById(int id, CloseRequestDto closeRequestDto)
        {
            var existingRequest = await dbContext.Requests.FindAsync(id);
            if (existingRequest is null)
                throw new NotFoundException ($"Request with id:{id} not found", "Request not found");
            if(existingRequest.StartDate > DateTime.Now)
                throw new ValidationException("Error validating transmitted data. FinishDate cannot be earlier than StartDate.", "Start date > Finish date");
            dbContext.Entry(existingRequest)
                        .CurrentValues
                        .SetValues(closeRequestDto.ToEntity(existingRequest));
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
        //A function that calls DELETE. Needed to delete a request by id.
        [HttpDelete("{id}/delete")]
        public async Task<ActionResult<Request>> DeleteRequestById(int id)
        {
            if(await dbContext.Requests.FindAsync(id) is null)
                throw new NotFoundException ($"Request with id:{id} not found", "Request not found");

            await dbContext.Requests.Where(request => request.Id == id).ExecuteDeleteAsync();
            return NoContent();
        }

    }
}
