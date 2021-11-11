using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SynchDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(IPlatformRepository platformRepository, IMapper mapper, ICommandDataClient commandDataClient)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
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

                // Sending the platform created to command service through command data client
                await _commandDataClient.SendPlatformToCommand(platformReadDto);

                return CreatedAtAction(nameof(Post), new { Id = platformReadDto.Id, platform = platformReadDto });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
