## ASP.NET 8 Web API Coding Exercise

Uses dotnet sdk version 8.0.411, developed with VS Code on Debian Linux.

### How to run:
Run these commands:
```bash
git clone https://github.com/esmangle/pizza-web-api.git
cd pizza-web-api
dotnet watch run
```
Swagger UI should be accessible at: http://localhost:5212/swagger/index.html

### API Endpoints

Toppings
- `GET /api/toppings` - List all toppings
- `GET /api/toppings/{id}` - Get a topping's data
- `POST /api/toppings` - Create a new topping
- `PUT /api/toppings/{id}` - Update a topping's name
- `DELETE /api/toppings/{id}` - Delete a topping

Pizzas
- `GET /api/pizzas` - List all pizzas with their toppings
- `GET /api/pizzas/{id}` - Get a pizza's data with its toppings
- `POST /api/pizzas` - Create a new pizza with toppings
- `PUT /api/pizzas/{id}` - Update a pizza's name and toppings
- `DELETE /api/pizzas/{id}` - Delete a pizza

### Default sample data:
Toppings:
1. Pepperoni
2. Pineapple
3. Ham
4. Bacon
5. Sausages
6. Mushrooms

Pizzas:
1. Pepperoni
   - Toppings: Pepperoni
2. Hawaiian
   - Toppings: Pineapple, Ham
3. Meat Lover's
   - Toppings: Pepperoni, Ham, Bacon, Sausages
