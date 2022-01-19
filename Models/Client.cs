using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Client
    {
        [Key]
        public int ID { get; set; }

        [MinLength(3)]
        [MaxLength(20)]
        public string Ime { get; set; }

        [MinLength(3)]
        [MaxLength(20)]
        public string Prezime { get; set; }

        public DocumentType DocumentType{ get; set; }
        [MaxLength(13)]
        public string DocumentIdNumber { get; set; }
        
        [Phone]
        public string PhoneNumber { get; set; }

        public virtual List<RoomBooking> RoomBookings { get; set; }
    }
}