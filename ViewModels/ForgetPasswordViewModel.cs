using System.ComponentModel.DataAnnotations;

namespace RouteG01.Pl.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage ="Please Email Is Required!!")]
        public string Email { get; set; } = null!;
    }
}
