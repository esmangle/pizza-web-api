# ASP.NET 8 Web API Coding Exercise

Uses dotnet sdk version 8.0.411, developed with VS Code on Debian Linux. Uses an in-memory database, though it can easily be switched out for another database like SQLite.

## How to run:
Run these commands:
```bash
git clone https://github.com/esmangle/pizza-web-api.git
cd pizza-web-api
dotnet watch run
```
Swagger UI should be accessible at: http://localhost:5212/swagger/index.html

## API Endpoints

### Toppings

`GET /api/toppings` - List all toppings
```json
[
	{"id": 0, "name": "string"}
]
```

`POST /api/toppings` - Create a new topping
```json
{"name": "string"}
```

`GET /api/toppings/{id}` - Get a topping's data
```json
{"id": 0, "name": "string"}
```

`PUT /api/toppings/{id}` - Update a topping's name
```json
{"name": "string"}
```

`DELETE /api/toppings/{id}` - Delete a topping

### Pizzas

`GET /api/pizzas` - List all pizzas with their toppings
```json
[
	{
		"id": 0,
		"name": "string",
		"toppings": [
			{"id": 0, "name": "string"}
		]
	}
]
```

`POST /api/pizzas` - Create a new pizza with toppings
```json
{
	"name": "string",
	"toppingIds": [0]
}
```

`GET /api/pizzas/{id}` - Get a pizza's data with its toppings
```json
{
	"id": 0,
	"name": "string",
	"toppings": [
		{"id": 0, "name": "string"}
	]
}
```

`PUT /api/pizzas/{id}` - Update a pizza's name and toppings
```json
{
	"name": "string",
	"toppingIds": [0]
}
```

`DELETE /api/pizzas/{id}` - Delete a pizza

## Example usage:

Create new topping:
```bash
curl -X POST https://localhost:5212/api/toppings \
	-H "Content-Type: application/json" \
	-d '{"name": "Onions"}'
```

Create new pizza:
```bash
curl -X POST https://localhost:5212/api/pizzas \
	-H "Content-Type: application/json" \
	-d '{"name": "Pepperoni and Ham", "toppingIds": [1, 3]}'
```

## Default sample data:
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
