using System.ComponentModel.DataAnnotations;

namespace Drinkbox.Web.ViewModels
{
    public class ImportExcelViewModel
    {
        [Required]
        [Display(Name = "Excel файл")]
        public IFormFile ExcelFile { get; set; }
    }
}
