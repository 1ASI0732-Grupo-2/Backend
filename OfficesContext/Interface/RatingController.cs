using Microsoft.AspNetCore.Mvc;
using workstation_backend.OfficesContext.Application.CommandServices;
using workstation_backend.OfficesContext.Domain.Models.Commands;
using workstation_backend.OfficesContext.Domain;
using System;
using System.Threading.Tasks;

namespace workstation_backend.OfficesContext.Interface
{
    /// <summary>
    /// Controlador para la gestión de calificaciones y valoraciones de oficinas
    /// </summary>
    [ApiController]
    [Route("api/workstation/[controller]")]
    [Produces("application/json")]
    public class RatingController : ControllerBase
    {
        private readonly RatingCommandService _ratingCommandService;
        private readonly IRatingRepository _ratingRepository;

        public RatingController(
            RatingCommandService ratingCommandService,
            IRatingRepository ratingRepository)
        {
            _ratingCommandService = ratingCommandService ?? throw new ArgumentNullException(nameof(ratingCommandService));
            _ratingRepository = ratingRepository ?? throw new ArgumentNullException(nameof(ratingRepository));
        }

        /// <summary>
        /// Crea una nueva calificación para una oficina
        /// </summary>
        /// <param name="command">Datos de la calificación a crear</param>
        /// <returns>Calificación creada</returns>
        /// <response code="200">Calificación creada exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        /// <response code="404">Oficina no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateRatingAsync([FromBody] CreateRatingCommand command)
        {
            if (command == null)
                return BadRequest("Invalid rating data.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _ratingCommandService.CreateRatingAsync(command);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Obtiene todas las calificaciones de una oficina específica
        /// </summary>
        /// <param name="officeId">ID único de la oficina</param>
        /// <returns>Lista de calificaciones de la oficina</returns>
        /// <response code="200">Retorna las calificaciones de la oficina</response>
        /// <response code="400">ID de oficina inválido</response>
        /// <response code="404">Oficina no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("office/{officeId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRatingsByOfficeAsync([FromRoute] Guid officeId)
        {
            if (officeId == Guid.Empty)
                return BadRequest("Invalid office ID.");

            try
            {
                var ratings = await _ratingRepository.GetByOfficeIdAsync(officeId);
                
                if (ratings == null)
                    return NotFound($"Office with ID {officeId} not found.");

                return Ok(ratings);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Calcula y obtiene el promedio de calificaciones de una oficina
        /// </summary>
        /// <param name="officeId">ID único de la oficina</param>
        /// <returns>Promedio de calificaciones redondeado a 2 decimales</returns>
        /// <response code="200">Retorna el promedio de calificaciones (0 si no hay calificaciones)</response>
        /// <response code="400">ID de oficina inválido</response>
        /// <response code="404">Oficina no encontrada</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("office/{officeId:guid}/average")]
        [ProducesResponseType(typeof(double), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAverageRatingByOfficeAsync([FromRoute] Guid officeId)
        {
            if (officeId == Guid.Empty)
                return BadRequest("Invalid office ID.");

            try
            {
                var ratings = await _ratingRepository.GetByOfficeIdAsync(officeId);

                if (ratings == null)
                    return NotFound($"Office with ID {officeId} not found.");

                if (!ratings.Any())
                    return Ok(0.0);

                var average = ratings.Average(r => r.Score);
                return Ok(Math.Round(average, 2));
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}