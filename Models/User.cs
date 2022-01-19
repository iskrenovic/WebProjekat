
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models
{
    public class User
    {
        [Key]
        public int ID{get; set;}

        [MinLength(4)]
        [MaxLength(16)]
        public string Username { get; set; }

        [MinLength(4)]
        [MaxLength(16)]
        public string Password { get; set; }

        [MinLength(3)]
        [MaxLength(20)]
        public string Ime { get; set; }

        [MinLength(3)]
        [MaxLength(20)]
        public string Prezime { get; set; }

        public UserType UserType { get; set; }
        [JsonIgnore]
        public virtual List<Employed> WorksAt { get; set; }
    }
}