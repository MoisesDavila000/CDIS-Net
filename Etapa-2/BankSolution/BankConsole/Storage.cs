using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace BankConsole;

public static class Storage{
    static string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\users.json";

    public static void AddUser(User user){
        string json = "", usersInFile = "";

        if(File.Exists(filePath))
            usersInFile = File.ReadAllText(filePath);
        
        var listUsers = JsonConvert.DeserializeObject<List<object>>(usersInFile);

        if(listUsers == null)
            listUsers = new List<object>();
        
        listUsers.Add(user);

        JsonSerializerSettings settings = new JsonSerializerSettings {Formatting = Formatting.Indented};
        
        json = JsonConvert.SerializeObject(listUsers, settings);

        File.WriteAllText(filePath, json);
    }

    public static List<User> GetNewUsers()
    {
        string usersInFile = "";
        var listUsers = new List<User>();

        if(File.Exists(filePath))
            usersInFile = File.ReadAllText(filePath);
        
        var listObjects = JsonConvert.DeserializeObject<List<object>>(usersInFile);

        if(listObjects == null)
            return listUsers;
        
        foreach (object obj in listObjects)
        {
            User newUser;
            JObject user = (JObject)obj;

            if(user.ContainsKey("TaxRegime"))
                newUser = user.ToObject<Client>()!;
            else
                newUser = user.ToObject<Employee>()!;
            
            listUsers.Add(newUser);
        }

        var newUsersList = listUsers.Where(user => user.GetRegisterDate().Date.Equals(DateTime.Today)).ToList();

        return newUsersList;

    }

    public static string DeleteUser(int ID)
    {
        string usersInFile = "";
        var listUsers = new List<User>();

        if(File.Exists(filePath))
            usersInFile = File.ReadAllText(filePath);

        var listObjects = JsonConvert.DeserializeObject<List<object>>(usersInFile);

        if(listObjects == null)
            return "There are no users in the file.";

        foreach (object obj in listObjects)
        {
            User newUser;
            JObject user = (JObject)obj;

            if(user.ContainsKey("TaxRegime"))
                newUser = user.ToObject<Client>();
            else
                newUser = user.ToObject<Employee>();

            listUsers.Add(newUser);
        }

        var userToDelete = listUsers.Where(user => user.GetID() == ID).Single();

        listUsers.Remove(userToDelete);

        JsonSerializerSettings settings = new JsonSerializerSettings {Formatting = Formatting.Indented};

        string json = JsonConvert.SerializeObject(listUsers, settings);

        File.WriteAllText(filePath, json);

        return "Success";
    }

    public static bool IDExists(int ID){
        string usersInFile = "";
        var listUsers = new List<User>();

        if(File.Exists(filePath))
            usersInFile = File.ReadAllText(filePath);

        var listObjects = JsonConvert.DeserializeObject<List<object>>(usersInFile);

        if(listObjects == null)
            return false;
        foreach (object obj in listObjects)
        {
            User newUser;
            JObject user = (JObject)obj;

            if(user.ContainsKey("TaxRegime"))
                newUser = user.ToObject<Client>();
            else
                newUser = user.ToObject<Employee>();

            listUsers.Add(newUser);
        }

        bool exists = listUsers.Any(user => user.GetID() == ID);

        return exists;
    }

    public static bool IsEmpty()
    {
        string usersInFile = "";
        
        if(File.Exists(filePath))
            usersInFile = File.ReadAllText(filePath);

        var listObjects = JsonConvert.DeserializeObject<List<object>>(usersInFile);

        if(listObjects == null)
            return true;
        return false;
    }
}