using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SuncDataServices.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IMapper _mapper;
        public readonly IPlatformRepo _repository;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(IPlatformRepo repository, IMapper mapper, ICommandDataClient commandDataClient)
        {
            _mapper = mapper;
            _repository = repository;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetAllPlatforms()
        {
            Console.WriteLine("--> Getting platforms");

            var platforms = _repository.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platform = _repository.GetPlatformById(id);
            if (platform == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto dto)
        {
            var platformModel = _mapper.Map<Platform>(dto);
            _repository.CreatePlatform(platformModel);
            _repository.SaveChanges();

            var createdDto = _mapper.Map<PlatformReadDto>(platformModel);

            try
            {       
               await _commandDataClient.SendPlatformToCommand(createdDto);
            } catch(Exception ex)
            {
                Console.WriteLine($"--> Could not send syncronously: {ex.Message}");
            }
            

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = createdDto.Id }, createdDto);
        }

    }
}