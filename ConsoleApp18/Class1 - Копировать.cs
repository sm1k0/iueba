using Newtonsoft.Json;

public static class AccountantManager
{
    private static List<FinancialRecord> financialRecords;

    static AccountantManager()
    {
        LoadFinancialRecords();
    }

    private static void LoadFinancialRecords()
    {
        string filePath = "financial_records.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            financialRecords = JsonConvert.DeserializeObject<List<FinancialRecord>>(json);
        }
        else
        {
            financialRecords = new List<FinancialRecord>();
        }
    }

    private static void SaveFinancialRecords()
    {
        string filePath = "financial_records.json";
        string json = JsonConvert.SerializeObject(financialRecords, Formatting.Indented);
        File.WriteAllText(filePath, json);
        Console.Clear();
    }

    public static void ShowAccountantMenu()
    {
        do
        {
            Console.Clear();
            Console.WriteLine("=== Меню бухгалтера ===\n");

            List<MenuItem> menuItems = new List<MenuItem>
            {
                new MenuItem("1", "Просмотреть записи бухгалтерии"),
                new MenuItem("2", "Добавить запись бухгалтерии"),
                new MenuItem("3", "Редактировать запись бухгалтерии"),
                new MenuItem("4", "Удалить запись бухгалтерии"),
                new MenuItem("5", "Поиск записи бухгалтерии"),
                new MenuItem("6", "Вернуться в главное меню")
            };

            int choice = ArrowMenu.ShowMenu(menuItems, "Меню бухгалтера");

            switch (choice)
            {
                case 0:
                    DisplayFinancialRecords();
                    break;
                case 1:
                    CreateFinancialRecord();
                    break;
                case 2:
                    EditFinancialRecord();
                    break;
                case 3:
                    DeleteFinancialRecord();
                    break;
                case 4:
                    SearchFinancialRecord();
                    break;
                case 5:
                    SaveFinancialRecords();
                    return;
            }

        } while (true);
    }

    private static void DisplayFinancialRecords()
    {
        Console.Clear();
        Console.WriteLine("=== Записи бухгалтерии ===\n");

        foreach (var record in financialRecords)
        {
            Console.WriteLine($"ID: {record.TransactionId}, Сумма: {record.Amount}, Описание: {record.Description}, Дата: {record.TransactionDate}");
        }

        Console.WriteLine("\nНажмите любую клавишу для возврата...");
        Console.ReadKey();
    }

    private static void CreateFinancialRecord()
    {
        Console.Clear();
        Console.WriteLine("=== Создание записи бухгалтерии ===\n");

        Console.Write("ID транзакции: ");
        string transactionId = Console.ReadLine();
        Console.Write("Сумма: ");
        decimal amount = Convert.ToDecimal(Console.ReadLine());
        Console.Write("Описание: ");
        string description = Console.ReadLine();
        DateTime transactionDate = DateTime.Now;

        FinancialRecord newRecord = new FinancialRecord(transactionId, amount, description, transactionDate);

        financialRecords.Add(newRecord);
        SaveFinancialRecords();
    }

    private static void EditFinancialRecord()
    {
        Console.Clear();
        Console.WriteLine("=== Редактирование записи бухгалтерии ===\n");

        int recordIndex = ChooseFinancialRecord("Выберите запись бухгалтерии для редактирования");

        if (recordIndex != -1)
        {
            Console.Write("Новая сумма: ");
            financialRecords[recordIndex].Amount = Convert.ToDecimal(Console.ReadLine());
            Console.Write("Новое описание: ");
            financialRecords[recordIndex].Description = Console.ReadLine();

            SaveFinancialRecords();
        }
    }

    private static void DeleteFinancialRecord()
    {
        Console.Clear();
        Console.WriteLine("=== Удаление записи бухгалтерии ===\n");

        int recordIndex = ChooseFinancialRecord("Выберите запись бухгалтерии для удаления");

        if (recordIndex != -1)
        {
            financialRecords.RemoveAt(recordIndex);
            SaveFinancialRecords();
        }
    }

    private static void SearchFinancialRecord()
    {
        Console.Clear();
        Console.WriteLine("=== Поиск записи бухгалтерии ===\n");

        Console.WriteLine("Выберите атрибут для поиска:");
        List<MenuItem> searchAttributes = new List<MenuItem>
        {
            new MenuItem("1", "Сумма"),
            new MenuItem("2", "Описание"),
            new MenuItem("3", "Дата"),
            new MenuItem("4", "Вернуться назад")
        };

        int attributeChoice = ArrowMenu.ShowMenu(searchAttributes, "Выберите атрибут для поиска");

        if (attributeChoice == searchAttributes.Count - 1)
        {
            return; // Вернуться назад
        }

        Console.Write("Введите значение для поиска: ");
        string searchValue = Console.ReadLine();

        List<FinancialRecord> searchResults = new List<FinancialRecord>();

        switch (attributeChoice)
        {
            case 0:
                searchResults = financialRecords
                    .Where(r => r.Amount.ToString().IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) != -1)
                    .ToList();
                break;
            case 1:
                searchResults = financialRecords
                    .Where(r => r.Description.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) != -1)
                    .ToList();
                break;
            case 2:
                DateTime searchDate;
                if (DateTime.TryParse(searchValue, out searchDate))
                {
                    searchResults = financialRecords
                        .Where(r => r.TransactionDate.Date == searchDate.Date)
                        .ToList();
                }
                else
                {
                    Console.WriteLine("Некорректный формат даты. Поиск по дате не выполнен.");
                }
                break;
        }

        DisplayFinancialRecords(searchResults);
    }

    private static void DisplayFinancialRecords(List<FinancialRecord> records)
    {
        Console.Clear();
        Console.WriteLine($"=== Результаты поиска ({records.Count} записей) ===\n");

        foreach (var record in records)
        {
            Console.WriteLine($"ID: {record.TransactionId}, Сумма: {record.Amount}, Описание: {record.Description}, Дата: {record.TransactionDate}");
        }

        Console.WriteLine("\nНажмите любую клавишу для возврата...");
        Console.ReadKey();
    }

    private static int ChooseFinancialRecord(string prompt)
    {
        Console.Clear();
        Console.WriteLine($"{prompt}:\n");

        int recordIndex = ArrowMenu.ShowMenu(financialRecords
            .Select((r, index) => new MenuItem(index.ToString(), $"{r.Amount} - {r.Description} - {r.TransactionDate}")), "");

        if (recordIndex == -1)
        {
            return -1;
        }

        return recordIndex;
    }
}