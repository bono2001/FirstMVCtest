namespace FirstMVCtest.Models
{
    public class ItemEditViewModel
    {
        public Item Item { get; set; }
        public List<Category>? Categories { get; set; }
        public List<int> SelectedCategoryIds { get; set; } // Geselecteerde categorieën
    }

}
