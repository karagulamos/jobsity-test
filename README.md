# Jobsity Test

- [Tasks](/docs/TASKS.md)
- [Preview](/docs/Preview.md)
- [High-Level Architecture](/docs/HLA.md)

## Setup

Requires Docker, RabbitMQ, .NET 7 or higher.

```bash
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.11-management
```

```bash
$dotnet restore
$dotnet watch run --project Jobsity.Chat.Web  
```

## Summary

| Command                       | Description                     |
| :---------------------------- | :------------------------------ |
| `dotnet build`                | To build the solution           |
| `dotnet watch run`            | To run the solution in dev      |

## Packages

| Name                  | Description                       |
| :-------------------- | :-------------------------------- |
| `SignalR`             | For realtime comm between clients |
| `EF Core`             | Persistence and data retrieval    |
| `EasyNetQ`            | For message queueing              |
| `Moq`                 | For mocking in tests              |
