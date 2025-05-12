using System;
using System.IO;
using System.Linq;

class DiskManager
{
    private static DriveInfo[] allDrives;
    private static string currentPath;

    static void Main(string[] args)
    {
        Console.Title = "Disk Manager";
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        allDrives = DriveInfo.GetDrives();
        currentPath = null;

        while (true)
        {
            Console.Clear();
            DisplayMainMenu();

            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            ProcessMainMenuInput(input.ToLower());
        }
    }

    static void DisplayMainMenu()
    {
        Console.WriteLine("=== Управление дисками ===");
        Console.WriteLine("1. Просмотреть доступные диски");

        if (currentPath != null)
        {
            Console.WriteLine("2. Информация о текущем диске");
            Console.WriteLine("3. Просмотр содержимого текущего диска");
            Console.WriteLine("4. Создать новый каталог");
            Console.WriteLine("5. Создать новый файл");
            Console.WriteLine("6. Удалить файл или каталог");
            Console.WriteLine("7. Сменить текущий диск");
        }
        else
        {
            Console.WriteLine("2. Выбрать диск для работы");
        }

        Console.WriteLine("0. Выход");
        Console.Write("Выберите действие: ");
    }

    static void ProcessMainMenuInput(string input)
    {
        switch (input)
        {
            case "1":
                DisplayAvailableDrives();
                break;
            case "2":
                if (currentPath == null)
                    SelectDrive();
                else
                    DisplayDriveInfo();
                break;
            case "3":
                if (currentPath != null)
                    BrowseDiskContent();
                break;
            case "4":
                if (currentPath != null)
                    CreateNewDirectory();
                break;
            case "5":
                if (currentPath != null)
                    CreateNewFile();
                break;
            case "6":
                if (currentPath != null)
                    DeleteItem();
                break;
            case "7":
                if (currentPath != null)
                    SelectDrive();
                break;
            case "0":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Неверная команда!");
                Console.ReadKey();
                break;
        }
    }

    static void DisplayAvailableDrives()
    {
        Console.Clear();
        Console.WriteLine("Доступные диски:");
        Console.WriteLine("----------------");

        foreach (var drive in allDrives)
        {
            if (drive.IsReady)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{drive.Name} ");
                Console.ResetColor();
                Console.WriteLine($"[{drive.DriveType}] {drive.VolumeLabel}");
                Console.WriteLine($"  Всего места: {drive.TotalSize / (1024 * 1024 * 1024)} GB");
                Console.WriteLine($"  Свободно: {drive.TotalFreeSpace / (1024 * 1024 * 1024)} GB");
                Console.WriteLine($"  Файловая система: {drive.DriveFormat}");
                Console.WriteLine();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{drive.Name} [Не готов]");
                Console.ResetColor();
            }
        }

        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }

    static void SelectDrive()
    {
        Console.Clear();
        Console.WriteLine("Выберите диск:");

        for (int i = 0; i < allDrives.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {allDrives[i].Name} [{allDrives[i].DriveType}]");
        }

        Console.Write("Введите номер диска (или 0 для отмены): ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= allDrives.Length)
        {
            var selectedDrive = allDrives[choice - 1];
            if (selectedDrive.IsReady)
            {
                currentPath = selectedDrive.RootDirectory.FullName;
                Console.WriteLine($"Выбран диск: {selectedDrive.Name}");
            }
            else
            {
                Console.WriteLine("Диск не готов к работе!");
                Console.ReadKey();
            }
        }
    }

    static void DisplayDriveInfo()
    {
        var drive = new DriveInfo(Path.GetPathRoot(currentPath));

        Console.Clear();
        Console.WriteLine($"Информация о диске {drive.Name}:");
        Console.WriteLine("-------------------------------");
        Console.WriteLine($"Метка тома: {drive.VolumeLabel}");
        Console.WriteLine($"Тип диска: {drive.DriveType}");
        Console.WriteLine($"Файловая система: {drive.DriveFormat}");
        Console.WriteLine($"Общий размер: {drive.TotalSize / (1024 * 1024 * 1024)} GB");
        Console.WriteLine($"Свободное место: {drive.TotalFreeSpace / (1024 * 1024 * 1024)} GB");
        Console.WriteLine($"Занятое место: {(drive.TotalSize - drive.TotalFreeSpace) / (1024 * 1024 * 1024)} GB");

        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }

    static void BrowseDiskContent()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Содержимое: {currentPath}");
            Console.WriteLine("----------------------------------");

            try
            {
               
                var directories = Directory.GetDirectories(currentPath);
                Console.ForegroundColor = ConsoleColor.Blue;
                foreach (var dir in directories)
                {
                    var dirInfo = new DirectoryInfo(dir);
                    Console.WriteLine($"[DIR]  {Path.GetFileName(dir)} \t{dirInfo.LastWriteTime}");
                }
                Console.ResetColor();

               
                var files = Directory.GetFiles(currentPath);
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    Console.WriteLine($"[FILE] {Path.GetFileName(file)} \t{fileInfo.Length / 1024} KB \t{fileInfo.LastWriteTime}");
                }

                Console.WriteLine("\nКоманды:");
                Console.WriteLine("1. Перейти в папку");
                Console.WriteLine("2. Подняться на уровень выше");
                Console.WriteLine("3. Вернуться в главное меню");
                Console.Write("Выберите действие: ");

                var input = Console.ReadLine();

                if (input == "1")
                {
                    Console.Write("Введите имя папки: ");
                    var folderName = Console.ReadLine();
                    var newPath = Path.Combine(currentPath, folderName);

                    if (Directory.Exists(newPath))
                    {
                        currentPath = newPath;
                    }
                    else
                    {
                        Console.WriteLine("Папка не найдена!");
                        Console.ReadKey();
                    }
                }
                else if (input == "2")
                {
                    var parent = Directory.GetParent(currentPath);
                    if (parent != null)
                    {
                        currentPath = parent.FullName;
                    }
                    else
                    {
                        Console.WriteLine("Вы в корневой директории диска!");
                        Console.ReadKey();
                    }
                }
                else if (input == "3")
                {
                    break;
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Ошибка доступа к этой директории!");
                Console.ReadKey();
                break;
            }
        }
    }

    static void CreateNewDirectory()
    {
        Console.Clear();
        Console.WriteLine($"Создание новой папки в: {currentPath}");
        Console.Write("Введите имя новой папки: ");
        var dirName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(dirName))
        {
            Console.WriteLine("Имя папки не может быть пустым!");
            Console.ReadKey();
            return;
        }

        try
        {
            var newDirPath = Path.Combine(currentPath, dirName);
            Directory.CreateDirectory(newDirPath);
            Console.WriteLine($"Папка '{dirName}' успешно создана!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при создании папки: {ex.Message}");
        }

        Console.ReadKey();
    }

    static void CreateNewFile()
    {
        Console.Clear();
        Console.WriteLine($"Создание нового файла в: {currentPath}");
        Console.Write("Введите имя файла: ");
        var fileName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(fileName))
        {
            Console.WriteLine("Имя файла не может быть пустым!");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Введите содержимое файла (для завершения введите пустую строку):");
        var contentLines = new System.Collections.Generic.List<string>();
        while (true)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;
            contentLines.Add(line);
        }

        try
        {
            var filePath = Path.Combine(currentPath, fileName);
            File.WriteAllLines(filePath, contentLines);
            Console.WriteLine($"Файл '{fileName}' успешно создан!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при создании файла: {ex.Message}");
        }

        Console.ReadKey();
    }

    static void DeleteItem()
    {
        Console.Clear();
        Console.WriteLine($"Удаление файла или папки в: {currentPath}");
        Console.Write("Введите имя файла или папки для удаления: ");
        var itemName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(itemName))
        {
            Console.WriteLine("Имя не может быть пустым!");
            Console.ReadKey();
            return;
        }

        var itemPath = Path.Combine(currentPath, itemName);

        Console.Write($"Вы уверены, что хотите удалить '{itemName}'? (y/n): ");
        var confirm = Console.ReadLine().ToLower();

        if (confirm == "y")
        {
            try
            {
                if (Directory.Exists(itemPath))
                {
                    Directory.Delete(itemPath, true);
                    Console.WriteLine($"Папка '{itemName}' успешно удалена!");
                }
                else if (File.Exists(itemPath))
                {
                    File.Delete(itemPath);
                    Console.WriteLine($"Файл '{itemName}' успешно удален!");
                }
                else
                {
                    Console.WriteLine("Файл или папка не найдены!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении: {ex.Message}");
            }
        }

        Console.ReadKey();
    }
}