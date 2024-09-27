namespace FirstMVCtest.Models
{
    public class CategoryItemsViewModel
    {
        public Category Category { get; set; } = null!;
        public List<Item> Items { get; set; } = new List<Item>();
    }

}
