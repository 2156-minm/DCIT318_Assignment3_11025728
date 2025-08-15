using System;
using System.Collections.Generic;
using System.IO;

namespace SchoolGradingSystem
{
    public class Student
    {
        public int Id;
        public string FullName;
        public int Score;

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            if (Score >= 70) return "B";
            if (Score >= 60) return "C";
            if (Score >= 50) return "D";
            return "F";
        }
    }

    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            List<Student> students = new();

            using (StreamReader reader = new StreamReader(inputFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(',');

                    if (parts.Length != 3)
                        throw new MissingFieldException($"Missing data in line: {line}");

                    if (!int.TryParse(parts[0], out int id))
                        throw new FormatException($"Invalid ID format in line: {line}");

                    string fullName = parts[1].Trim();

                    if (!int.TryParse(parts[2], out int score))
                        throw new InvalidScoreFormatException($"Invalid score format in line: {line}");

                    students.Add(new Student(id, fullName, score));
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            string inputFilePath = "students.txt";
            string outputFilePath = "report.txt";

            if (!File.Exists(inputFilePath))
            {
                File.WriteAllLines(inputFilePath, new[]
                {
                    "101, Alice Johnson, 84",
                    "102, Bob Smith, 73",
                    "103, Charlie Brown, 65",
                    "104, Diana Prince, 58",
                    "105, Evan Wright, 45"
                });
                Console.WriteLine("Sample students.txt file created.\n");
            }

            try
            {
                StudentResultProcessor processor = new StudentResultProcessor();
                var students = processor.ReadStudentsFromFile(inputFilePath);
                processor.WriteReportToFile(students, outputFilePath);

                Console.WriteLine("Report generated successfully! Contents:");
                Console.WriteLine(File.ReadAllText(outputFilePath));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Error: Input file not found.");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}
