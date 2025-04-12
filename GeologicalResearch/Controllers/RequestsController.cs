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
        //Метод который вызывает POST запрос. Создает новую заявку
        [HttpPost("new")]
        public async Task<ActionResult<Request>> PostNewRequest(CreateRequestDto createRequestDto)
        {
            Request request = createRequestDto.ToEntity();
            await dbContext.Requests.AddAsync(request);
            await dbContext.SaveChangesAsync();
            return Ok(request.ToRequestDetailsDto());
        }
        //Метод который вызывает GET запрос. Возвращает все созданные заявки
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
        //Метод который вызывает GET запрос. Возвращает 1 заявку по указанному id
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequestById(int id)
        {
            Request? request = await dbContext.Requests.FindAsync(id);
            return request is null ? throw new NotFoundException ($"Заявка с id:{id} не найдена", "Request not found") : Ok(request.ToRequestDetailsDto());
        }
        //Метод который вызывает PUT запрос. Изменяет данные заявки. Нужен для того чтобы назначать бригаду на заявку
        [HttpPut("{id}/assignBrigade")]
        public async Task<ActionResult<Request>> PutAssignBrigadeForRequest(int id, AssignBrigadeDto assignBrigadeDto)
        {
            if(assignBrigadeDto.BrigadeId < 1 || assignBrigadeDto.BrigadeId > 3)
                throw new ValidationException($"Ошибка валидации передаваемых данных. BrigadeId не может быть = {assignBrigadeDto.BrigadeId}", "Invalid values");

             var existingRequest = await dbContext.Requests.FindAsync(id);
             if(existingRequest is null)
                throw new NotFoundException ($"Заявка с id:{id} не найдена", "Request not found");
            
            dbContext.Entry(existingRequest)
                        .CurrentValues
                        .SetValues(assignBrigadeDto.ToEntity(existingRequest));
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
        //Метод который вызывает PUT запрос. Изменяет данные заявки. Нужен для того чтобы назначать бригаду на заявку
        [HttpPut("{id}/update")]
        public async Task<ActionResult<Request>> PutUpdateRequestById(int id, UpdateRequestDto updateRequestDto)
        {
            if(updateRequestDto.BrigadeId > 3)
                throw new ValidationException($"Ошибка валидации передаваемых данных.\nBrigadeId не может быть = {updateRequestDto.BrigadeId}", "Invalid values");
            if(updateRequestDto.StatusId > 3)
                    throw new ValidationException($"Ошибка валидации передаваемых данных.\nStatusId не может быть = {updateRequestDto.StatusId}", "Invalid values");
            if(updateRequestDto.StartDate > updateRequestDto.FinishDate)
                throw new ValidationException("Ошибка валидации передаваемых данных.\nFinishDate не может быть раньше StartDate", "Invalid values");
            var existingRequest = await dbContext.Requests.FindAsync(id);
            
            if(existingRequest is null)
                throw new NotFoundException ($"Заявка с id:{id} не найдена", "Request not found");
            dbContext.Entry(existingRequest)
                        .CurrentValues
                        .SetValues(updateRequestDto.ToEntity(existingRequest));
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
        //Метод который вызывает PUT запрос. Изменяет данные заявки. Нужен для того чтобы закрыть заявку (То есть пометить заявку как выполненную)
        [HttpPut("{id}/close")]
        public async Task<ActionResult<Request>> PutCloseRequestById(int id, CloseRequestDto closeRequestDto)
        {
            var existingRequest = await dbContext.Requests.FindAsync(id);
            if (existingRequest is null)
                throw new NotFoundException ($"Заявка с id:{id} не найдена", "Request not found");
            if(existingRequest.StartDate > DateTime.Now)
                throw new ValidationException("Ошибка валидации передаваемых данных.\nFinishDate не может быть раньше StartDate", "Start date > Finish date");
            dbContext.Entry(existingRequest)
                        .CurrentValues
                        .SetValues(closeRequestDto.ToEntity(existingRequest));
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
        //Метод который вызывает DELETE. Нужен для удаления заявки по id
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
