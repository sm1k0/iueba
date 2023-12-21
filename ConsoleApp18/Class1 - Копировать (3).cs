using Newtonsoft.Json;

public static class CashierManager
{
    private const string ReceiptsFilePath = "receipts.json";

    public static void ShowCashierMenu(List<Product> products, List<Receipt> receipts)
    {
        do
        {
            Console.Clear();
            Console.WriteLine("=== Меню кассы ===\n");

            DisplayProducts(products);

            Console.WriteLine("\nВыберите товар (используйте стрелки + и - для добавления или уменьшения количества):");
            int selectedProductIndex = SelectProduct(products.Count);

            if (selectedProductIndex == -1)
                break;

            Product selectedProduct = products[selectedProductIndex];

            Console.Write("Введите количество: ");
            int quantity = ReadQuantity();

            if (quantity > 0 && quantity <= selectedProduct.StockQuantity)
            {
                ProcessOrder(selectedProduct, quantity, receipts, products);
                SaveData(products, receipts);
            }
            else
            {
                Console.WriteLine("Некорректное количество товара. Пожалуйста, выберите количество от 1 до " + selectedProduct.StockQuantity);
                Console.ReadKey();
            }

        } while (true);
    }

    private static void DisplayProducts(List<Product> products)
    {
        Console.WriteLine("=== Товары на складе ===\n");

        for (int i = 0; i < products.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {products[i].Name} - {products[i].StockQuantity} шт. по {products[i].Price} руб.");
        }
    }

    private static int SelectProduct(int maxIndex)
    {
        int selectedIndex = 0;
        ConsoleKeyInfo key;

        do
        {
            Console.SetCursorPosition(0, Console.CursorTop + 1);
            Console.Write($"->    {selectedIndex + 1}");

            key = Console.ReadKey();

            if (key.Key == ConsoleKey.UpArrow && selectedIndex > 0)
            {
                selectedIndex--;
            }
            else if (key.Key == ConsoleKey.DownArrow && selectedIndex < maxIndex - 1)
            {
                selectedIndex++;
            }
            else if (key.Key == ConsoleKey.S)
            {
                return -1; // Завершение заказа
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                return -1; // Возврат к предыдущему меню
            }

        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine(); // Переход на новую строку после выбора товара
        return selectedIndex;
    }

    private static int ReadQuantity()
    {
        int quantity;
        while (!int.TryParse(Console.ReadLine(), out quantity) || quantity < 0)
        {
            Console.WriteLine("Некорректное количество. Введите положительное число.");
        }
        return quantity;
    }

    private static void ProcessOrder(Product product, int quantity, List<Receipt> receipts, List<Product> products)
    {
        // Создание нового чека
        Receipt receipt = new Receipt
        {
            ProductId = product.ProductId,
            Quantity = quantity,
            TotalPrice = product.Price * quantity,
            PurchaseDate = DateTime.Now
        };

        // Обновление количества товара на складе
        product.StockQuantity -= quantity;

        // Обновление бухгалтерии
        // Предположим, что у вас есть класс Accounting, который отвечает за учет финансов
        Accounting.UpdateAccounting(receipt.TotalPrice);

        // Добавление чека в список
        receipts.Add(receipt);

        // Вывод подтверждения заказа
        Console.WriteLine($"Заказ успешно оформлен!\n" +
                          $"Товар: {product.Name}, Количество: {quantity}, Общая сумма: {receipt.TotalPrice} руб.\n" +
                          $"Дата покупки: {receipt.PurchaseDate}\n" +
                          $"Нажмите любую клавишу для продолжения...");

        Console.ReadKey();
    }

    private static void SaveData(List<Product> products, List<Receipt> receipts)
    {
        // Сохранение данных в файлы JSON
        string productsJson = JsonConvert.SerializeObject(products, Formatting.Indented);
        File.WriteAllText(WarehouseManager.ProductsFilePath, productsJson); // Используем путь из WarehouseManager

        string receiptsJson = JsonConvert.SerializeObject(receipts, Formatting.Indented);
        File.WriteAllText(ReceiptsFilePath, receiptsJson);
    }
}
