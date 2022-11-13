using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Dtos;
using PlatformService.SyncDataServices.Http;
using PLatformService.Data;
using PLatformService.Models;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(
        IPlatformRepo repository,
        IMapper mapper,
        ICommandDataClient commandDataClient
        )
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }
        [HttpGet]
        public ActionResult<IEnumerable<PlatformCreateDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms....");

            var platformItems = _repository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformCreateDto> GetPlatformById(int id)
        {
            Console.WriteLine("--> Getting Platforms....");

            var platformItem = _repository.getPlatformById(id);

            if (platformItem != null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreateNewPlatform(PlatformCreateDto platformCreateDto)
        {
            Console.WriteLine("--> Creating Platforms....");

            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(platformModel);
            _repository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
                throw;
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);

        }

    }
}