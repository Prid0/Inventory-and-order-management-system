namespace Pim.Data.Models
{
    public class UserRoleMapping : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

    }
}
