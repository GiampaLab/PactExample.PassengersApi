using System.ComponentModel.DataAnnotations;

namespace PassengersApi
{
    public class Passenger
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}