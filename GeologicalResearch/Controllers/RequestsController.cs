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
        public ActionResult<Request> PostNewRequest(CreateRequestDto createRequestDto)
        {
            Request request = createRequestDto.ToEntity();
            dbContext.Requests.Add(request);
            dbContext.SaveChanges();
            return Ok(request.ToRequestDetailsDto());
        }

        [HttpGet]
        public ActionResult<Request> GetAllRequests()
        {
            var requests = dbContext.Requests
            .Include(request=>request.Brigade)
            .Include(request=>request.Status)
            .Select(request => request.ToRequestSummaryDto())
            .AsNoTracking().ToList();

            return requests.Count() == 0 ? throw new NotFoundException("Заявки не найдены",  "Requests not found") : Ok(requests);
        }

        [HttpGet("{id}")]
        public ActionResult<Request> GetRequestById(int id)
        {
            Request? request = dbContext.Requests.Find(id);
            return request is null ? throw new NotFoundException ($"Заявка с id:{id} не найдена", "Request not found") : Ok(request.ToRequestDetailsDto());
        }

        [HttpPut("{id}/assignBrigade")]
        public ActionResult<Request> PutAssignBrigadeForRequest(int id, AssignBrigadeDto assignBrigadeDto)
        {
            if(assignBrigadeDto.BrigadeId < 1 || assignBrigadeDto.BrigadeId > dbContext.Brigades.Count())
                throw new ValidationException("Ошибка валидации передаваемых данных", "Invalid values");

             var existingRequest = dbContext.Requests.Find(id);
             if(existingRequest is null)
                throw new NotFoundException ($"Заявка с id:{id} не найдена", "Request not found");
            
            dbContext.Entry(existingRequest)
                        .CurrentValues
                        .SetValues(assignBrigadeDto.ToEntity(existingRequest));
            dbContext.SaveChanges();
            return NoContent();
        }
        [HttpPut("update/{id}")]
        public ActionResult<Request> PutUpdateRequestById(int id, UpdateRequestDto updateRequestDto)
        {
            if((updateRequestDto.BrigadeId < 1 || updateRequestDto.BrigadeId > dbContext.Brigades.Count()) ||
            (updateRequestDto.StatusId < 1 || updateRequestDto.StatusId > 3) ||
            updateRequestDto.StartDate > updateRequestDto.FinishDate)
                throw new ValidationException("Ошибка валидации передаваемых данных", "Invalid values");
            var existingRequest = dbContext.Requests.Find(id);
            
            if(existingRequest is null)
                throw new NotFoundException ($"Заявка с id:{id} не найдена", "Request not found");
            dbContext.Entry(existingRequest)
                        .CurrentValues
                        .SetValues(updateRequestDto.ToEntity(existingRequest));
            dbContext.SaveChanges();
            return NoContent();
        }
        [HttpPut("close/{id}")]
        public ActionResult<Request> PutCloseRequestById(int id, CloseRequestDto closeRequestDto)
        {
            var existingRequest = dbContext.Requests.Find(id);
            if (existingRequest is null)
                throw new NotFoundException ($"Заявка с id:{id} не найдена", "Request not found");
            if(existingRequest.StartDate > DateTime.Now)
                throw new ValidationException("Дата открытия заявки больше даты закрытия заявки", "Start date > Finish date");
            dbContext.Entry(existingRequest)
                        .CurrentValues
                        .SetValues(closeRequestDto.ToEntity(existingRequest));
            dbContext.SaveChanges();
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public ActionResult<Request> DeleteRequestById(int id)
        {
            if(dbContext.Requests.Find(id) is null)
                throw new NotFoundException ($"Заявка с id:{id} не найдена", "Request not found");

            dbContext.Requests.Where(request => request.Id == id).ExecuteDelete();
            return NoContent();
        }

    }
}
