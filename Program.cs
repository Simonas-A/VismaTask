// See https://aka.ms/new-console-template for more information
using VismaTask;
using VismaTask.Models;

Console.WriteLine("© 2022 Simonas Albrechtas");

//string dataPath = "..//..//..//Data//meetings.json";

//List<Meeting> meetings = IOController.ReadMeetings(dataPath);
CommandHandler handler = new CommandHandler();

Console.WriteLine("Enter your name:");
string user = Console.ReadLine();


string line;

while((line = Console.ReadLine()) != "exit")
{
    int status = handler.Handle(line, user);
    //CommandHandler.Handle(line, user);

    if (status == 5)
    {
        Console.WriteLine(handler.FormatList());
    }
    /*
    if (status > 0)
    {
        
    }
    else
    {
        if (status == 0)
            Console.WriteLine("unknown command");
        else if (status == -1)
            Console.WriteLine("too few arguments");
    }
    */
}