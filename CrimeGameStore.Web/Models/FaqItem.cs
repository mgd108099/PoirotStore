using System.ComponentModel.DataAnnotations;

namespace CrimeGameStore.Web.Models
{
    public class FaqItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "السؤال مطلوب"), MaxLength(250)]
        [Display(Name = "السؤال")]
        public string Question { get; set; } = string.Empty;

        [Required(ErrorMessage = "الإجابة مطلوبة"), MaxLength(1500)]
        [Display(Name = "الإجابة")]
        public string Answer { get; set; } = string.Empty;

        [Display(Name = "الترتيب")]
        public int SortOrder { get; set; } = 1;

        [Display(Name = "مفعّل")]
        public bool IsActive { get; set; } = true;
    }
}