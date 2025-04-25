using recipeCore.Interfaces.Repositories;
using recipeCore.Interfaces.Services;
using recipeCore.Models;

namespace recipeInfra.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _ingredientRepository;

        public IngredientService(IIngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync()
        {
            return await _ingredientRepository.GetAllAsync();
        }

        public async Task<Ingredient?> GetIngredientByIdAsync(int id)
        {
            return await _ingredientRepository.GetByIdAsync(id);
        }

        public async Task<int> CreateIngredientAsync(Ingredient ingredient)
        {
            // Add any business logic validation here
            return await _ingredientRepository.CreateAsync(ingredient);
        }

        public async Task UpdateIngredientAsync(Ingredient ingredient)
        {
            // Add any business logic validation here
            await _ingredientRepository.UpdateAsync(ingredient);
        }

        public async Task DeleteIngredientAsync(int id)
        {
            // Add any business logic validation here
            await _ingredientRepository.DeleteAsync(id);
        }
    }

    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;

        public RecipeService(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        {
            return await _recipeRepository.GetAllAsync();
        }

        public async Task<Recipe?> GetRecipeByIdAsync(int id)
        {
            return await _recipeRepository.GetByIdAsync(id);
        }

        public async Task<Recipe?> GetRecipeWithIngredientsAsync(int id)
        {
            return await _recipeRepository.GetRecipeWithIngredientsAsync(id);
        }

        public async Task<int> CreateRecipeAsync(Recipe recipe)
        {
            // Add any business logic validation here
            return await _recipeRepository.CreateAsync(recipe);
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            // Add any business logic validation here
            await _recipeRepository.UpdateAsync(recipe);
        }

        public async Task DeleteRecipeAsync(int id)
        {
            // Add any business logic validation here
            await _recipeRepository.DeleteAsync(id);
        }
    }
}