# Тестовое задание GeologicalResearch
Выполнил Шустов Савелий

## Стек
- ASP.NET Core
- Entity Framework Core
- Swagger
- SQLite

## Выполненные требования
- Реализация функционала информационной системы для компании, которая оказывает геологические услуги
    - Создан функционал для регистрации заявок
    - Создан функционал для назначения бригад на выполнение заявок
    - Создан функционал подтверждения выполнение работ бригадой с указанием примечаний и пометок
- Технические требования
    - Обработка исключений происходит через кастомный middleware - ExceptionHandlerMiddleware
    - В качестве ORM использован использовался EF в связке с SQLite

## Отчет о работе
### Работа с данными
Для работы с данными были использованы Entity Framework + SQLite. Контекст для EF задан в классе GRDataContext. 
В базе данных 3 таблицы:
- Request - заявки
- Brigade - бригады
- Status - Статус выполнения заявки
Так как основной функционал Api связан с созданием заявок то я решил указать значения в таблицу статусов и бригад статично в теле контекста.
Таблицы созданы в БД с помощью миграций.

### Работа с http запросами
Для создания методов для http запросов я использовал 2 контроллера
- RequestsController - для запросов связанных с заявками
- ReportsController - для составления отчета
Для передачи только нужных данных в ответах использованы Dto.
Для перевода Dto в объекты Request и наоборот я написал расширение RequestMapping.
Схемы используемых Dto:
- CreateRequestDto - Для передачи параметров для создания заявки (в данном случае описание работ и клиент)
<pre> ```json {
    "requestDescription": "string",
    "client": "string"
} ```</pre>
- AssignBrigadeDto - Для назначения бригады на заявку
<pre> ```json {
    "brigadeId": 0
} ```</pre>
- RequestDetailsDto - Более техническое представление, которое возвращает свойства заявки. Возвращаются внешние ключи для статуса и бригады.
<pre> ```json {
    "id": 1,
    "requestDescription": "Исследование грунта участка",
    "client": "Иванов И.И.",
    "brigadeId": 1,
    "statusId": 3,
    "startDate": "2025-04-01T20:36:34.128",
    "finishDate": "2025-04-02T20:36:34.128",
    "requestNote": null
} ```</pre>
- RequestSummaryDto - Возвращает свойства заявок. Для вывода в UI например.
<pre> ```json {
    "id": 2,
    "requestDescription": "Лидарная съемка",
    "client": "ИП Плюшкин В.П.",
    "brigadeName": "Бригада №2",
    "statusName": "Заявка закрыта",
    "startDate": "2025-03-20T20:36:34.128",
    "finishDate": "2025-03-25T20:36:34.128",
    "requestNote": "Требуется проведение доп. работ"
} ```</pre>
- UpdateRequestDto - Для передачи обновленных данных. Например если секретарь ошибся и ему нужно исправить некоторые данные в заявке.
<pre> ```json {
    "requestDescription": "string",
    "client": "string",
    "brigadeId": 0,
    "statusId": 0,
    "startDate": "2025-04-13T00:15:24.092Z",
    "finishDate": "2025-04-13T00:15:24.092Z",
    "requestNote": "string"
} ```</pre>
- CloseRequestDto - Для закрытия заявки (отметке о выполнении). Передает только заметки по заявке
<pre> ```json {
  "requestNote": "string"
} ```</pre>
- BrigadeReportDto и RequestReportDto - нужны для передачи отчетных данных. BrigadeReportDto - отчетные данные по бригаде содержит список RequestReportDto в котором находятся отчетные данные по каждой заявке
  <pre> ```json {
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
} ```</pre>
### Основной функционал
#### Создание заявок
Заявки создаются с помощью метода PostNewRequest. Через запрос в параметр метода передается CreateRequestDto. В теле метода полученные данные из CreateRequestDto конвертируются в сущность Request c помощью метода .ToEntity() из кастомного расширения MappingRequest. Дата и время открытия заявки (StartDate) ставится автоматически на момент выполнения запроса.

#### Назначение бригады на заявку
Бригада назначается на заявку с помощью метода PutAssignBrigadeForRequest. Через запрос в параметр метода передается id заявки и AssignBrigadeDto содержащий id бригады. Метод находит по id заявку в базе данных и меняет значение BrigadeId в заявке на указанное в AssignBrigadeDto и сохраняет изменения в БД.

#### Подтверждение выполнения заявки (закрытие заявки)
Заявка закрывается с помощью метода PutCloseRequestById. Через запрос в параметр метода передается id заявки и CloseRequestDto, который содержит только заметки которые можно оставить после выполнения заявки. Метод находит по id заявку в базе данных и меняет значение statusId=3 (Заявка закрыта), FinishDate - дату и время завершения ставит автоматически во время выполнения, данные о заметках передаются из CloseRequestDto и сохраняет изменения в БД.

#### Отчет по работе бригад за месяц
Отчет создается с помощью метода GetReport в контроллере ReportsController. В параметрах указаны год и месяц за который необходимо получить отчет. Сначала из базы данных берутся данные о заявках и фильтруются по статусу выполнения (должен быть статус 3 "Заявка закрыта") и по месяцу и году которые должны соответствовать указанным в атрибутах значениям. Из этих данных создается список. Затем данные в списке группируются по бригадам. Для каждой группы создается BrigadeReВportDto. Все заявки в группе преобразуются в список RequestReportDto BrigadeReВportDto сортируется по id бригады.

### Дополнительный функционал
#### Вывод заявок с помощью GET запросов
Информацию по всем созданным заявкам можно получить с помощью метода GetAllRequests. Метод получает все заявки, конвертирует каждый Request в RequestSummaryDto с помощью MappingRequest для более удобного прочтения данных. Информацию по конкретным заявкам можно получить через GetRequestById. Метод получает заявку по id и конвертирует с помощью MappingRequest в RequestDetailsDto.

#### Обновление данных о заявках
Обновление данных заявки происходит с помощью PutUpdateRequestById. Через запрос метод получает id и UpdateRequestDto который содержит все основные данные для Request, затем находит соответствующую заявку. Если данные в полях UpdateRequestDto не равну 0 или null то значения полей Request меняются на соответствующие значения UpdateRequestDto (кроме requestNotes, в это поле присваивается любое значение).

#### Удаление заявок
Удаление данных из БД происходит с помощью метода DeleteRequestById который вызывается запросом DELETE. Метод получает id нужной заявки, ищет в БД нужную заявку и удаляет ее.

### Обработка ошибок
Обработка ошибок происходит через кастомный middleware ExceptionHandlerMiddleware. Данный middleware может обрабатывать ошибки кастомного типа GeologicalResearchAppException. GeologicalResearchAppException это базовый тип который содержит в свойствах дополнительно http код и сообщение для пользователя. На основе которого я создал два дочерних типа:
- NotFoundException - для обработки ошибок отсутствия искомых элементов;
- ValidationException - для обработки ошибок валидации.
Все остальные ошибки в ExceptionHandlerMiddleware помечаются как Internal server error.
ExceptionHandlerMiddleware обрабатывает ошибки с помощью метода HandleExсeptionAsync - логирует ошибки и создает http ответ в виде структурированного JSON-ответа с соответствующим статусом.

### Возможные улучшения
- Создание клентов и хранение клиентов как сущности в БД;
- Возможность создания и редактирования бригад и статусы;
- Создание пользователей и назначение им ролей (начальник, секретарь, бригадир).

## Запуск
- через Visual Studio или VS code - sln решение прилагаю к проекту.
- CLI - В папке GeologicalResearch:
    - dotnet build
    - dotnet run
- После запуска Swagger будет доступен по адресу http://localhost:5135/swagger/index.html