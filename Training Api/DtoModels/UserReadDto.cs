namespace Training_Api.DtoModels
{
    public class UserReadDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public Role.Role Role { get; set; }
    }
}
