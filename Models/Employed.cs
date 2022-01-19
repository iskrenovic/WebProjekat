using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Employed
    {
        [Key]
        public int ID { get; set; }         
        public virtual Hotel Hotel { get; set; }
        public virtual User User { get; set; }
        
    }
}