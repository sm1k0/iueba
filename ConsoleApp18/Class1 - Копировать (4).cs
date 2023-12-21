using Newtonsoft.Json;

    public static class WarehouseManager
    {

        public const string ProductsFilePath = "products.json";
        private static List<Product> products;
    static WarehouseManager()
    {
        LoadProducts();
    }

    public static void ShowWarehouseMenu()
    {
        do
        {
            Console.Clear();
            Console.WriteLine("=== Склад-менеджер ===\n");

            List<MenuItem> warehouseMenuItems = new List<MenuItem>
            {
                new MenuItem("1", "Просмотреть все товары"),
                new MenuItem("2", "Добавить товар"),
                new MenuItem("3", "Изменить товар"),
                new MenuItem("4", "Удалить товар"),
                new MenuItem("5", "Поиск товара"),
                new MenuItem("6", "Выход")
            };

            int choice = ArrowMenu.ShowMenu(warehouseMenuItems, "Склад-менеджер");

            switch (choice)
            {
                case 0:
                    ShowAllProducts();
                    break;
                case 1:
                    AddProduct();
                    break;
                case 2:
                    EditProduct();
                    break;
                case 3:
                    DeleteProduct();
                    break;
                case 4:
                    SearchProduct();
                    break;
                case 5:
                    SaveProducts();
                    Console.Clear();
                    return;
            }

        } while (true);
    }

    private static void ShowAllProducts()
    {
        Console.Clear();
        Console.WriteLine("=== Список товаров на складе ===\n");

        foreach (var product in products)
        {
            Console.WriteLine($"ID: {product.ProductId}, Название: {product.Name}, Цена: {product.Price}, Количество: {product.Quantity}");
        }

        Console.WriteLine("\nНажмите любую клавишу для возврата...");
        Console.ReadKey();
    }

    private static void AddProduct()
    {
        Console.Clear();
        Console.WriteLine("=== Добавление товара ===\n");

        Console.Write("Название товара: ");
        string name = Console.ReadLine();
        Console.Write("Цена товара: ");
        decimal price = Convert.ToDecimal(Console.ReadLine());
        Console.Write("Количество товара: ");
        int quantity = Convert.ToInt32(Console.ReadLine());

        Product newProduct = new Product(Guid.NewGuid().ToString(), name, price, quantity);

        products.Add(newProduct);
        SaveProducts();
    }

    private static void EditProduct()
    {
        Console.Clear();
        Console.WriteLine("=== Изменение товара ===\n");

        int productIndex = ChooseProduct("Выберите товар для редактирования");

        if (productIndex != -1)
        {
            Console.Write("Новое название товара: ");
            products[productIndex].Name = Console.ReadLine();
            Console.Write("Новая цена товара: ");
            products[productIndex].Price = Convert.ToDecimal(Console.ReadLine());
            Console.Write("Новое количество товара: ");
            products[productIndex].Quantity = Convert.ToInt32(Console.ReadLine());

            SaveProducts();
        }
    }

    private static void DeleteProduct()
    {
        Console.Clear();
        Console.WriteLine("=== Удаление товара ===\n");

        int productIndex = ChooseProduct("Выберите товар для удаления");

        if (productIndex != -1)
        {
            products.RemoveAt(productIndex);
            SaveProducts();
        }
    }

    private static void SearchProduct()
    {
        Console.Clear();
        Console.WriteLine("=== Поиск товара ===\n");

        Console.WriteLine("Выберите атрибут для поиска:");
        List<MenuItem> searchAttributes = new List<MenuItem>
        {
            new MenuItem("1", "Название"),
            new MenuItem("2", "Цена"),
            new MenuItem("3", "Количество"),
            new MenuItem("4", "Вернуться назад")
        };

        int attributeChoice = ArrowMenu.ShowMenu(searchAttributes, "Выберите атрибут для поиска");

        if (attributeChoice == searchAttributes.Count - 1)
        {
            return; // Вернуться назад
        }

        Console.Write("Введите значение для поиска: ");
        string searchValue = Console.ReadLine();

        List<Product> searchResults = new List<Product>();

        switch (attributeChoice)
        {
            case 0:
                searchResults = products
                    .Where(p => p.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                break;
            case 1:
                searchResults = products
                    .Where(p => p.Price.ToString().Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                break;
            case 2:
                searchResults = products
                    .Where(p => p.Quantity.ToString().Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                break;
        }

        DisplaySearchResults(searchResults);
    }

    private static void DisplaySearchResults(List<Product> searchResults)
    {
        Console.Clear();
        Console.WriteLine($"=== Результаты поиска ({searchResults.Count} товаров) ===\n");

        foreach (var product in searchResults)
        {
            Console.WriteLine($"ID: {product.ProductId}, Название: {product.Name}, Цена: {product.Price}, Количество: {product.Quantity}");
        }

        Console.WriteLine("\nНажмите любую клавишу для возврата...");
        Console.ReadKey();
    }

    private static int ChooseProduct(string prompt)
    {
        Console.Clear();
        Console.WriteLine($"{prompt}:\n");

        int productIndex = ArrowMenu.ShowMenu(products
            .Select((p, index) => new MenuItem(index.ToString(), $"{p.Name} - Цена: {p.Price}, Количество: {p.Quantity}")), "");

        if (productIndex == -1)
        {
            return -1;
        }

        return productIndex;
    }

    private static void LoadProducts()
    {
        if (File.Exists(ProductsFilePath))
        {
            string json = File.ReadAllText(ProductsFilePath);
            products = JsonConvert.DeserializeObject<List<Product>>(json);
        }
        else
        {
            products = new List<Product>();
        }
    }

    private static void SaveProducts()
    {
        string json = JsonConvert.SerializeObject(products, Formatting.Indented);
        File.WriteAllText(ProductsFilePath, json);
    }
}