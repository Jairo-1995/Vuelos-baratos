using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

class Vuelo
{
    public string Origen { get; set; }
    public string Destino { get; set; }
    public string Aerolinea { get; set; }
    public decimal Precio { get; set; }

    public Vuelo(string origen, string destino, string aerolinea, decimal precio)
    {
        Origen = origen;
        Destino = destino;
        Aerolinea = aerolinea;
        Precio = precio;
    }
}

class Program
{
    static void Main()
    {
        // Cargar la base de datos ficticia desde el archivo de texto
        List<Vuelo> vuelos = CargarVuelosDesdeArchivo("vuelos.txt");

        if (vuelos.Count == 0)
        {
            Console.WriteLine("No se pudieron cargar vuelos. Verifique el archivo vuelos.txt");
            return;
        }

        Console.WriteLine($"Base de datos cargada: {vuelos.Count} vuelos disponibles.\n");

        Console.WriteLine("=== BUSCADOR DE VUELOS BARATOS ===\n");

        // Mostrar orígenes disponibles para que el usuario elija
        var origenes = vuelos.Select(v => v.Origen).Distinct().ToList();

        Console.WriteLine("Ciudades de origen disponibles:");
        for (int i = 0; i < origenes.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {origenes[i]}");
        }

        int numOrigen;
        string origen;
        while (true)
        {
            Console.Write("\nSeleccione el origen (número): ");
            string? entrada = Console.ReadLine();
            if (entrada == null) return; // entrada cerrada
            if (int.TryParse(entrada, out numOrigen)
                && numOrigen >= 1 && numOrigen <= origenes.Count)
                break;
            Console.WriteLine("Opción no válida, intente de nuevo.");
        }

        origen = origenes[numOrigen - 1];

        // Mostrar destinos disponibles SOLO desde el origen elegido
        var destinos = vuelos
            .Where(v => v.Origen.Equals(origen, StringComparison.OrdinalIgnoreCase))
            .Select(v => v.Destino)
            .Distinct()
            .ToList();

        Console.WriteLine($"\nDestinos disponibles desde {origen}:");
        for (int i = 0; i < destinos.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {destinos[i]}");
        }

        int numDestino;
        string destino;
        while (true)
        {
            Console.Write("\nSeleccione el destino (número): ");
            string? entrada = Console.ReadLine();
            if (entrada == null) return; // entrada cerrada
            if (int.TryParse(entrada, out numDestino)
                && numDestino >= 1 && numDestino <= destinos.Count)
                break;
            Console.WriteLine("Opción no válida, intente de nuevo.");
        }

        destino = destinos[numDestino - 1];

        Console.WriteLine($"\nBuscando vuelos de {origen} a {destino}...\n");

        Stopwatch tiempo = Stopwatch.StartNew();

        var resultados = vuelos
            .Where(v => v.Origen.Equals(origen, StringComparison.OrdinalIgnoreCase)
                     && v.Destino.Equals(destino, StringComparison.OrdinalIgnoreCase))
            .OrderBy(v => v.Precio)
            .ToList();

        tiempo.Stop();

        Console.WriteLine("\n=== RESULTADOS ===");

        if (resultados.Count == 0)
        {
            Console.WriteLine("No se encontraron vuelos.");
        }
        else
        {
            foreach (var vuelo in resultados)
            {
                Console.WriteLine(
                    $"Origen: {vuelo.Origen} | " +
                    $"Destino: {vuelo.Destino} | " +
                    $"Aerolínea: {vuelo.Aerolinea} | " +
                    $"Precio: ${vuelo.Precio}"
                );
            }

            Console.WriteLine($"\nVuelo más barato:");
            Console.WriteLine($"Aerolínea: {resultados[0].Aerolinea}");
            Console.WriteLine($"Precio: ${resultados[0].Precio}");
        }

        Console.WriteLine($"\nTiempo de ejecución: {tiempo.Elapsed.TotalMilliseconds} ms");

        try
        {
            Console.ReadKey();
        }
        catch (InvalidOperationException)
        {
            // No hay consola interactiva disponible; se ignora.
        }
    }

    // Carga la base de datos ficticia desde un archivo de texto CSV
    // Formato de cada línea: Origen,Destino,Aerolinea,Precio
    static List<Vuelo> CargarVuelosDesdeArchivo(string ruta)
    {
        var vuelos = new List<Vuelo>();

        if (!File.Exists(ruta))
        {
            Console.WriteLine($"No se encontró el archivo '{ruta}'.");
            return vuelos;
        }

        string[] lineas = File.ReadAllLines(ruta);

        foreach (string linea in lineas)
        {
            if (string.IsNullOrWhiteSpace(linea))
                continue;

            string[] partes = linea.Split(',');

            // Se salta el encabezado (Origen,Destino,...) y líneas inválidas
            if (partes.Length != 4)
                continue;

            if (partes[0].Trim().Equals("Origen", StringComparison.OrdinalIgnoreCase))
                continue;

            if (decimal.TryParse(partes[3].Trim(), out decimal precio))
            {
                vuelos.Add(new Vuelo(
                    partes[0].Trim(),
                    partes[1].Trim(),
                    partes[2].Trim(),
                    precio));
            }
        }

        return vuelos;
    }
}
