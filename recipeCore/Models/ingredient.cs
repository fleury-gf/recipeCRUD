namespace recipeCore.Models
{   
    public class Ingredient
    {
        public int Id {get; set; }
        public string Name {get; set; } = string.Empty;
        public string UnitOfMeasure {get; set; } = string.Empty;
        public DateTime CreatedAt {get; set; }
        public DateTime UpdatedAt {get; set; }
    }


}
