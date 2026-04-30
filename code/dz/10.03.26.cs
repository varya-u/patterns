using System;
using System.Collections.Generic;

public abstract class Document
{
    public string Name { get; set; } = "Untitled";
    public abstract void Save(string filepath);
    public abstract void Load(string filepath);
    public virtual void Print() => Console.WriteLine($"Печать документа: {Name}");
}

public class TextDocument : Document
{
    public override void Save(string filepath)
    {
        Name = filepath;
        Console.WriteLine($"Сохранение текста в: {Name}");
    }
    public override void Load(string filepath)
    {
        Name = filepath;
        Console.WriteLine($"Загрузка текста из: {Name}");
    }
}

public class EditorApplication
{
    private List<Document> _documents = new List<Document>();

    public void CreateDocument(Document doc)
    {
        _documents.Add(doc);
        Console.WriteLine($"Документ {doc.Name} создан.");
    }

    public void OpenDocument(Document doc, string path)
    {
        doc.Load(path);
        _documents.Add(doc);
    }

    public void CloseDocument(Document doc)
    {
        _documents.Remove(doc);
        Console.WriteLine("Документ закрыт.");
    }
}