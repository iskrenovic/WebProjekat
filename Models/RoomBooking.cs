using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models
{
    public class RoomBooking
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        public DateTime ArrivalDate { get; set; }
        [Required]
        public DateTime DepartureDate { get; set; }
        [Required]
        [JsonIgnore]
        public virtual Room Room { get; set; }
        [Required]
        [JsonIgnore]
        public virtual Client Client{ get; set; }
    }
}