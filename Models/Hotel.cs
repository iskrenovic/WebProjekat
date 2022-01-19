using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models
{
    public class Hotel
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Address { get; set; }

        [MaxLength(50)]
        public string City { get; set; }
        [Range(0f,10f)]
        public double Rating{ get; set; }

        public virtual List<Room> Rooms { get; set; }
        [JsonIgnore]
        public virtual List<Employed> Employees { get; set; }
    }
}