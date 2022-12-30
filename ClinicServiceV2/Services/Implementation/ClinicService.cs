using ClinicService.Core.Entities;
using ClinicService.Data.EF;
using ClinicServiceNamespace;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using static ClinicServiceNamespace.ClinicService;

namespace ClinicServiceV2.Services.Implementation
{
    public class ClinicService: ClinicServiceBase
    {
        private readonly ApplicationContext _context;

        public ClinicService(ApplicationContext context)
        {
            _context = context;
        }

        public override async Task<CreateClientResponse> CreateClient(CreateClientRequest request, ServerCallContext context)
        {
            try
            {
                var client = new Client()
                {
                    Document = request.Document,
                    SurName = request.Surname,
                    FirstName = request.FirstName,
                    Patronymic = request.Patronymic
                };

                await _context.Clients.AddAsync(client);
                await _context.SaveChangesAsync();

                return new CreateClientResponse()
                {
                    ClientId = client.Id.ToString(),
                    ErrCode = 0,
                    ErrMessage = string.Empty
                };
            }
            catch (Exception ex)
            {
                return new CreateClientResponse()
                {
                    ErrCode = 1001,
                    ErrMessage = ex.Message
                };
            }
        }

        public override async Task<GetClientsResponse> GetClients(GetClientsRequest request, ServerCallContext context)
        {
            try
            {
                var clients = await _context.Clients
                    .Select(c => new ClientResponse()
                    {
                        ClientId = c.Id.ToString(),
                        Document = c.Document,
                        Surname = c.SurName,
                        FirstName = c.FirstName,
                        Patronymic = c.Patronymic
                    })
                    .ToListAsync();

                var response = new GetClientsResponse()
                {
                    ErrCode = 0,
                    ErrMessage = string.Empty
                };

                response.Clients.AddRange(clients);

                return response;
            }
            catch (Exception ex)
            {
                return new GetClientsResponse()
                {
                    ErrCode = 1002,
                    ErrMessage = ex.Message
                };
            }
        }
    }

}
