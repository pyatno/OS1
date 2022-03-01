﻿using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.IO.Compression;

static void Compress(string sourceFile, string compressedFile)
{
    using (FileStream sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
    {
        using (FileStream targetStream = File.Create(compressedFile))
        {
            using (GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
            {
                sourceStream.CopyTo(compressionStream);
                Console.WriteLine("Сжатие файла {0} завершено. Исходный размер: {1}  сжатый размер: {2}.",
                    sourceFile, sourceStream.Length.ToString(), targetStream.Length.ToString());
            }
        }
    }
}

static void Decompress(string compressedFile, string targetFile)
{
    using (FileStream sourceStream = new FileStream(compressedFile, FileMode.OpenOrCreate))
    {
        using (FileStream targetStream = File.Create(targetFile))
        {
            using (GZipStream decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
            {
                decompressionStream.CopyTo(targetStream);
                Console.WriteLine("Восстановлен файл: {0}", targetFile);
            }
        }
    }
}

DriveInfo[] drives = DriveInfo.GetDrives();

Console.WriteLine("Меню\n\n");
Console.WriteLine("1-е задание\n");
Console.WriteLine("2-е задание\n");
Console.WriteLine("3-е задание\n");
Console.WriteLine("4-е задание\n");
Console.WriteLine("5-е задание\n\n");

Menu:

var menuArgument = Console.ReadLine();

switch (Convert.ToInt32(menuArgument))
{
    case 1:
        foreach (DriveInfo drive in drives)
        {
            Console.WriteLine($"\nINFO:\nНазвание: {drive.Name}");
            Console.WriteLine($"Тип: {drive.DriveType}");

            if (drive.IsReady)
            {
                Console.WriteLine($"Объем диска: {drive.TotalSize}");
                Console.WriteLine($"Свободное пространство: {drive.TotalFreeSpace}");
                Console.WriteLine($"Метка диска: {drive.VolumeLabel}");
            }

            Console.WriteLine();
        }
        break;
    case 2:
        Console.WriteLine("\nFILE:\n\nВведите строку:\n\n");
        var fileString = Console.ReadLine();
        string pathToFile = @"C:\new_file.txt";
        File.WriteAllText(pathToFile, fileString);

        using (FileStream fstream = File.OpenRead(pathToFile))
        {
            byte[] array = new byte[fstream.Length];
            fstream.Read(array, 0, array.Length);
            string textFromFile = System.Text.Encoding.Default.GetString(array);
            Console.WriteLine($"Текст из файла: {textFromFile}");
        }

        File.Delete(pathToFile);
        Console.WriteLine("\nФайл удален");
        break;
    case 3:
        Console.WriteLine("Введите имя:");
        string name = Console.ReadLine();
        Console.WriteLine("Введите ID:");
        int id = Convert.ToInt32(Console.ReadLine());

        Person tom = new Person { Name = name, Id = id };
        string json = JsonSerializer.Serialize<Person>(tom);
        Console.WriteLine(json);
        var restoredPerson = JsonSerializer.Deserialize<Person>(json);
        Console.WriteLine(restoredPerson.Name);

        using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
        {
            await JsonSerializer.SerializeAsync<Person>(fs, tom);
            Console.WriteLine("Data has been saved to file");
        }

        using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
        {
            Console.WriteLine($"Name: {restoredPerson.Name}  Id: {restoredPerson.Id}");
        }
        File.Delete("user.json");
        break;
    case 4:
        Console.WriteLine("\nUSER1:\nИмя:");
        string nameXml1 = Console.ReadLine();
        Console.WriteLine("Id:");
        int idXml1 = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("\nUSER2:\nИмя:");
        string nameXml2 = Console.ReadLine();
        Console.WriteLine("Id:");
        int idXml2 = Convert.ToInt32(Console.ReadLine());

        XDocument xdoc = new XDocument();

        XElement person1 = new XElement("person");
        XAttribute person1NameAttr = new XAttribute("Name", nameXml1);
        XElement person1IdAttr = new XElement("Id", idXml1);
        person1.Add(person1NameAttr);
        person1.Add(person1IdAttr);

        XElement person2 = new XElement("person");
        XAttribute person2NameAttr = new XAttribute("Name", nameXml2);
        XElement person2IdAttr = new XElement("Id", idXml2);
        person2.Add(person2NameAttr);
        person2.Add(person2IdAttr);

        XElement people = new XElement("people");

        people.Add(person1);
        people.Add(person2);

        xdoc.Add(people);
        xdoc.Save("people.xml");

        XmlDocument xDoc = new XmlDocument();
        xDoc.Load("people.xml");

        XmlElement xRoot = xDoc.DocumentElement;

        Console.WriteLine("\nXML FILE:\n");

        foreach (XmlNode xnode in xRoot)
        {
            if (xnode.Attributes.Count > 0)
            {
                XmlNode attr = xnode.Attributes.GetNamedItem("Name");
                if (attr != null) Console.WriteLine(attr.Value);
            }

            foreach (XmlNode childnode in xnode.ChildNodes)
            {
                if (childnode.Name == "Name") Console.WriteLine($"Name: {childnode.InnerText}");
                if (childnode.Name == "Id") Console.WriteLine($"Id: {childnode.InnerText}");
            }
        }

        File.Delete("people.xml");

        break;
    case 5:

        Console.WriteLine("\nFILE NAME:");
        string sourceFile = Console.ReadLine();
        string compressedFile = "book.gz";
        string targetFile = "book_new.pdf";

        Compress(sourceFile, compressedFile);
        Decompress(compressedFile, targetFile);

        Console.ReadLine();

        break;
    default:
        goto Menu;
}

internal class Person
{
    public string? Name { get; set; }
    public int Id { get; set; }
}
