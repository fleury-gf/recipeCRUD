namespace recipeCore.Models
{
    public class RecipeIngredient
    {
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        public decimal Quantity { get; set; }
        public Ingredient? Ingredient { get; set; }
    }
}