using BankConsole;
using System.Text.RegularExpressions;

if(args.Length == 0)
    EmailService.SendMail();
else
    ShowMenu();
// //Metodos

void ShowMenu()
{
    Console.Clear();
    Console.WriteLine("Selecciona una opción:");
    Console.WriteLine("1 - Crear un Usuario nuevo.");
    Console.WriteLine("2 - Eliminar un Usuario existente.");
    Console.WriteLine("3 - Salir.");

    int option = 0;
    do{
        string input = Console.ReadLine()!;
        if(!int.TryParse(input, out option))
            Console.WriteLine("Debes ingresar un numero (1, 2 o 3).");
        else if(option>3)
            Console.WriteLine("Debes ingresar un numer valido (1, 2 o 3).");
        
    }while(option == 0 || option > 3);

    switch(option)
    {
        case 1:
            CreateUser();
            break;
        case 2:
            DeleteUser();
            break;
        case 3:
            Environment.Exit(0);
            break;
    }
}

void CreateUser()
{
    Console.Clear();
    Console.WriteLine("Ingresa la información del usuario:");
    int ID;
    bool valid = false;
    do{
        Console.Write("ID: ");
        try{
            ID = int.Parse(Console.ReadLine());
        }
        catch{
            ID = -1;
        }
        if(ID<=0)
            Console.WriteLine("Ingrese un ID valido (Numero positivo mayor a 0).");
        else
        {
            valid = !Storage.IDExists(ID);
            if(!valid){
                Console.WriteLine("Este ID ya esta registrado, intente con otro.");
                ID = -1;
            }
        }

    }while(!valid);

    Console.Write("Nombre: ");
    string name = Console.ReadLine();

    string email = "";
    do{
        Console.Write("Email: ");
        try{
            email = Console.ReadLine();
            string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";
            valid = Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
        }
        catch{
            valid = false;
        }
        if(!valid)
            Console.WriteLine("Por favor ingrese un correo valido.");

    }while(!valid);

    decimal balance;
    do{
        Console.Write("Saldo: ");
        try{
            balance = decimal.Parse(Console.ReadLine());
        }catch{
            balance = -1;
        }
        if(balance<=0)
            Console.WriteLine("El balance debe ser un numero decimal positivo.");
    }while(balance<=0);

    char userType = 'x';
    do{
        Console.Write("Escibre 'c' si el usuario es Cliente, 'e' si es Empleado: ");
        try{
            userType = char.Parse(Console.ReadLine());
        }
        catch{
            valid = false;
        }
        if(userType == 'c' || userType == 'e')
            valid = true;
        else
        {
            Console.WriteLine("Por favor ingrese un caracter valido ('c' o 'e').");
            valid = false;
        }

    }while(!valid);

    User newUser;

    if(userType.Equals('c'))
    {
        Console.Write("Regimen Fiscal: ");
        char taxRegime = char.Parse(Console.ReadLine());

        newUser = new Client(ID, name, email, balance, taxRegime);
    }
    else
    {
        Console.Write("Departamento: ");
        string department = Console.ReadLine();

        newUser = new Employee(ID, name, email, balance, department);
    }

    Storage.AddUser(newUser);

    Console.WriteLine("Usuario creado.");
    Thread.Sleep(2000);
    ShowMenu();
}

void DeleteUser()
{
    if(Storage.IsEmpty()){
        Console.Write("No se han registrado usuarios");
        Thread.Sleep(2000);
        ShowMenu();
    }
    else{
        Console.Clear();

        int ID;
        bool exists = false;
        do{
            Console.Write("Ingrese el ID del usuario a eliminar: ");
            try{
                ID = int.Parse(Console.ReadLine());
            }
            catch{
                ID = -1;
            }
            if(ID<=0)
                Console.WriteLine("Ingrese un ID valido (Numero positivo mayor a 0).");
            else
                exists = Storage.IDExists(ID);
                if(!exists){
                    Console.WriteLine("Este ID no existe, intente con otro.");
                    ID = -1;
            }

        }while(!exists);

        string result = Storage.DeleteUser(ID);

        if(result.Equals("Success"))
        {
            Console.Write("Usuario eliminado");
            Thread.Sleep(2000);
            ShowMenu();
        }
    }
}