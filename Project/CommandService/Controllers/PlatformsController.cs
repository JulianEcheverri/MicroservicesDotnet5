using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepository _commandRepository;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepository commandRepository, IMapper mapper)
        {
            _commandRepository = commandRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<PlatformReadDto>> Get()
        {
            Console.WriteLine("--> Getting Platforms from CommandService");
            try
            {
                var platforms = _commandRepository.GetAllPlatforms();
                return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
            }
            catch (Exception ex)
            {
                Console.WriteLine("--> Something went wrong retrieving the Platforms from CommandService", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Post()
        {
            Console.WriteLine("--> Inbound POST # Command Service");

            return CreatedAtAction(nameof(Post), new { Message = "Inbound test from Platforms Controller (command service)" });
        }
    }
}