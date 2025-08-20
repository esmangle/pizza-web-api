## ASP.NET 8 Web API Coding Exercise

Uses dotnet sdk version 8.0.411, developed with VS Code on Debian Linux.

### How to run:
Run these commands:
```bash
git clone https://github.com/esmangle/pizza-web-api.git
cd pizza-web-api
dotnet watch run
```
Swagger should be accessible at: http://localhost:5212/swagger/index.html

### Default seeded data:
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
