using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WoodyMovie.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }
		[Display(Name ="Size")]
        public decimal MovieSize { get; set; }
        public string Category { get; set; }
        [Display(Name = "Released on")]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Downloads")]
        public long DownloadCount { get; set; }

    }
}