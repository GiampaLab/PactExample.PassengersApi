using System.ComponentModel.DataAnnotations;

namespace Provider
{
    public class Passenger
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}