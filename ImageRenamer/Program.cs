using System;
using System.Globalization;
using System.IO;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: Program <directory_path>");
            return;
        }

        string directoryPath = args[0];

        if (!System.IO.Directory.Exists(directoryPath))
        {
            Console.WriteLine("Directory not found.");
            return;
        }

        var files = System.IO.Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);

        foreach (var filePath in files)
        {
            try
            {
                var directories = ImageMetadataReader.ReadMetadata(filePath);
                var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();

                if (subIfdDirectory != null && subIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out DateTime dateTaken))
                {
                    // Format the new file name
                    string originalFileName = Path.GetFileName(filePath);
                    string newFileName = $"{dateTaken:yyyy-MM-dd-HH-mm-ss}_{originalFileName}";
                    string newFilePath = Path.Combine(directoryPath, newFileName);

                    // Rename the file
                    File.Move(filePath, newFilePath);

                    Console.WriteLine($"File renamed to: {newFileName}");
                }
                else
                {
                    Console.WriteLine($"DateTimeOriginal property not found in the image: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
            }
        }
    }
}
