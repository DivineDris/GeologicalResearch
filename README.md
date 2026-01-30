# GeologicalResearch project
Completed by Shustov Savely

## Tools
- ASP.NET Core
- Entity Framework Core
- Swagger
- SQLite

## Requirements fulfilled
- Implementation of information system functionality for a company that provides geological services
    - Functionality for registering requests has been created
    - Functionality for assigning brigades to fulfill requests has been created
    - Functionality for confirming the completion of work by a brigade with notes and comments has been created
- Technical requirements
    - Exceptions are handled through custom middleware - ExceptionHandlerMiddleware
- EF was used as ORM in conjunction with SQLite

## Work report
### Data
Entity Framework + SQLite were used to work with the data. The context for EF is defined in the GRDataContext class.
There are three tables in the database:
- Request - requests
- Brigade - brigades
- Status - Request execution status
Since the main functionality of the API is related to creating requests, I decided to specify the values in the status and brigade tables statically in the context body.
The tables were created in the database using migrations.

### HTTP requests
To create functions for HTTP requests, I used two controllers:
- RequestsController - for requests related to applications
- ReportsController - for generating reports
DTOs were used to transfer only the necessary data in responses.
To convert DTOs to Request objects and vice versa, I wrote the RequestMapping extension.
Schemas of the DTOs used:
- CreateRequestDto - For transferring parameters for creating a request (in this case, a description of the work and the client)
<pre>  {
    "requestDescription": "string",
    "client": "string"
} </pre>
- AssignBrigadeDto - For assigning a brigade to a request
<pre>  {
    "brigadeId": 0
} </pre>
- RequestDetailsDto - A more technical representation that returns the properties of the request. Returns foreign keys for status and team.
<pre>  {
    "id": 1,
    "requestDescription": "Soil investigation of the site",
    "client": "John Doe",
    "brigadeId": 1,
    "statusId": 3,
    "startDate": "2025-04-01T20:36:34.128",
    "finishDate": "2025-04-02T20:36:34.128",
    "requestNote": null
} </pre>
- RequestSummaryDto - Returns request properties. For output to the UI, for example.
<pre>  {
    "id": 2,
    "requestDescription": "Lidar scanning",
    "client": "Soil Oy",
    "brigadeName": "Brigade #2",
    "statusName": "Request closed",
    "startDate": "2025-03-20T20:36:34.128",
    "finishDate": "2025-03-25T20:36:34.128",
    "requestNote": "Additional work is required."
} </pre>
- UpdateRequestDto - For transferring updated data. For example, if a secretary made a mistake and needs to correct some data in the application.
<pre>  {
    "requestDescription": "string",
    "client": "string",
    "brigadeId": 0,
    "statusId": 0,
    "startDate": "2025-04-13T00:15:24.092Z",
    "finishDate": "2025-04-13T00:15:24.092Z",
    "requestNote": "string"
} </pre>
- CloseRequestDto - For closing a request (marking it as completed). Only transmits notes on the request.
<pre>  {
  "requestNote": "string"
} </pre>
- BrigadeReportDto and RequestReportDto are required for transferring report data. BrigadeReportDto contains report data for the brigade and includes a list of RequestReportDto, which contains report data for each request.
  <pre>  {
    "brigadeId": 0,
    "brigadeName": "string",
    "requests": [
      {
        "requestId": 0,
        "requestDescription": "string",
        "timeSpent": 0
      }
    ],
    "amountOfFinishedRequests": 0
} </pre>
### Main functionality
#### Requests creation
Requests are created using the PostNewRequest function. CreateRequestDto is passed to the function parameter via a request. In the body of the function, the data received from CreateRequestDto is converted to a Request entity using the .ToEntity() function from the custom MappingRequest extension. The date and time of the request opening (StartDate) is set automatically at the time of the request execution.

#### Assigning a team to a request
The brigade is assigned to the request using the PutAssignBrigadeForRequest function. The request parameter of the function contains the request ID and AssignBrigadeDto containing the brigade ID. The function finds the request by ID in the database, changes the BrigadeId value in the request to the one specified in AssignBrigadeDto, and saves the changes to the database.

#### Confirmation of request completion (request closure)
The request is closed using the PutCloseRequestById function. The request parameter of the function contains the request ID and CloseRequestDto, which contains only notes that can be left after the request is completed. The function finds the request by id in the database and changes the value of statusId=3 (Request closed), FinishDate - the date and time of completion is set automatically during execution, the note data is transferred from CloseRequestDto and saves the changes to the database.

#### Monthly report on the work of the teams
The report is generated using the GetReport method in the ReportsController controller. The parameters specify the year and month for which the report is required. First, data on requests is retrieved from the database and filtered by completion status (must be status 3 “Request closed”) and by month and year, which must match the values specified in the attributes. A list is created from this data. Then the data in the list is grouped by teams. A BrigadeReВportDto is created for each group. All requests in the group are converted to a RequestReportDto list. BrigadeReВportDto is sorted by team id.

### Additional functionality
#### Retrieving requests using GET requests
Information on all created requests can be obtained using the GetAllRequests method. The method receives all requests, converts each Request into RequestSummaryDto using MappingRequest for more convenient data reading. Information on specific requests can be obtained via GetRequestById. The method receives a request by id and converts it using MappingRequest into RequestDetailsDto.

#### Updating request's data
The application data is updated using PutUpdateRequestById. Through the request, the method receives the id and UpdateRequestDto, which contains all the basic data for the Request, and then finds the corresponding application. If the data in the UpdateRequestDto fields is not equal to 0 or null, the values of the Request fields are changed to the corresponding values of UpdateRequestDto (except for requestNotes, which is assigned any value).

#### Deleting requests
Data is deleted from the database using the DeleteRequestById method, which is called by a DELETE request. The method receives the ID of the required request, searches for it in the database, and deletes it.

### Error handling
Errors are handled by custom middleware ExceptionHandlerMiddleware. This middleware can handle errors of the custom type GeologicalResearchAppException. GeologicalResearchAppException is a base type that contains additional HTTP code and a message for the user in its properties. Based on this, I created two child types:
- NotFoundException - for handling errors when the requested elements are missing;
- ValidationException - for handling validation errors.
All other errors in ExceptionHandlerMiddleware are marked as Internal server error.
ExceptionHandlerMiddleware handles errors using the HandleExсeptionAsync method - it logs errors and creates an HTTP response in the form of a structured JSON response with the appropriate status.

### Possible improvements
- Creating clients and storing clients as entities in the database;
- Ability to create and edit teams and statuses;
- Creating users and assigning roles to them (manager, secretary, foreman).

## Launch
- via Visual Studio or VS code - I am attaching the sln solution to the project.
- CLI - In the GeologicalResearch folder:
    - dotnet build
    - dotnet run
- After launch, Swagger will be available at http://localhost:5135/swagger/index.html
