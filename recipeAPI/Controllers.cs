using Microsoft.AspNetCore.Mvc;
using recipeCore.Interfaces.Services;
using recipeCore.Models;

namespace recipeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;
        private readonly ILogger<IngredientsController> _logger;

        public IngredientsController(IIngredientService ingredientService, ILogger<IngredientsController> logger)
        {
            _ingredientService = ingredientService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetAll()
        {
            try
            {
                var ingredients = await _ingredientService.GetAllIngredientsAsync();
                return Ok(ingredients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all ingredients");
                return StatusCode(500, "An error occurred while retrieving ingredients");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Ingredient>> GetById(int id)
        {
            try
            {
                var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
                if (ingredient == null)
                {
                    return NotFound($"Ingredient with ID {id} not found");
                }
                return Ok(ingredient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ingredient {Id}", id);
                return StatusCode(500, $"An error occurred while retrieving ingredient {id}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Ingredient>> Create([FromBody] Ingredient ingredient)
        {
            try
            {
                var id = await _ingredientService.CreateIngredientAsync(ingredient);
                ingredient.Id = id;
                return CreatedAtAction(nameof(GetById), new { id }, ingredient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ingredient");
                return StatusCode(500, "An error occurred while creating the ingredient");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Ingredient ingredient)
        {
            try
            {
                if (id != ingredient.Id)
                {
                    return BadRequest("ID in URL does not match ID in request body");
                }

                var existingIngredient = await _ingredientService.GetIngredientByIdAsync(id);
                if (existingIngredient == null)
                {
                    return NotFound($"Ingredient with ID {id} not found");
                }

                await _ingredientService.UpdateIngredientAsync(ingredient);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating ingredient {Id}", id);
                return StatusCode(500, $"An error occurred while updating ingredient {id}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existingIngredient = await _ingredientService.GetIngredientByIdAsync(id);
                if (existingIngredient == null)
                {
                    return NotFound($"Ingredient with ID {id} not found");
                }

                await _ingredientService.DeleteIngredientAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting ingredient {Id}", id);
                return StatusCode(500, $"An error occurred while deleting ingredient {id}");
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly ILogger<RecipesController> _logger;

        public RecipesController(IRecipeService recipeService, ILogger<RecipesController> logger)
        {
            _recipeService = recipeService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAll()
        {
            try
            {
                var recipes = await _recipeService.GetAllRecipesAsync();
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all recipes");
                return StatusCode(500, "An error occurred while retrieving recipes");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetById(int id)
        {
            try
            {
                var recipe = await _recipeService.GetRecipeByIdAsync(id);
                if (recipe == null)
                {
                    return NotFound($"Recipe with ID {id} not found");
                }
                return Ok(recipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recipe {Id}", id);
                return StatusCode(500, $"An error occurred while retrieving recipe {id}");
            }
        }

        [HttpGet("{id}/with-ingredients")]
        public async Task<ActionResult<Recipe>> GetWithIngredients(int id)
        {
            try
            {
                var recipe = await _recipeService.GetRecipeWithIngredientsAsync(id);
                if (recipe == null)
                {
                    return NotFound($"Recipe with ID {id} not found");
                }
                return Ok(recipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recipe {Id} with ingredients", id);
                return StatusCode(500, $"An error occurred while retrieving recipe {id} with ingredients");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> Create([FromBody] Recipe recipe)
        {
            try
            {
                var id = await _recipeService.CreateRecipeAsync(recipe);
                recipe.Id = id;
                return CreatedAtAction(nameof(GetById), new { id }, recipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating recipe");
                return StatusCode(500, "An error occurred while creating the recipe");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Recipe recipe)
        {
            try
            {
                if (id != recipe.Id)
                {
                    return BadRequest("ID in URL does not match ID in request body");
                }

                var existingRecipe = await _recipeService.GetRecipeByIdAsync(id);
                if (existingRecipe == null)
                {
                    return NotFound($"Recipe with ID {id} not found");
                }

                await _recipeService.UpdateRecipeAsync(recipe);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating recipe {Id}", id);
                return StatusCode(500, $"An error occurred while updating recipe {id}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existingRecipe = await _recipeService.GetRecipeByIdAsync(id);
                if (existingRecipe == null)
                {
                    return NotFound($"Recipe with ID {id} not found");
                }

                await _recipeService.DeleteRecipeAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting recipe {Id}", id);
                return StatusCode(500, $"An error occurred while deleting recipe {id}");
            }
        }
    }
}