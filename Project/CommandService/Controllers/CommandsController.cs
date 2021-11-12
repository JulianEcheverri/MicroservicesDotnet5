using System;
using System.Collections.Generic;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository _commandRepository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandRepository commandRepository, IMapper mapper)
        {
            _commandRepository = commandRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<CommandReadDto>> Get(int platformId)
        {
            Console.WriteLine("--> Getting Commands from CommandService");
            try
            {
                if (!_commandRepository.PlatformExists(platformId))
                    return NotFound();

                var commands = _commandRepository.GetCommandsByPlatformId(platformId);
                return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
            }
            catch (Exception ex)
            {
                Console.WriteLine("--> Something went wrong retrieving the Commands from CommandService", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{commandId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<CommandReadDto> Get(int platformId, int commandId)
        {
            Console.WriteLine($"--> Getting Command of Platform from CommandService: {nameof(platformId)}: {platformId}, {nameof(commandId)}: {commandId}");
            try
            {
                if (!_commandRepository.PlatformExists(platformId))
                    return NotFound();

                var command = _commandRepository.GetCommand(platformId, commandId);
                return Ok(_mapper.Map<CommandReadDto>(command));
            }
            catch (Exception ex)
            {
                Console.WriteLine("--> Something went wrong retrieving the Command of Platform from CommandService", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<CommandReadDto> Post(int platformId, CommandCreateDto commandCreateDto)
        {
            Console.WriteLine($"--> Post Command for Platform from CommandService");
            try
            {
                if (!_commandRepository.PlatformExists(platformId))
                    return NotFound();

                var command = _mapper.Map<Command>(commandCreateDto);
                _commandRepository.CreateCommand(platformId, command);
                _commandRepository.SaveChanges();

                var commandReadDto = _mapper.Map<CommandReadDto>(command);

                return CreatedAtAction(nameof(Post), new { platformId, commandId = commandReadDto.Id, commandReadDto });
            }
            catch (Exception ex)
            {
                Console.WriteLine("--> Something went wrong retrieving the Command of Platform from CommandService", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}