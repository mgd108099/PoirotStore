using System.ComponentModel.DataAnnotations;

namespace CrimeGameStore.Web.Models
{
    public class CaseGallery
    {
        public int Id { get; set; }

        public int CrimeCaseId { get; set; }
        public CrimeCase? CrimeCase { get; set; }

        [Required, MaxLength(250)]
        [Display(Name = "مسار الصورة")]
        public string ImageUrl { get; set; } = string.Empty;

        [MaxLength(150)]
        [Display(Name = "وصف الصورة (Alt للسيو)")]
        public string Caption { get; set; } = string.Empty;

        public int SortOrder { get; set; } = 1;
    }
}