namespace Pim.Model.Dtos
{
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public long PhoneNumber { get; set; }
        public string NewPassword { get; set; }
    }
}
