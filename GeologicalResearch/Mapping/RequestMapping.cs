using System;
using GeologicalResearch.Dto;
using GeologicalResearch.Models;

namespace GeologicalResearch.Mapping;

public static class RequestMapping
{
    public static Request ToEntity(this CreateRequestDto createRequestDto)
    {
            return new Request()
            {
                RequestDescription = createRequestDto.RequestDescription,
                StatusId = 1,
                BrigadeId = createRequestDto.BrigadeId,
                StartDate = DateTime.Now,
                FinishDate = null,
                RequestNote = null,
            };
    }

        public static Request ToEntity(this UpdateRequestDto updateRequestDto, int id)
    {
            return new Request()
            {
                Id = id,  
                RequestDescription = updateRequestDto.RequestDescription,
                StatusId = updateRequestDto.StatusId,
                BrigadeId = updateRequestDto.BrigadeId,
                StartDate = updateRequestDto.StartDate,
                FinishDate = updateRequestDto.FinishDate,
                RequestNote = updateRequestDto.RequestNote,
            };
    }

            public static Request ToEntity(this CloseRequestDto closeRequestDto, int id, Request request)
    {
            return new Request()
            {
                Id = id,
                RequestDescription = request.RequestDescription,
                StatusId = 3,
                BrigadeId = request.BrigadeId,
                StartDate = request.StartDate,
                FinishDate = DateTime.Now,
                RequestNote = closeRequestDto.RequestNote,
            };
    }

    public static RequestDetailsDto ToRequestDetailsDto(this Request request)
    {
        return new
        (
            request.Id,
            request.RequestDescription,
            request.BrigadeId,
            request.StatusId,
            request.StartDate,
            request.FinishDate,
            request.RequestNote
        );
    }

        public static RequestSummaryDto ToRequestSummaryDto(this Request request)
    {
        return new
        (
            request.Id,
            request.RequestDescription,
            request.Brigade!.BrigadeName,
            request.Status!.StatusName,
            request.StartDate,
            request.FinishDate,
            request.RequestNote
        );
    }

    public static RequestReportDto ToRequestReportDto(this Request request)
    {
        TimeSpan? duration = request.FinishDate - request.StartDate;
        return new
        (
            request.Id,
            request.RequestDescription,
            (int)duration!.Value.TotalHours  
        );
    }

}
