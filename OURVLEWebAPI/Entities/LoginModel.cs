using System.ComponentModel.DataAnnotations;

namespace OURVLEWebAPI.Entities
{
    public class LoginModel
    {
        [Required(ErrorMessage = "UserId is required")]
        public int? UserId { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
