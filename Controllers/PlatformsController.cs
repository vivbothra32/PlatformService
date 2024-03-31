using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private IPlatformRepo _repository;
        private IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(IPlatformRepo repository,
         IMapper mapper,
         ICommandDataClient commandDataClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms(){
            Console.WriteLine("Getting platforms...");
            var platformItems = _repository.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id){
            Console.WriteLine($"Getting platform with Id {id}", id);
            var platform = _repository.GetPlatformById(id);
            if(platform != null){
                return Ok(_mapper.Map<PlatformReadDto>(platform));
            }
            return NotFound();
        }

        [HttpPost("create")]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto createDto){
            var platformModel = _mapper.Map<Platform>(createDto);
            _repository.CreatePlatform(platformModel);
            _repository.SaveChanges();
            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            try{
                Console.WriteLine($"Calling Commands Service...");
                await _commandDataClient.SendDatatoCommandService(platformReadDto);
            }
            catch(Exception ex){
                if(ex.Message == "Success"){
                    Console.WriteLine($"Command Service call success: {ex.Message}");
                    return CreatedAtRoute(nameof(GetPlatformById), new {Id = platformReadDto.Id}, platformReadDto);       
                }
                else{
                    Console.WriteLine($"Command Service call failed: {ex.Message}");
                    return null;
                }            
            }
            return null;
        }
    }    
}