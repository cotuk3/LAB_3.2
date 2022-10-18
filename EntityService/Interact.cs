using EntityContext;
using System.Text.RegularExpressions;

namespace EntityService;

public class InteractWithPerson
{
	#region Fields
	int _typeOfPerson;
	string _filePath;
	string _extension;
	ArgumentException wrongFile = new ArgumentException("Unknow file extension!");
	static Dictionary<string, Func<string, object>> deser = new Dictionary<string, Func<string, object>>()
	{
		{ ".dat", (filePath) => new BinaryProvider(typeof(List<Person>)).Deserialize(filePath) },
		{ ".xml", (filePath) => new XMLProvider(typeof(List<Student>)).Deserialize(filePath) },
		{ ".json", (filePath) => new JSONProvider(typeof(List<Student>)).Deserialize(filePath) },
		{ ".txt", (filePath) => new CustomProvider(typeof(List<>), typeof(Student)).Deserialize(filePath) }
	};
	static Dictionary<string, Action<object, string>> ser = new Dictionary<string, Action<object, string>>()
	{
		{".dat", (graph, filePath) => new BinaryProvider(typeof(List<Student>)).Serialize(graph, filePath) },
		{".xml", (graph, filePath) => new XMLProvider(typeof(List<Student>)).Serialize(graph, filePath) },
		{".json", (graph, filePath) => new JSONProvider(typeof(List<Student>)).Serialize(graph, filePath)},
		{".txt", (graph, filePath) => new CustomProvider(typeof(List<>), typeof(Student)).Serialize(graph, filePath)}
	};
	#endregion

	#region ctorS
	public InteractWithPerson(string filePath, int typeOfPerson = 0)
	{
		_typeOfPerson = typeOfPerson;
		FilePath = filePath;
	}
	public InteractWithPerson()
	{
	}
	#endregion

	#region Properties
	public string FilePath
	{
		get => _filePath;
		set
		{
			string extension = Path.GetExtension(value);
			if (extension == ".dat" || extension == ".xml" || extension == ".json" || extension == ".txt")
			{
				_filePath = value;
				_extension = Path.GetExtension(_filePath);
			}

			else
				throw wrongFile;
		}
	}
	public (bool, string) SetFilePath(string filePath)
	{
		try
		{
			FilePath = filePath;
			return (true, null);
		}
		catch
		{
			return (false, wrongFile.Message);
		}
	}
	public int TypeOfPerson
	{
		get => _typeOfPerson;
		set => _typeOfPerson = value;
	}

	public List<Student> DefList
	{
		get
		{
			return new List<Student>()
			{
				new Student("Bohdan", "Liashenko", "Male", false, "Odesa", "BL 12345678", "2"),
				new Student("Name", "Surname", "Female", true, "2-222", "NS 12345678", "3"),
				new Student("Someone", "Somebody", "Helicopter", true, "11-111", "NS 88888888", "4"),
			};
		}
	}
	#endregion

	#region Write to File
	public void Add(Student student)
	{
		if (ser.ContainsKey(_extension))
		{
			List<Student> res;
			if (File.Exists(_filePath))
			{
				res = deser[_extension](_filePath) as List<Student>;
				if (res == null)
					res = new List<Student>();
			}
			else
			{
				res = new List<Student>();
			}
			res.Add(student);
			ser[_extension](res, _filePath);
		}
		else
			throw wrongFile;
	}
	public void Add(List<Student> list)
	{
		if (File.Exists(_filePath))
		{
			List<Student> res;
			try
			{
				res = deser[_extension](_filePath) as List<Student>;
			}
			catch
			{
                res = new List<Student>();
            }
			if (res == null)
                res = new List<Student>();

			foreach (var student in res)
				list.Add(student);
        }
		ser[_extension](list, _filePath);
	}
	public bool Delete(int index) // DONE 8:25 p.m 14.10
	{
		if (!File.Exists(_filePath))
			return false;

		List<Student> list = GetAll();

		if (list == null || index >= list.Count || index < 0)
			return false;

		list.RemoveAt(index);
		File.Delete(_filePath);
		Add(list);
		return true;
	}
	public bool Clear()
	{
		return DataProvider.ClearFile(_filePath);
	}
	#endregion

	#region Read From File
	public List<Student> GetAll()
	{
		return deser[_extension](_filePath) as List<Student>;
	}
	public List<Student> Search3CourseInDorm(string filePath)
	{
		string extension = Path.GetExtension(filePath);
		if (deser.ContainsKey(extension))
		{
			var res = (from x in (deser[extension](filePath) as List<Student>)
					   where x.IsLivingInDorm == true && x.Course == "3"
					   select x).ToList();
			return res;
		}
		else
			throw wrongFile;
	}
	#endregion

	#region Interact with Entities
	public bool CreateStudent(string firstName, string lastName, string sex,
		bool isLivingInDorm, string residence, string studentId, string course)
	{
		//Student student = new Student();
        Regex validName = new Regex(@"^[A-Za-z ]+$");
        Regex validId = new Regex(@"^[A-Z]{2}\s\d{8}$");
        Regex validDorm = new Regex(@"^\d{1,2}-\d{3}$");
        Regex validCourse = new Regex("[1-6]");

        string input = "";
		bool create = true;

		if (!validName.IsMatch(firstName))
		{
			input += "First Name, ";
			create = false;
		}

		if (!validName.IsMatch(lastName))
		{
            input += "Last Name, ";
            create = false;
        }

		if (!validName.IsMatch(sex))
		{
            input += "Sex, ";
            create = false;
        }

		if(!validId.IsMatch(studentId))
        {
            input += "Student Id, ";
            create = false;
        }

		if((isLivingInDorm && !validDorm.IsMatch(residence)) || (!isLivingInDorm && !validName.IsMatch(residence)))
        {
            input += "Residence, ";
            create = false;
        }

        if (!validCourse.IsMatch(course))
        {
            input += "Course, ";
            create = false;
        }

		if (create)
		{
			Add(new Student(firstName, lastName, sex, isLivingInDorm, residence, studentId, course));
			return true;
		}
		else
			throw new MyException(input);
	}
	public Gardener CreateGardener(string firstName, string lastName, string sex,
		string residence, string employer)
	{
		return new Gardener(firstName, lastName, sex, residence, employer);
	}
	public Seller CreateSeller(string firstName, string lastName, string sex,
		 string residence, string product)
	{
		return new Seller(firstName, lastName, sex, residence, product);
	}
	#endregion
}