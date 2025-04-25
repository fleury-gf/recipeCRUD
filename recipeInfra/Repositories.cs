using System.Data;
using Dapper;
using recipeCore.Interfaces.Repositories;
using recipeCore.Models;
using recipeInfra.Data;

namespace recipeInfra.Repositories
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public IngredientRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Ingredient>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Ingredient>(
                "SELECT id, name, unit_of_measure AS UnitOfMeasure, created_at AS CreatedAt, updated_at AS UpdatedAt FROM ingredients");
        }

        public async Task<Ingredient?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Ingredient>(
                "SELECT id, name, unit_of_measure AS UnitOfMeasure, created_at AS CreatedAt, updated_at AS UpdatedAt FROM ingredients WHERE id = @Id", 
                new { Id = id });
        }

        public async Task<int> CreateAsync(Ingredient ingredient)
        {
            using var connection = _connectionFactory.CreateConnection();
            var now = DateTime.UtcNow;
            ingredient.CreatedAt = now;
            ingredient.UpdatedAt = now;

            var sql = @"INSERT INTO ingredients (name, unit_of_measure, created_at, updated_at) 
                        VALUES (@Name, @UnitOfMeasure, @CreatedAt, @UpdatedAt) 
                        RETURNING id";

            return await connection.QuerySingleAsync<int>(sql, new { 
                ingredient.Name, 
                UnitOfMeasure = ingredient.UnitOfMeasure,
                ingredient.CreatedAt,
                ingredient.UpdatedAt 
            });
        }

        public async Task UpdateAsync(Ingredient ingredient)
        {
            using var connection = _connectionFactory.CreateConnection();
            ingredient.UpdatedAt = DateTime.UtcNow;

            await connection.ExecuteAsync(
                "UPDATE ingredients SET name = @Name, unit_of_measure = @UnitOfMeasure, updated_at = @UpdatedAt WHERE id = @Id",
                new { 
                    ingredient.Id, 
                    ingredient.Name, 
                    UnitOfMeasure = ingredient.UnitOfMeasure,
                    ingredient.UpdatedAt 
                });
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync("DELETE FROM ingredients WHERE id = @Id", new { Id = id });
        }
    }



    public class RecipeRepository : IRecipeRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public RecipeRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Recipe>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Recipe>(
                "SELECT id, title, preparation_instructions AS PreparationInstructions, " +
                "preparation_time_minutes AS PreparationTimeMinutes, created_at AS CreatedAt, updated_at AS UpdatedAt FROM recipes");
        }

        public async Task<Recipe?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<Recipe>(
                "SELECT id, title, preparation_instructions AS PreparationInstructions, " +
                "preparation_time_minutes AS PreparationTimeMinutes, created_at AS CreatedAt, updated_at AS UpdatedAt " +
                "FROM recipes WHERE id = @Id",
                new { Id = id });
        }

        public async Task<Recipe?> GetRecipeWithIngredientsAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            
            var sql = @"
                SELECT 
                    r.id, r.title, r.preparation_instructions AS PreparationInstructions, 
                    r.preparation_time_minutes AS PreparationTimeMinutes, r.created_at AS CreatedAt, r.updated_at AS UpdatedAt,
                    ri.recipe_id AS RecipeId, ri.ingredient_id AS IngredientId, ri.quantity,
                    i.id, i.name, i.unit_of_measure AS UnitOfMeasure
                FROM recipes r
                LEFT JOIN recipe_ingredients ri ON r.id = ri.recipe_id
                LEFT JOIN ingredients i ON ri.ingredient_id = i.id
                WHERE r.id = @Id";
            
            Recipe? recipe = null;
            var ingredientDictionary = new Dictionary<int, Ingredient>();
            
            await connection.QueryAsync<Recipe, RecipeIngredient, Ingredient, Recipe>(
                sql,
                (r, ri, i) => {
                    if (recipe == null)
                    {
                        recipe = r;
                        recipe.Ingredients = new List<RecipeIngredient>();
                    }
                    
                    if (i != null && i.Id != 0)
                    {
                        if (!ingredientDictionary.TryGetValue(i.Id, out var ingredient))
                        {
                            ingredient = i;
                            ingredientDictionary[i.Id] = ingredient;
                        }
                        
                        ri.Ingredient = ingredient;
                        recipe.Ingredients.Add(ri);
                    }
                    
                    return recipe;
                },
                new { Id = id },
                splitOn: "RecipeId,Id"
            );
            
            return recipe;
        }

        public async Task<int> CreateAsync(Recipe recipe)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            
            try
            {
                var now = DateTime.UtcNow;
                recipe.CreatedAt = now;
                recipe.UpdatedAt = now;

                var sql = @"INSERT INTO recipes (title, preparation_instructions, preparation_time_minutes, created_at, updated_at) 
                            VALUES (@Title, @PreparationInstructions, @PreparationTimeMinutes, @CreatedAt, @UpdatedAt) 
                            RETURNING id";

                var recipeId = await connection.QuerySingleAsync<int>(sql, new {
                    recipe.Title,
                    PreparationInstructions = recipe.PreparationInstructions,
                    PreparationTimeMinutes = recipe.PreparationTimeMinutes,
                    recipe.CreatedAt,
                    recipe.UpdatedAt
                }, transaction);

                if (recipe.Ingredients?.Any() == true)
                {
                    var ingredientSql = "INSERT INTO recipe_ingredients (recipe_id, ingredient_id, quantity) VALUES (@RecipeId, @IngredientId, @Quantity)";
                    
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        await connection.ExecuteAsync(ingredientSql, new {
                            RecipeId = recipeId,
                            IngredientId = ingredient.IngredientId,
                            ingredient.Quantity
                        }, transaction);
                    }
                }

                transaction.Commit();
                return recipeId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task UpdateAsync(Recipe recipe)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            
            try
            {
                recipe.UpdatedAt = DateTime.UtcNow;

                await connection.ExecuteAsync(
                    "UPDATE recipes SET title = @Title, preparation_instructions = @PreparationInstructions, " +
                    "preparation_time_minutes = @PreparationTimeMinutes, updated_at = @UpdatedAt WHERE id = @Id",
                    new {
                        recipe.Id,
                        recipe.Title,
                        PreparationInstructions = recipe.PreparationInstructions,
                        PreparationTimeMinutes = recipe.PreparationTimeMinutes,
                        recipe.UpdatedAt
                    }, transaction);

                // Delete existing recipe ingredients
                await connection.ExecuteAsync(
                    "DELETE FROM recipe_ingredients WHERE recipe_id = @RecipeId",
                    new { RecipeId = recipe.Id }, transaction);

                // Add updated recipe ingredients
                if (recipe.Ingredients?.Any() == true)
                {
                    var ingredientSql = "INSERT INTO recipe_ingredients (recipe_id, ingredient_id, quantity) VALUES (@RecipeId, @IngredientId, @Quantity)";
                    
                    foreach (var ingredient in recipe.Ingredients)
                    {
                        // Handle case where ingredientId is 0 but we have a nested ingredient object
                        if (ingredient.IngredientId == 0 && ingredient.Ingredient != null)
                        {
                            ingredient.IngredientId = ingredient.Ingredient.Id;
                        }
                        
                        await connection.ExecuteAsync(ingredientSql, new {
                            RecipeId = recipe.Id,
                            IngredientId = ingredient.IngredientId,
                            ingredient.Quantity
                        }, transaction);
                    }
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            
            try
            {
                // Delete recipe ingredients first (respects foreign key)
                await connection.ExecuteAsync(
                    "DELETE FROM recipe_ingredients WHERE recipe_id = @Id",
                    new { Id = id }, transaction);
                
                // Delete the recipe
                await connection.ExecuteAsync(
                    "DELETE FROM recipes WHERE id = @Id",
                    new { Id = id }, transaction);
                
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}


