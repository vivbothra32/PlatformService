using System.ComponentModel.DataAnnotations;
namespace PlatformService.DTOs
{
    public class PlatformCreateDto
    {
        public string? Name { get; set; }
        public string? Publisher { get; set; }
        public string? Cost { get; set; }
    }
}