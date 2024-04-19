namespace Server.Model
{
    //Neste caso os clientes deverão estar guardados num sistema CSV

    internal class Client 
    {
        public string Id { get; set; }
        public string? Service { get; set; }

        public Client()
        {
            Id = string.Empty;
            Service = null;
        }

        public Client(string id, string? service)
        {
            Id = id;
            Service = service;
        }


        // Criar método para atualizar o serviço, caso não tenha.
    }
}
