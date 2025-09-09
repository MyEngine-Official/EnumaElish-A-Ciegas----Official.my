using System.Numerics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using IA.Models;

while (true)
{
    string path = @"C:\\Users\\Usuario\\Desktop\\EnumaElish_A CIEGAS\\IA\\training.json";
    string dataReferencia = await File.ReadAllTextAsync(path);

    JsonArray jsonArray = JsonNode.Parse(dataReferencia).AsArray();


    List<JsonTrainer> data = jsonArray.Select(node => node.Deserialize<JsonTrainer>()).ToList();



    Dictionary<string, Vector2> clasificadorPalabras = new Dictionary<string, Vector2>();


    foreach (var item in data)
    {
        var tokensDataset = Regex.Replace(item.mensaje.ToLowerInvariant(), @"[^\p{L}\p{Nd}]+", " ")
                         .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var mensaje in tokensDataset)
        {
            if (clasificadorPalabras.ContainsKey(mensaje))
            {
                Vector2 probabilidad = clasificadorPalabras[mensaje];
                probabilidad.X += 1;

                if (item.ofensivo)
                {
                    probabilidad.Y += 1;
                }

                clasificadorPalabras[mensaje] = probabilidad;

            }
            else
            {
                if (item.ofensivo)
                {
                    clasificadorPalabras[mensaje] = new Vector2(1, 1);
                }
                else
                {
                    clasificadorPalabras[mensaje] = new Vector2(1, 0);
                }
            }
        }
    }

    Console.WriteLine("Ingrese el mensaje a evaluar:");
    string mensajeEvaluar = Console.ReadLine() ?? "";

    Vector2 probabilidadOfensivo = new Vector2(0f, 0f);


    List<string> palabrasNoEncontradas = new List<string>();
    List<string> palabrasEncontradas = new List<string>();
    var tokens = Regex.Replace(mensajeEvaluar.ToLowerInvariant(), @"[^\p{L}\p{Nd}]+", " ")
                  .Split(' ', StringSplitOptions.RemoveEmptyEntries);
    int cantidadDePalabras = tokens.Length;
    var dataNorm = data.Select(x => new {
        Original = x,
        Normal = Regex.Replace(x.mensaje.ToLowerInvariant(), @"[^\p{L}\p{Nd}]+", " ").Trim()
    }).ToList();
    foreach (var palabra in tokens)
    {

        if (!clasificadorPalabras.ContainsKey(palabra))
        {
            palabrasNoEncontradas.Add(palabra);
        }
        else
        {
            clasificadorPalabras.TryGetValue(palabra, out Vector2 probabilidad);
            probabilidadOfensivo.X += 1;
            probabilidadOfensivo.Y += (probabilidad.Y / probabilidad.X);

            palabrasEncontradas.Add(palabra);




            //Console.WriteLine("¿Quieres ver las coincidencias encontradas? (s/n)");
            //bool verCoincidencias = Console.ReadLine()?.ToLower() == "s";

            for (int n = 2; n <= cantidadDePalabras; n++)
            {
                for (int start = 0; start + n <= cantidadDePalabras; start++)
                {
                    string seccionEvaluar = string.Join(' ', tokens, start, n);

                    var coincidencias = dataNorm
                        .Where(d => d.Normal.Contains(seccionEvaluar))
                        .Select(d => d.Original);

                    foreach (var coincidencia in coincidencias)
                    {
                        //if (verCoincidencias)
                        //{
                        //    Console.WriteLine(coincidencia.mensaje);
                        //}

                        probabilidadOfensivo.X += n;
                        if (coincidencia.ofensivo)
                        {
                            probabilidadOfensivo.Y += n;
                        }
                    }
                }
            }

            ;
        }
    }

    float resultadoFinal = probabilidadOfensivo.X != 0 ? probabilidadOfensivo.Y / probabilidadOfensivo.X : 0;

    Console.WriteLine($"-----------------------------------------------------");

    Console.WriteLine(resultadoFinal > 0.6 ? "El mensaje es Ofensivo" :"Es un mensaje educado");
    Console.WriteLine($"Estoy en lo correcto? ( s | n )");
    string respuesta = Console.ReadLine() ?? "n";

    if (respuesta.ToLower() == "s")
    {
        bool esOfensivo = resultadoFinal > 0.6;
        JsonTrainer nuevoDato = new JsonTrainer
        {
            mensaje = mensajeEvaluar,
            ofensivo = esOfensivo
        };
        data.Add(nuevoDato);
        
    }
    else
    {
        bool esOfensivo = resultadoFinal > 0.6;
        JsonTrainer nuevoDato = new JsonTrainer
        {
            mensaje = mensajeEvaluar,
            ofensivo = !esOfensivo
        };
        data.Add(nuevoDato);
        
    }
    string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true });
    await File.WriteAllTextAsync(path, jsonString);
    Console.WriteLine($"-----------------------------------------------------");
    Console.WriteLine("Quieres ver la probabilidad? ( s | n )");
        string verProbabilidad = Console.ReadLine() ?? "n";
    if (verProbabilidad.ToLower() == "s")
    {
        Console.WriteLine($"Probabilidad de ser ofensivo: {resultadoFinal * 100}%");
    }
    Console.WriteLine($"-----------------------------------------------------");
    Console.WriteLine("Quieres evaluar otro mensaje? ( s | n )");

    if ((Console.ReadLine() ?? "n").ToLower() == "s")
    {
        Console.Clear();
    }
    else
    {
        Console.Clear();
        return;
    }
}