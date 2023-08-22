using System.ComponentModel.DataAnnotations;

namespace BackEnd.Models
{
    public class Book
    {
      
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string PublishedDate { get; set; }
        public bool IsAvailable { get; set; }
    }
}
