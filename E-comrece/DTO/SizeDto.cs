namespace E_comerce.DTO
{
    public class SizeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StoreId { get; set; }
        public string? Value { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


    }
}
