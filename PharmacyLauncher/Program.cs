namespace PharmacyLauncher;

using System.Diagnostics;

internal static class Program
{
    private const string AppUrl = "https://abc-pharmacy.onrender.com/";

    private static void Main()
    {
        Console.Title = "ABC Pharmacy Launcher";

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=================================");
            Console.WriteLine("   ABC Pharmacy Launcher");
            Console.WriteLine("=================================");
            Console.WriteLine();
            Console.WriteLine($"  App URL: {AppUrl}");
            Console.WriteLine();
            Console.WriteLine("  [1] Preview  - Open app in browser");
            Console.WriteLine("  [2] Exit");
            Console.WriteLine();
            Console.Write("  Enter choice: ");

            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                case "p":
                case "P":
                    OpenPreview();
                    break;
                case "2":
                case "q":
                case "Q":
                    return;
                default:
                    Console.WriteLine();
                    Console.WriteLine("  Invalid choice. Press 1 for Preview or 2 to Exit.");
                    Pause();
                    break;
            }
        }
    }

    private static void OpenPreview()
    {
        try
        {
            Console.WriteLine();
            Console.WriteLine("  Opening preview in your default browser...");

            Process.Start(new ProcessStartInfo
            {
                FileName = AppUrl,
                UseShellExecute = true
            });

            Console.WriteLine("  Preview opened successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Failed to open browser: {ex.Message}");
        }

        Pause();
    }

    private static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("  Press any key to return to menu...");
        Console.ReadKey(true);
    }
}
