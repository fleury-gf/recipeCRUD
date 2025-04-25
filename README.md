# Recipe API

A RESTful API for managing recipes and ingredients built with ASP.NET Core.

## Prerequisites

- .NET 9.0 SDK or later
- PostgreSQL database

## API Endpoints
GET /api/Ingredients - Get all ingredients
GET /api/Ingredients/{id} - Get ingredient by ID
POST /api/Ingredients - Create a new ingredient
PUT /api/Ingredients/{id} - Update an ingredient
DELETE /api/Ingredients/{id} - Delete an ingredient
GET /api/Recipes - Get all recipes
GET /api/Recipes/{id} - Get recipe by ID
GET /api/Recipes/{id}/with-ingredients - Get recipe with ingredients
POST /api/Recipes - Create a new recipe
PUT /api/Recipes/{id} - Update a recipe
DELETE /api/Recipes/{id} - Delete a recipe
## Project Structure
recipeCore: Contains domain models and interfaces
recipeInfra: Contains implementations of repositories and services
recipeAPI: Contains API controllers and configuration
## Technologies Used
ASP.NET Core
PostgreSQL
Dapper (ORM)