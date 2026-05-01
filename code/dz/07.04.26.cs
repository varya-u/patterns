using System;
using System.Linq;

class Program
{
    static void Main()
    {
        string input = " касьмин михаил, кириллова виктория, урсу мария, худи виктория";

        var parts = input.Split(',')
                         .Select(part => part.Trim())
                         .Where(part => !string.IsNullOrEmpty(part));

        var formattedNames = parts.Select(namePair =>
        {
            var words = namePair.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(word => word.Trim())
                                .ToArray();

            if (words.Length < 2)
                return "";

            string lastName = char.ToUpper(words[0][0]) + words[0].Substring(1).ToLower();
            string firstName = char.ToUpper(words[1][0]) + words[1].Substring(1).ToLower();

            return lastName + " " + firstName;
        })
        .Where(name => !string.IsNullOrEmpty(name));

        string result = string.Join("\n",
            formattedNames.Select((name, index) => (index + 1) + ". " + name));

        Console.WriteLine(result);
    }
}