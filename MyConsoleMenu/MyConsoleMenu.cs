using EntityService;
using System.Collections;
using System.Runtime.Serialization;

namespace MyConsoleMenu;
public class MyConsoleMenu : ConsoleMenu.ConsoleMenu
{
    static Dictionary<string, Action> commands = new Dictionary<string, Action>();

    public MyConsoleMenu()
    {
        commands.Add("/info", () => Info());
        commands.Add("/add", () => Add());
        commands.Add("/add def", () => Add(true));
        commands.Add("/delete", () => Delete());
        commands.Add("/show", () => ShowAll());
        commands.Add("/search", () => Search());
        commands.Add("/clear", () => ClearFile());
        commands.Add("/end", () => { Console.WriteLine("Bye, have a good time!"); Console.Read(); });
        commands.Add("/cls", () => { Console.Clear(); Info(); });
    }

    #region Start
    public override void Start()
    {
        string input = "";
        commands["/info"]();
        do
        {
            try
            {
                Console.Write("Enter the command: ");
                input = Console.ReadLine();
                int number;
                if (int.TryParse(input, out number))
                {
                    switch (number)
                    {
                        case 1:
                            input = "/info";
                            break;
                        case 2:
                            input = "/add";
                            break;
                        case 3:
                            input = "/delete";
                            break;
                        case 4:
                            input = "/show";
                            break;
                        case 5:
                            input = "/search";
                            break;
                        case 6:
                            input = "/clear";
                            break;
                        case 7:
                            input = "/cls";
                            break;
                        case 8:
                            input = "end";
                            break;
                        case 21:
                            input = "/add def";
                            break;
                    }
                }
                if (commands.ContainsKey(input))
                    commands[input]();
                else
                {
                    Console.WriteLine("Unknow Command");
                }
            }
            catch (SerializationException)
            {
                Console.WriteLine("File is empty or not filled with students!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        } while (input != "/end");
    }
    public override void Info()
    {
        string info = "***** Serialization 2.0 *****\n" +
            "1./info\n" +
            "2./add\n" +
            "  2.1/add def\n" +
            "3./delete\n" +
            "4./show\n" +
            "5./search\n" +
            "6./clear\n" +
            "7./cls\n" +
            "8./end\n";
        Console.WriteLine(info);
    }
    #endregion

    #region Add
    private void Add(bool def = false)
    {
        InteractWithPerson iwp = new InteractWithPerson();

        if (!AskForFilePath(ref iwp)) return;

        if (def)
        {
            iwp.Add(iwp.DefList);
        }
        else
        {
            FillInfoAboutStudent(ref iwp);
        }
        ShowAll(iwp.FilePath);
    }
    static void FillInfoAboutStudent(ref InteractWithPerson iwp, int type = 1)
    {
        //string name = Regex.Replace(person.GetType().ToString(), @"\w+.(?<name>\w+)", @"${name}");
        string fname, lname, sex, residence, course, studentId, dorm;
        bool isDorm;

    Loop:
        Console.Write($"Enter First Name: ");
        fname = Console.ReadLine();

        Console.Write($"Enter Last Name: ");
        lname = Console.ReadLine();

        Console.Write($"Enter Sex: ");
        sex = Console.ReadLine();


        Console.Write($"Enter Course: ");
        course = Console.ReadLine();

        Console.Write($"Enter Student Id: ");
        studentId = Console.ReadLine();

        Console.Write("Does student live in dorm: ");
        dorm = Console.ReadLine();

        if (dorm.ToLower() == "yes")
            isDorm = true;
        else
            isDorm = false;

        Console.Write($"Enter Residence: ");
        residence = Console.ReadLine();

        try
        {
            iwp.CreateStudent(fname, lname, sex, isDorm, residence, studentId, course);
        }
        catch (MyException e)
        {
            Console.WriteLine(e.Message);
            goto Loop;
        }
        //else
        //{
        //    if (type == 2)
        //    {
        //        string product;
        //        Console.Write($"Enter Product: ");
        //        product = Console.ReadLine();
        //        Console.Write($"Enter Residence: ");
        //        residence = Console.ReadLine();

        //        return iwp.CreateSeller(fname, lname, sex, residence, product);
        //    }
        //    else if (type == 3)
        //    {
        //        string employer;
        //        Console.Write($"Enter Empolyer: ");
        //        employer = Console.ReadLine();
        //        Console.Write($"Enter Residence: ");
        //        residence = Console.ReadLine();

        //        return iwp.CreateGardener(fname, lname, sex, residence, employer);
        //    }

        //}
        //return null;
    }
    #endregion

    #region Delete
    public void Delete() // TODO : first implement in ES then here
    {
        InteractWithPerson iwp = new InteractWithPerson();

        if (!AskForFilePath(ref iwp)) return;
        ShowAll(iwp.FilePath);
        string index = "";
        int number;
        do
        {
            Console.Write("Enter index of who you want delete:");
            index = Console.ReadLine();
            if (int.TryParse(index, out number)) ;


        } while (!iwp.Delete(number - 1));

        ShowAll(iwp.FilePath);
    }
    #endregion

    #region Show
    public override void Show(IEnumerable collection)
    {
        int index = 1;
        foreach (var item in collection)
        {
            Console.WriteLine($"{index++}.{item}\n");
        }
        Console.WriteLine();
    }
    public void ShowAll(string filePath = null)
    {
        InteractWithPerson iwp = new InteractWithPerson();
        if (filePath == null)
            AskForFilePath(ref iwp);
        else
            iwp.FilePath = filePath;

        Show(iwp.GetAll());
    }
    public void Search(string filePath = null)
    {
        InteractWithPerson iwp = new InteractWithPerson();
        if (filePath == null)
        {
            if (!AskForFilePath(ref iwp)) return;
        }
        else
            iwp.FilePath = filePath;

        var res = iwp.Search3CourseInDorm(iwp.FilePath);
        Console.WriteLine($"There is {res.Count} who study in 3rd year and live in dorm: ");

        Show(res);
    }
    #endregion

    #region Auxiliary Methods
    public bool AskForFilePath(ref InteractWithPerson iwp)
    {
        Console.Write("Enter file path:");
        string filePath = Console.ReadLine();
        var file = iwp.SetFilePath(filePath);
        while (!file.Item1)
        {
            Console.WriteLine(file.Item2);
            string input = Console.ReadLine();
            if (input == "/return") return false;

            file = iwp.SetFilePath(input);
        }
        return true;
    }
    public void ClearFile()
    {
        InteractWithPerson iwp = new InteractWithPerson();

        if (!AskForFilePath(ref iwp)) return;

        iwp.Clear();
    }
    #endregion
}