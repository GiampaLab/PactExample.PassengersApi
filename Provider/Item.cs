using System.ComponentModel.DataAnnotations;

namespace Provider
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
    }
}