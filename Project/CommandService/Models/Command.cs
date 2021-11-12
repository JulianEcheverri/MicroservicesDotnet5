using System.ComponentModel.DataAnnotations;

namespace CommandService.Models
{
    public class Command
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string HowTo { get; set; }

        [Required]
        public string CommnadLine { get; set; }

        public int PlatformId { get; set; }
        public Platform Platform { get; set; }
    }
}