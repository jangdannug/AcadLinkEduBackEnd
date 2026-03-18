using Supabase;

namespace AcadLinkEduBackEnd.Infrastructure;

public class SupabaseService
{
    public Client Client { get; }

    public SupabaseService(Client client)
    {
        Client = client;
    }
}
