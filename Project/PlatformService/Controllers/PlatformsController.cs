using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepository platformRepository, IMapper mapper, ICommandDataClient commandDataClient, IMessageBusClient messageBusClient)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<PlatformReadDto>> Get()
        {
            var platforms = _platformRepository.GetAll();
            if (platforms.Any())
                return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));

            return NotFound();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PlatformReadDto> Get(int id)
        {
            var platform = _platformRepository.GetById(id);
            if (platform != null)
                return Ok(_mapper.Map<PlatformReadDto>(platform));

            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PlatformReadDto>>> Post(PlatformCreateDto platformCreateDto)
        {
            try
            {
                var platform = _mapper.Map<Platform>(platformCreateDto);
                _platformRepository.Create(platform);
                _platformRepository.SaveChanges();

                var platformReadDto = _mapper.Map<PlatformReadDto>(platform);

                // Sending Synch Message of the Platform created to command service through HTTP Command data client
                await _commandDataClient.SendPlatformToCommand(platformReadDto);

                // Sending Asynch Message of the Platform created to RabbitMQ MessageBus
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = "Platform_Published";

                _messageBusClient.PublishNewPlatform(platformPublishedDto);

                return CreatedAtAction(nameof(Post), new { Id = platformReadDto.Id, platform = platformReadDto });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Something went wrong... ex: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
