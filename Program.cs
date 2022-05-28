// See https://aka.ms/new-console-template for more information
using VismaTask;

Console.WriteLine("© 2022 Simonas Albrechtas");

string dataPath = "..//..//..//Data//meetings.json";


Console.WriteLine("Enter your name:");
string user = Console.ReadLine();
Console.WriteLine($"Welcome, {user}");
CommandHandler handler = new CommandHandler(user, dataPath);

string line;

while ((line = Console.ReadLine()) != "exit")
{
    Console.WriteLine(handler.Handle(line));
}