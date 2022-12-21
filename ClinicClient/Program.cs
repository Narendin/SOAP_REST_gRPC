using ClinicServiceNamespace;
using Grpc.Net.Client;
using static ClinicServiceNamespace.ClinicService;

namespace ClinicClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            using var channel = GrpcChannel.ForAddress("http://localhost:5001");

            var clinicServiceClient = new ClinicServiceClient(channel);
            var createClientResponse = clinicServiceClient.CreateClient(new CreateClientRequest()
            {
                Document = "new Document",
                Surname = "Иванов",
                FirstName = "Иван",
                Patronymic = "Иванович"
            });

            if (createClientResponse == null || createClientResponse.ErrCode != 0)
            {
                Console.WriteLine($"Create client error: {createClientResponse?.ErrMessage}");
            } 
            else
            {
                Console.WriteLine($"Client {createClientResponse.ClientId} was created");
            }

            Console.ReadLine();

            var getClientsResponse = clinicServiceClient.GetClients(new GetClientsRequest());

            if (getClientsResponse == null || getClientsResponse.ErrCode != 0)
            {
                Console.WriteLine($"Get clients error: {getClientsResponse?.ErrMessage}");
            }
            else
            {
                foreach (var client in getClientsResponse.Clients)
                    Console.WriteLine($"Client {client.ClientId} {client.Document} {client.Surname} {client.FirstName} {client.Patronymic}");
            }

            Console.ReadLine();
        }
    }
}