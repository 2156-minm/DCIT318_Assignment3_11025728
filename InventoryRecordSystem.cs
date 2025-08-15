using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryRecordSystem
{
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    public interface IInventoryEntity
    {
        int Id { get; }
    }

    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(_log);
        }

        public void SaveToFile()
        {
            try
            {
                using (FileStream fs = new FileStream(_filePath, FileMode.Create))
                {
                    JsonSerializer.Serialize(fs, _log);
                }
                Console.WriteLine($"Data saved to {_filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    using (FileStream fs = new FileStream(_filePath, FileMode.Open))
                    {
                        var data = JsonSerializer.Deserialize<List<T>>(fs);
                        if (data != null)
                        {
                            _log = data;
                        }
                    }
                    Console.WriteLine($"Data loaded from {_filePath}");
                }
                else
                {
                    Console.WriteLine("No file found to load data.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }
    }

    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger;

        public InventoryApp()
        {
            _logger = new InventoryLogger<InventoryItem>("inventory.json");
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Mouse", 50, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Keyboard", 30, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Monitor", 15, DateTime.Now));
            _logger.Add(new InventoryItem(5, "Printer", 5, DateTime.Now));
        }

        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }

        public void PrintAllItems()
        {
            var items = _logger.GetAll();
            Console.WriteLine("\n--- Inventory Items ---");
            foreach (var item in items)
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Qty: {item.Quantity}, Added: {item.DateAdded}");
            }
        }

        public void Run()
        {
            SeedSampleData();
            SaveData();

            _logger = new InventoryLogger<InventoryItem>("inventory.json");
            LoadData();
            PrintAllItems();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            new InventoryApp().Run();
        }
    }
}
