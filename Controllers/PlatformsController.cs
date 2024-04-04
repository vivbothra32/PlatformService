using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
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
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repository,
         IMapper mapper,
         ICommandDataClient commandDataClient,
         IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
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

            //Send Sync Message
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

            //Send Async Message
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                platformPublishedDto.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(platformPublishedDto);
            }
            catch(Exception ex){
                Console.WriteLine("--> Could not send message asynchronously!");
            }
            return null;
        }
    }    
}