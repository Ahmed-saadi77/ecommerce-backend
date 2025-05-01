namespace E_comrece.DTO
{
    public class ColorDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StoreId { get; set; }
        public string? Value { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
