using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models
{
    public class Room
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Range(1,20000)]
        public int RoomNumber { get; set; }
        [Required]
        [Range(1,20)]
        public int Capacity { get; set; }
        [Required]
        public int RoomType { get; set; }
        public bool Open { get; set; }
        [JsonIgnore]
        public virtual Hotel Hotel { get; set; }

        public virtual List<RoomBooking> RoomBookings { get; set; }
    }
}