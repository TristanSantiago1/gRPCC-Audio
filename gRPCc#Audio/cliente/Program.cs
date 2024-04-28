using System;
using System.IO;
using Grpc.Core;
using System.Threading.Tasks;
using Grpc.Net.Client;
using static ServicioAudio;
using System.Media;

class Program
{
    static async Task <MemoryStream> DescargarStreamAsync(ServicioAudioClient stub, string nombre_Archivo)
    {
        using var call = stub.descargarAudio(new PeticionDescargarAudio
        {
            Nombre = nombre_Archivo
        });

        Console.WriteLine($"Recibiendo archivo: {nombre_Archivo}");
        var writeStream = new MemoryStream();
        await foreach (var item in call.ResponseStream.ReadAllAsync())
        {
            if (item.Datos != null)
            {
                var bytes = item.Datos.Memory.ToArray();
                Console.Write(":");
                await writeStream.WriteAsync(bytes);
            }
        }


        Console.WriteLine("Recepcion correcta");
        return writeStream;
    }

    public static void PlayStream(MemoryStream memoryStream, string nombre_archivo)
    {
        if (memoryStream != null)
        {
            Console.WriteLine("\nReproduciendo el archivo " + nombre_archivo);
            SoundPlayer player = new SoundPlayer(memoryStream);
            player.Stream?.Seek(0, SeekOrigin.Begin);
            player.Play();
        }
    }

    static async Task Main(string[] args)
    {
        using var channel = GrpcChannel.ForAddress("http://localhost:9000");
        var stub = new ServicioAudioClient(channel);
        string nombre_archivo = "anyma.wav";
        MemoryStream stream = await DescargarStreamAsync(stub, nombre_archivo);
        PlayStream(stream, nombre_archivo);
        Console.WriteLine("Presione cualquier tecla para salir");
        Console.ReadKey();
        stream.Close();
    }
}

