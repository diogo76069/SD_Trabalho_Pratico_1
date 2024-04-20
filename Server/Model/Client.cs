namespace Server.Model
{
    
    internal class Client 
    {
        public string Id { get; set; }
        public string? Service { get; set; }

        private static Mutex mutex_cliente = new Mutex();

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

        public Tarefa? FindCurrentTask()
        {
            mutex_cliente.WaitOne();
            // Atenção! Deve ser verificado se o cliente tem serviço alocado antes de usar este método
            string filePath = @$"C:\Users\Pedro\Source\Repos\diogo76069\SD_Trabalho_Pratico_1\Server\Data\{Service}.csv";

            var reader = new StreamReader(filePath);

            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                string[] columns = line.Split(','); // TarefaId,Descricao,Estado,ClienteId

                if (columns[3] == Id) // Verificar todas as linhas com o id do cliente
                {
                    if (columns[2] != "Concluido") // Ignorar as tarefas concluidas
                    {
                        Tarefa currentTask = new Tarefa(columns[0], columns[1], columns[2], columns[3]);
                        mutex_cliente.ReleaseMutex();
                        return currentTask;
                    }
                }
            }

            mutex_cliente.ReleaseMutex();
            return null;
        }

        // Criar método para atualizar o serviço, caso não tenha.
    }
}
