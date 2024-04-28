using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication;

public class AudioServicer : ServicioAudio.ServicioAudioBase{
    private const int TAMANO_CHUNK = 1024;

    public override async Task descargarAudio(PeticionDescargarAudio request, IServerStreamWriter<RespuestaChunkAudio> responseStream, ServerCallContext context)
    {
        var buffer = new byte[TAMANO_CHUNK];
        var numBytesRead = 0;
        await using var fileStream = File.OpenRead($"recursos\\{request.Nombre}");

        Console.WriteLine("\n\nEnviado archivo : " +  request.Nombre);
        try
        {
            while ((numBytesRead = await fileStream.ReadAsync(buffer)) > 0 ){
                await responseStream.WriteAsync(new RespuestaChunkAudio {
                    Datos = UnsafeByteOperations.UnsafeWrap(buffer.AsMemory(0, numBytesRead))
                });
                Console.Write(".");
            }
        }
        catch (System.Exception)
        {
        }
    }
}