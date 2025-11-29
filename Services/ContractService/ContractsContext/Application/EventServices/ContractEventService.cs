using System;
using System.Text.Json;
using workstation_backend.ContractsContext.Domain.Services;

namespace workstation_backend.ContractsContext.Application.EventServices;

/// <summary>
/// Servicio responsable de publicar eventos de dominio relacionados a contratos.
/// Se utiliza para registrar y notificar acciones importantes dentro del ciclo de vida
/// de los contratos, firmas, compensaciones y recibos.
/// </summary>
public class ContractEventService : IContractEventService
{
    private readonly ILogger<ContractEventService> _logger;

    /// <summary>
    /// Inicializa una nueva instancia del <see cref="ContractEventService"/>.
    /// </summary>
    /// <param name="logger">Instancia del logger para registrar los eventos publicados.</param>
    public ContractEventService(ILogger<ContractEventService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Publica un evento de dominio de forma asíncrona.
    /// </summary>
    /// <typeparam name="TEvent">Tipo del evento que se desea publicar.</typeparam>
    /// <param name="event">Instancia del evento que será publicado.</param>
    /// <returns>Una tarea que representa la operación asincrónica.</returns>
    /// <remarks>
    /// Este método serializa el evento a formato JSON y lo registra mediante el logger.
    /// En esta implementación, la publicación es local (solo registro), pero puede
    /// extenderse a un bus de eventos externo como RabbitMQ, Kafka o EventGrid.
    /// </remarks>
    public Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
    {
        var eventName = typeof(TEvent).Name;
        var eventData = JsonSerializer.Serialize(@event);

        _logger.LogInformation("[EVENT PUBLISHED] {EventName}: {EventData}", eventName, eventData);

        return Task.CompletedTask;
    }
}
