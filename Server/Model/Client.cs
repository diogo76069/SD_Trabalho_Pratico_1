namespace Server.Model
{
    //Neste caso os clientes deverão estar guardados num sistema CSV

    internal class Client 
    {
        public required string Id { get; set; }
        public string? Service { get; set; }

        public Client()
        {
            Id = string.Empty;
            Service = string.Empty;
        }

       
        // Criar método para atualizar o serviço, caso não tenha.
    }
}
