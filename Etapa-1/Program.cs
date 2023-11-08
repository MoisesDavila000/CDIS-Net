//Metodos

int IngresarRetiros(List<int> retiros)
{
    int input=0;
    int cant=0;
    do{
        Console.WriteLine("¿Cuantos retiros se hicieron (máximo 10)?");
        input = Convert.ToInt32(Console.ReadLine());
        if(input<=0 || input>10){
            Console.WriteLine("Numero no valido.\n");
        }
    }while(input<=0 || input>10);

    for(int i=0; i<input; i++)
    {
        do{
            Console.WriteLine($"Ingresa la cantidad del retiro #{i+1}");
            cant = Convert.ToInt32(Console.ReadLine());
            if(cant<0 || cant>50000){
                Console.WriteLine("Cantidad no valida\n");
            }
        }while(cant<0 || cant >50000);
        retiros.Add(cant);
    }

    return input;
}

void DineroPorRetiro(List<int> retiros){
    int cant = 0;
    int billetesCant = 0;
    int monedasCant = 0;
    int[] billetes = {500, 200, 100, 50, 20};
    int[] monedas = {10, 5, 1};
    double value=0;
    for(int i=0; i<retiros.Count; i++){
        cant = retiros[i];
        billetesCant = 0;
        monedasCant = 0;
        //Billetes
        for(int j=0; j<billetes.Length; j++){
            value = cant/billetes[j];
            billetesCant += (int)Math.Floor(value);
            cant = cant - (int)Math.Floor(value)*billetes[j];
        }

        //Monedas
        for(int j=0; j<monedas.Length; j++){
            value = cant/monedas[j];
            monedasCant += (int)Math.Floor(value);
            cant = cant - (int)Math.Floor(value)*monedas[j];
        }

        Console.WriteLine($"\nRetiro #{i+1}:\nBilletes entregados: {billetesCant}\nMonedas entregadas: {monedasCant}");
    }
    Console.WriteLine(@$"Presiona 'enter' para continuar...");

    while(Console.ReadKey().Key != ConsoleKey.Enter){}
}

//Codigo

bool activo = true;
int input = 0;
var retiros = new List<int>();
int numRetiros = 0;

do{
    Console.WriteLine("\n----------Banco CDIS----------");
    Console.WriteLine("1. Ingresar la cantidad de retiros hechos por los usuarios.");
    Console.WriteLine("2. Revisar la cantidad entregada de billetes y monedas.");
    Console.WriteLine("3. Salir.");
    try{
        input = Convert.ToInt32(Console.ReadLine());
    }
    catch{
        input=0;
    }

    if(input == 1){
        numRetiros += IngresarRetiros(retiros);
    }
    else if(input == 2){
        if(numRetiros>0){
            DineroPorRetiro(retiros);
        }
        else{
            Console.WriteLine("No se han ingresado retiros.\n");
        }
    }
    else if(input == 3){
        activo = false;
    }

}while(activo);