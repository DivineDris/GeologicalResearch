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
                BrigadeId = null,
                StartDate = DateTime.Now,
                FinishDate = null,
                RequestNote = null,
            };
    }

    public static Request ToEntity(this UpdateRequestDto updateRequestDto, Request request)
    {
        if(updateRequestDto.RequestDescription != null)
            request.RequestDescription = updateRequestDto.RequestDescription;
        if(updateRequestDto.BrigadeId != null)
            request.BrigadeId = updateRequestDto.BrigadeId;
        if(updateRequestDto.StatusId != null)
            request.StatusId = (int)updateRequestDto.StatusId;
        if(updateRequestDto.StartDate != null)
            request.StartDate = (DateTime)updateRequestDto.StartDate;
        if(updateRequestDto.FinishDate != null)
            request.FinishDate = updateRequestDto.FinishDate;
        if(updateRequestDto.RequestNote != null)
            request.RequestNote = updateRequestDto.RequestNote;
        
        return request;
    }

    public static Request ToEntity(this CloseRequestDto closeRequestDto, Request request)
    {
        request.StatusId = 3;
        request.FinishDate = DateTime.Now;
        request.RequestNote = closeRequestDto.RequestNote;
        return request;
    }

            public static Request ToEntity(this AssignBrigadeDto assignBrigadeDto, Request request)
    {
        request.BrigadeId = assignBrigadeDto.BrigadeId;
        return request;
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
        string BrigadeName = string.Empty;
        if (request.BrigadeId != null)
            BrigadeName = request.Brigade!.BrigadeName;
        return new
        (
            request.Id,
            request.RequestDescription,
            BrigadeName,
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
