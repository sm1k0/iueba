public static class ArrowMenu
{
    public static int ShowMenu(IEnumerable<MenuItem> options, string categoryб)
    {
        int currentPosition = 0;
        ConsoleKeyInfo key;

        do
        {
            Console.Clear();


            for (int i = 0; i < options.Count(); i++)
            {
                Console.WriteLine($"{(currentPosition == i ? "->" : "   ")} {options.ElementAt(i).Id}. {options.ElementAt(i).Description}");
            }

            key = Console.ReadKey();

            if (key.Key == ConsoleKey.UpArrow && currentPosition > 0)
            {
                currentPosition--;
            }
            else if (key.Key == ConsoleKey.DownArrow && currentPosition < options.Count() - 1)
            {
                currentPosition++;
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                return -1;
            }

        } while (key.Key != ConsoleKey.Enter);

        return currentPosition;
    }
}