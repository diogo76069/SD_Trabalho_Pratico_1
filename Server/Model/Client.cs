namespace Server.Model
{
    //Neste caso os clientes deverão estar guardados num sistema CSV

    internal class Client 
    {
        public required string Id { get; set; }
        public string? Service { get; set; }

        // Criar construtor

        // Criar método para atualizar o serviço, caso não tenha.
    }
}
