using System;
using System.Linq;
using System.Data.Entity;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class Program
{
    public static void Main()
    {
        try
        {
            //Defino zona horaria (GMT -3)
            TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

            int yearOfBirth = 2000;
            using (var ctx = new DatabaseContext())
            {
                Console.WriteLine("Inicializando base de datos...");
                InitializeDB(ctx);
                Console.Clear();

                //Ejemplo de impresion
                //foreach (var purchase in ctx.Purchases)
                //{
                //    Console.WriteLine(purchase.PurchaseId.ToString());
                //}

                var youngCustomers = ctx.Customers.Where(customer => customer.DateOfBirth.Year < yearOfBirth).ToList();
                foreach (var youngCustomer in youngCustomers)
                {
                    //Convierto la fecha de UTC a la zona horaria deseada
                    DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(youngCustomer.DateOfBirth, targetTimeZone);

                    Console.WriteLine($"{youngCustomer.FullName}  (Edad: {CalcularEdad(localDateTime)} años)");
                    //Filtro por CustomerId y obtengo todas las compras
                    var purchases = ctx.Purchases
                        .Where(purchase => purchase.CustomerId == youngCustomer.CustomerId)
                        .ToList();

                    //Ordeno las compras de más reciente a más antigua (ignorando la hora)
                    var orderedPurchases = purchases
                        .OrderByDescending(purchase => purchase.PurchaseDateUTC.Date) //Ordeno por fecha ignorando la hora
                        .ThenByDescending(purchase => purchase.Total) //Ordeno por el total (mayor importe primero)
                        .ToList();

                    var cont = 1;
                    var footerLenght = 0;
                    //Imprimo los resultados con el formato regional y la zona horaria convertida
                    foreach (var purchase in orderedPurchases)
                    {
                        //Convierto la fecha de UTC a la zona horaria deseada
                        localDateTime = TimeZoneInfo.ConvertTimeFromUtc(purchase.PurchaseDateUTC, targetTimeZone);

                        string PurchaseId = purchase.PurchaseId.ToString();
                        string PurchaseDate = localDateTime.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        string PurchaseTotal = Convert.ToInt32(purchase.Total).ToString();

                        string plantillaInforme = "{0} ----- $\t{1,7}";

                        string informeGenerado = string.Format(plantillaInforme, PurchaseDate, PurchaseTotal);

                        if (cont == 1)
                        {
                            footerLenght = informeGenerado.Length + 5;
                            Console.WriteLine(new string('=', footerLenght));
                        }
                        Console.WriteLine(informeGenerado);

                        cont++;
                    }

                    Console.WriteLine(new string('=', footerLenght));

                    //TOTAL
                    int sumaTotal = Convert.ToInt32(orderedPurchases.Sum(purchase => purchase.Total));
                    string itemTotal = "TOTAL: $ {0}\n\n";
                    Console.WriteLine(string.Format(itemTotal, sumaTotal.ToString()));
                }

                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
    }

    public static void InitializeDB(DatabaseContext ctx)
    {
        if (ctx.Customers.Count() == 0)
        {
            ctx.Customers.Add(new Customer() { CustomerId = 1, FullName = "Sanchez Mario", DateOfBirth = new DateTime(1985, 10, 18) });
            ctx.Customers.Add(new Customer() { CustomerId = 2, FullName = "Gimenez Pedro", DateOfBirth = new DateTime(2010, 01, 10) });
            ctx.Customers.Add(new Customer() { CustomerId = 3, FullName = "Gomez Ricardo", DateOfBirth = new DateTime(1993, 11, 25) });
            ctx.Customers.Add(new Customer() { CustomerId = 4, FullName = "Araujo María", DateOfBirth = new DateTime(2009, 12, 2) });
    
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1001, PurchaseDateUTC = new DateTime(2021, 2, 2, 15, 22, 35), Total = 255m, CustomerId = 1 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1002, PurchaseDateUTC = new DateTime(2021, 2, 7, 12, 07, 45), Total = 888m, CustomerId = 3 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1003, PurchaseDateUTC = new DateTime(2021, 2, 9, 9, 00, 10), Total = 672m, CustomerId = 1 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1004, PurchaseDateUTC = new DateTime(2021, 1, 2, 10, 12, 32), Total = 1000m, CustomerId = 1 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1005, PurchaseDateUTC = new DateTime(2021, 1, 4, 2, 25, 55), Total = 56m, CustomerId = 2 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1006, PurchaseDateUTC = new DateTime(2021, 1, 7, 3, 12, 57), Total = 75m, CustomerId = 2 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1007, PurchaseDateUTC = new DateTime(2021, 1, 12, 1, 17, 12), Total = 987m, CustomerId = 3 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1008, PurchaseDateUTC = new DateTime(2021, 1, 15, 8, 55, 00), Total = 12000m, CustomerId = 3 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1009, PurchaseDateUTC = new DateTime(2021, 1, 25, 10, 43, 10), Total = 1m, CustomerId = 3 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1010, PurchaseDateUTC = new DateTime(2021, 2, 2, 17, 32, 22), Total = 100m, CustomerId = 4 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1011, PurchaseDateUTC = new DateTime(2021, 2, 2, 15, 22, 35), Total = 256m, CustomerId = 1 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1012, PurchaseDateUTC = new DateTime(2021, 2, 7, 12, 07, 45), Total = 887m, CustomerId = 3 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1013, PurchaseDateUTC = new DateTime(2021, 2, 9, 9, 00, 10), Total = 673m, CustomerId = 1 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1014, PurchaseDateUTC = new DateTime(2021, 1, 12, 1, 17, 12), Total = 987m, CustomerId = 3 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1015, PurchaseDateUTC = new DateTime(2021, 1, 15, 8, 55, 00), Total = 12000m, CustomerId = 3 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1016, PurchaseDateUTC = new DateTime(2021, 1, 25, 10, 43, 10), Total = 1m, CustomerId = 3 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1017, PurchaseDateUTC = new DateTime(2021, 1, 25, 12, 43, 10), Total = 111m, CustomerId = 3 });
            ctx.Purchases.Add(new Purchase() { PurchaseId = 1018, PurchaseDateUTC = new DateTime(2021, 1, 25, 16, 43, 10), Total = 10m, CustomerId = 3 });
            ctx.SaveChanges();
        }

    }

    // Método para calcular la edad a partir de la fecha de nacimiento
    private static int CalcularEdad(DateTime fechaNacimiento)
    {
        DateTime ahora = DateTime.Today;
        int edad = ahora.Year - fechaNacimiento.Year;

        // Verificar si aún no ha pasado el cumpleaños de este año
        if (fechaNacimiento > ahora.AddYears(-edad))
        {
            edad--;
        }

        return edad;
    }
}

public class Purchase
{
    public int PurchaseId { get; set; }
    public DateTime PurchaseDateUTC { get; set; }
    public Decimal Total { get; set; }
    public int CustomerId { get; set; }
    public virtual Customer Customer { get; set; }
}

public class Customer
{
    public int CustomerId { get; set; }
    public string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
}

public class DatabaseContext : DbContext
{
    public DatabaseContext() : base()
    {

    }

    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<Customer> Customers { get; set; }
}
