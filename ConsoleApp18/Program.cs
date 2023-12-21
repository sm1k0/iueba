using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public static class Program
{
    private const string UsersFilePath = "users.json";
    private static List<User> users;
    private static List<Employee> employees = new List<Employee>();
    private static  List<Product> products = new List<Product>();
    private static List<Receipt> receipts = new List<Receipt>();
    static void Main()
    {
        
        LoadUsers();
        LoadEmployees();
        int userIndex;

        while (true)
        {
            Authenticate(out userIndex);
            if (userIndex != -1)
            {
                User currentUser = users[userIndex];
                if (currentUser.Role == "admin")
                {
                    ShowMainMenu(currentUser);
                }
                else if (currentUser.Role == "hrmanager")
                {
                    ShowHRManagerMenu();
                }
                else if (currentUser.Role == "warehousemanager")
                {
                    WarehouseManager.ShowWarehouseMenu();
                }
                else if (currentUser.Role == "cashier")
                {
                    CashierManager.ShowCashierMenu(products, receipts);
                }
                else if (currentUser.Role == "accountant")
                {
                    AccountantManager.ShowAccountantMenu();
                }
            }

        } while (true);
    }
    private static int AuthenticateUser()
    {
        Console.Write("Введите логин: ");
        string username = Console.ReadLine();

        Console.Write("Введите пароль: ");
        string password = ReadPassword();

        return FindUserIndex(username, password);
    }

    private static int FindUserIndex(string username, string password)
    {
        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].Username == username && users[i].Password == password)
            {
                return i;
            }
        }
        return -1;
    }
    private static void Authenticate(out int userIndex)
    {
        do
        {
            userIndex = AuthenticateUser();
            if (userIndex != -1)
            {
                User currentUser = users[userIndex];
                
                switch (currentUser.Role.ToLower())
                {
                    case "admin":
                        ShowMainMenu(currentUser);
                        break;
                    case "hrmanager":
                        ShowHRManagerMenu();
                        break;
                    case "warehousemanager":
                        WarehouseManager.ShowWarehouseMenu();
                        break;
                    case "cashier":
                        CashierManager.ShowCashierMenu(products, receipts);
                        break;
                    case "accountant":
                        AccountantManager.ShowAccountantMenu();
                        break;
                    default:
                        Console.WriteLine("Неизвестная роль. Пользователь не будет связан с каким-либо меню.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Ошибка аутентификации. Повторите попытку.");
            }
        } while (true);
    }



    private static void ShowMainMenu(User user)
    {
        do
        {
            Console.Clear();
            Console.WriteLine($"=== Главное меню ===\nЗдравствуйте, {user.FullName} ({user.Position})!\n");

            List<MenuItem> menuItems = new List<MenuItem>
        {
            new MenuItem("1", "Просмотреть информацию о пользователях"),
            new MenuItem("2", "Управление пользователями"),
            new MenuItem("3", "Выход")
        };

            int choice = ArrowMenu.ShowMenu(menuItems, "Главное меню");

            switch (choice)
            {
                case 0:
                    ShowUserList();
                    break;
                case 1:
                    ManageUsers();
                    break;
                case 2:
                    SaveUsers();
                    Console.Clear();
                    return;
            }

        } while (true);
    }

    private static void ShowUserList()
    {
        Console.Clear();
        Console.WriteLine("=== Список пользователей ===\n");

        foreach (var user in users)
        {
            Console.WriteLine($"Логин: {user.Username}, Роль: {user.Role}, Имя: {user.FullName}, Должность: {user.Position}");
        }

        Console.WriteLine("\nНажмите любую клавишу для возврата...");
        Console.ReadKey();
    }
    private static void ShowHRManagerMenu()
    {
        do
        {
            Console.Clear();
            Console.WriteLine("=== Меню менеджера персонала ===\n");

            List<MenuItem> menuItems = new List<MenuItem>
            {
                new MenuItem("1", "Просмотреть информацию о сотрудниках"),
                new MenuItem("2", "Управление сотрудниками"),
                new MenuItem("3", "Выход")
            };

            int choice = ArrowMenu.ShowMenu(menuItems, "Меню менеджера персонала");

            switch (choice)
            {
                case 0:
                    ViewEmployeeDetails();
                    break;
                case 1:
                    ManageEmployees();
                    break;
                case 2:
                    SaveEmployees();
                    Environment.Exit(0);
                    break;
            }

        } while (true);
    }
    private static void ViewEmployeeDetails()
    {
        int selectedIndex = ArrowMenu.ShowMenu(employees.Select(e => new MenuItem(e.EmployeeId, $"{e.FirstName} {e.LastName} - {e.Position}")), "Employee Details");

        if (selectedIndex != -1)
        {
            Employee selectedEmployee = employees.ElementAt(selectedIndex);
            ShowEmployeeDetails(selectedEmployee);
        }
    }
    private static void SaveEmployees()
    {
        string json = JsonConvert.SerializeObject(employees, Formatting.Indented);
        File.WriteAllText(UsersFilePath, json);
    }

    private static void ShowEmployeeList()
    {
        Console.Clear();
        Console.WriteLine("=== Список сотрудников ===\n");

        foreach (var employee in employees)
        {
            Console.WriteLine($"ID: {employee.EmployeeId}, Имя: {employee.FirstName} {employee.LastName}, Должность: {employee.Position}");
        }

        Console.WriteLine("\nНажмите любую клавишу для возврата...");
        Console.ReadKey();
    }
    private static void ManageEmployees()
    {
        do
        {
            Console.Clear();
            Console.WriteLine("=== Управление сотрудниками ===\n");

            List<MenuItem> menuItems = new List<MenuItem>
        {
            new MenuItem("1", "Создать сотрудника"),
            new MenuItem("2", "Редактировать сотрудника"),
            new MenuItem("3", "Удалить сотрудника"),
            new MenuItem("4", "Поиск сотрудника"),
            new MenuItem("5", "Вернуться в меню менеджера персонала")
        };

            int choice = ArrowMenu.ShowMenu(menuItems, "Управление сотрудниками");

            switch (choice)
            {
                case 0:
                    CreateEmployee();
                    break;
                case 1:
                    EditEmployee();
                    break;
                case 2:
                    DeleteEmployee();
                    break;
                case 3:
                    SearchEmployee();
                    break;
                case 4:
                    return;
            }

        } while (true);
    }

    private static void CreateEmployee()
    {
        Console.Clear();
        Console.WriteLine("=== Создание сотрудника ===\n");

        Console.Write("Имя: ");
        string firstName = Console.ReadLine();
        Console.Write("Фамилия: ");
        string lastName = Console.ReadLine();
        Console.Write("Должность: ");
        string position = Console.ReadLine();
        Console.Write("Логин: ");
        string login = Console.ReadLine();
        Console.Write("Пароль: ");
        string password = ReadPassword();
        Console.Write("Роль: ");
        string role = Console.ReadLine();

        // Проверка наличия пользователя с введенным ID
        Console.Write("ID пользователя (если есть): ");
        string userId = Console.ReadLine();
        if (!string.IsNullOrEmpty(userId) && !IsUserIdAvailable(userId))
        {
            Console.WriteLine("Этот ID пользователя уже занят. Выберите другой или оставьте пустым.");
            return;
        }

        // Создание экземпляра Employee с использованием конструктора с параметрами
        Employee newEmployee = new Employee(Guid.NewGuid().ToString(), firstName, lastName, position, userId, login, password, role);

        employees.Add(newEmployee);
        SaveEmployees();
    }


    private static bool IsUserIdAvailable(string userId)
    {
        return employees.All(e => e.UserId != userId);
    }

    private static void EditEmployee()
    {
        Console.Clear();
        Console.WriteLine("=== Редактирование сотрудника ===\n");

        int employeeIndex = ChooseEmployee("Выберите сотрудника для редактирования");

        if (employeeIndex != -1)
        {
            Console.Write("Новое имя: ");
            employees[employeeIndex].FirstName = Console.ReadLine();
            Console.Write("Новая фамилия: ");
            employees[employeeIndex].LastName = Console.ReadLine();
            Console.Write("Новая должность: ");
            employees[employeeIndex].Position = Console.ReadLine();

            SaveEmployees();
        }
    }
    private static void LoadEmployees()
    {
        if (File.Exists(UsersFilePath))
        {
            string json = File.ReadAllText(UsersFilePath);
            employees = JsonConvert.DeserializeObject<List<Employee>>(json);
        }
        else
        {
            employees = new List<Employee>();
        }
    }

    private static void DeleteEmployee()
    {
        Console.Clear();
        Console.WriteLine("=== Удаление сотрудника ===\n");

        int employeeIndex = ChooseEmployee("Выберите сотрудника для удаления");

        if (employeeIndex != -1)
        {
            employees.RemoveAt(employeeIndex);
            SaveEmployees();
        }
    }

    private static void SearchEmployee()
    {
        Console.Clear();
        Console.WriteLine("=== Поиск сотрудника ===\n");

        Console.WriteLine("Выберите атрибут для поиска:");
        List<MenuItem> searchAttributes = new List<MenuItem>
    {
        new MenuItem("1", "Имя"),
        new MenuItem("2", "Фамилия"),
        new MenuItem("3", "Должность"),
        new MenuItem("4", "ID пользователя"),
        new MenuItem("5", "Вернуться назад")
    };

        int attributeChoice = ArrowMenu.ShowMenu(searchAttributes, "Выберите атрибут для поиска");

        if (attributeChoice == searchAttributes.Count - 1)
        {
            return; // Вернуться назад
        }

        Console.Write("Введите значение для поиска: ");
        string searchValue = Console.ReadLine();

        List<Employee> searchResults = new List<Employee>();

        switch (attributeChoice)
        {
            case 0:
                searchResults = employees
                    .Where(e => e.FirstName.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) != -1)
                    .ToList();
                break;
            case 1:
                searchResults = employees
                    .Where(e => e.LastName.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) != -1)
                    .ToList();
                break;
            case 2:
                searchResults = employees
                    .Where(e => e.Position.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) != -1)
                    .ToList();
                break;
            case 3:
                searchResults = employees
                    .Where(e => e.UserId?.IndexOf(searchValue, StringComparison.OrdinalIgnoreCase) != -1)
                    .ToList();
                break;
        }

        DisplaySearchResults(searchResults);
    }

    private static void DisplaySearchResults(List<Employee> searchResults)
    {
        Console.Clear();
        Console.WriteLine($"=== Результаты поиска ({searchResults.Count} сотрудников) ===\n");

        foreach (var employee in searchResults)
        {
            Console.WriteLine($"ID: {employee.EmployeeId}, Имя: {employee.FirstName} {employee.LastName}, Должность: {employee.Position}");
        }

        Console.WriteLine("\nНажмите любую клавишу для возврата...");
        Console.ReadKey();
    }
    private static User FindUserById(string userId)
    {
        return users.FirstOrDefault(u => u.Username == userId);
    }
    private static void SearchUserById()
    {
        Console.Clear();
        Console.WriteLine("=== Поиск пользователя по ID ===\n");

        Console.Write("Введите ID пользователя для поиска: ");
        string searchUserId = Console.ReadLine();

        User foundUser = FindUserById(searchUserId);

        if (foundUser != null)
        {
            Console.WriteLine($"Найден пользователь: Логин: {foundUser.Username}, Роль: {foundUser.Role}, Имя: {foundUser.FullName}, Должность: {foundUser.Position}");
        }
        else
        {
            Console.WriteLine($"Пользователь с ID {searchUserId} не найден.");
        }

        Console.WriteLine("\nНажмите любую клавишу для возврата...");
        Console.ReadKey();
    }
    private static int ChooseEmployee(string prompt)
    {
        Console.Clear();
        Console.WriteLine($"{prompt}:\n");

        int employeeIndex = ArrowMenu.ShowMenu(employees
            .Select((e, index) => new MenuItem(index.ToString(), $"{e.FirstName} {e.LastName} - {e.Position}")), "");

        if (employeeIndex == -1)
        {
            return -1;
        }

        return employeeIndex;
    }
    private static void ManageUsers()
{
    do
    {
        Console.Clear();
        Console.WriteLine("=== Управление пользователями ===\n");

        List<MenuItem> menuItems = new List<MenuItem>
        {
            new MenuItem("1", "Создать пользователя"),
            new MenuItem("2", "Редактировать пользователя"),
            new MenuItem("3", "Удалить пользователя"),
            new MenuItem("4", "Поиск пользователя"),
            new MenuItem("5", "Вернуться в главное меню")
        };

        int choice = ArrowMenu.ShowMenu(menuItems, "Управление пользователями");

        switch (choice)
        {
            case 0:
                CreateUser();
                break;
            case 1:
                EditUser();
                break;
            case 2:
                DeleteUser();
                break;
            case 3:
                SearchUser();
                break;
            case 4:
                return;
        }

    } while (true);
}
    private static void CreateUser()
    {
        Console.Clear();
        Console.WriteLine("=== Создание пользователя ===\n");

        User newUser = new User();
        Console.Write("Логин: ");
        newUser.Username = Console.ReadLine();
        Console.Write("Пароль: ");
        newUser.Password = ReadPassword();
        Console.Write("Роль: ");
        newUser.Role = Console.ReadLine();
        Console.Write("Имя: ");
        newUser.FullName = Console.ReadLine();
        Console.Write("Должность: ");
        newUser.Position = Console.ReadLine();

        users.Add(newUser);
        SaveUsers();

    }

    private static void EditUser()
    {
        
        Console.WriteLine("=== Редактирование пользователя ===\n");

        int userIndex = ChooseUser("Выберите пользователя для редактирования");

        if (userIndex != -1)
        {
            Console.Write("Новый логин: ");
            users[userIndex].Username = Console.ReadLine();
            Console.Write("Новый пароль: ");
            users[userIndex].Password = ReadPassword();
            Console.Write("Новая роль: ");
            users[userIndex].Role = Console.ReadLine();
            Console.Write("Новое имя: ");
            users[userIndex].FullName = Console.ReadLine();
            Console.Write("Новая должность: ");
            users[userIndex].Position = Console.ReadLine();

            SaveUsers();
        }
    }

    private static void DeleteUser()
    {
        Console.Clear();
        Console.WriteLine("=== Удаление пользователя ===\n");

        int userIndex = ChooseUser("Выберите пользователя для удаления");

        if (userIndex != -1)
        {
            users.RemoveAt(userIndex);
            SaveUsers();
        }
    }

    private static void SearchUser()
    {
        Console.Clear();
        Console.WriteLine("=== Поиск сотрудника ===\n");

        Console.WriteLine("Выберите атрибут для поиска:");
        List<MenuItem> searchAttributes = new List<MenuItem>
    {
        new MenuItem("1", "Имя"),
        new MenuItem("2", "Фамилия"),
        new MenuItem("3", "Должность"),
        new MenuItem("4", "ID пользователя"),
        new MenuItem("5", "Вернуться назад")
    };

        int attributeChoice = ArrowMenu.ShowMenu(searchAttributes, "Выберите атрибут для поиска");

        if (attributeChoice == searchAttributes.Count - 1)
        {
            return; // Вернуться назад
        }

        Console.Write("Введите значение для поиска: ");
        string searchValue = Console.ReadLine();

        List<Employee> searchResults = new List<Employee>();

        switch (attributeChoice)
        {
            case 0:
                searchResults = employees
                    .Where(e => e.FirstName.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                break;
            case 1:
                searchResults = employees
                    .Where(e => e.LastName.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                break;
            case 2:
                searchResults = employees
                    .Where(e => e.Position.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                break;
            case 3:
                searchResults = employees
                    .Where(e => e.UserId.Equals(searchValue, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                break;
        }

        Console.Clear();
        Console.WriteLine($"=== Результаты поиска ({searchResults.Count} сотрудников) ===\n");

        foreach (var employee in searchResults)
        {
            Console.WriteLine($"ID: {employee.EmployeeId}, Имя: {employee.FirstName} {employee.LastName}, Должность: {employee.Position}");
        }

        Console.WriteLine("\nНажмите любую клавишу для возврата...");
        Console.ReadKey();
    }

    private static int ChooseUser(string prompt)
    {
        Console.Clear();
        Console.WriteLine($"{prompt}:\n");

        int userIndex = ArrowMenu.ShowMenu(users
            .Select(u => new MenuItem(u.Username, !string.IsNullOrWhiteSpace(u.FullName) ? $"{u.FullName} ({u.Position})" : u.Position)), "");

        if (userIndex == -1)
        {
            return -1;
        }

        return userIndex;
    }
    private static void ShowEmployeeDetails(Employee employee)
    {
        Console.Clear();
        Console.WriteLine($"=== Детальная информация о сотруднике ===\n");
        Console.WriteLine($"ID: {employee.EmployeeId}");
        Console.WriteLine($"Имя: {employee.FirstName} {employee.LastName}");
        Console.WriteLine($"Должность: {employee.Position}");
        Console.WriteLine($"ID пользователя: {employee.UserId}");
        Console.WriteLine("\nНажмите любую клавишу для возврата...");
        Console.ReadKey();
    }

    private static void LoadUsers()
    {
        if (File.Exists(UsersFilePath))
        {
            string json = File.ReadAllText(UsersFilePath);
            users = JsonConvert.DeserializeObject<List<User>>(json);
        }
        else
        {
            users = new List<User>();
        }
    }
    private static void CreateAdminUser()
    {
        User adminUser = new User
        {
            Username = "admin",
            Password = "admin", // Замените на надежный пароль
            Role = "admin",
            FullName = "Администратор",
            Position = "Администратор"
        };

        users.Add(adminUser);
        SaveUsers();

    }
    private static void SaveUsers()
    {
        string json = JsonConvert.SerializeObject(users, Formatting.Indented);
        File.WriteAllText(UsersFilePath, json);
    }

    private static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[0..^1];
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }
}
