namespace Server.Model
{
    internal class Tarefa
    {
        public string Id { get; set; }
        public string? Description { get; set; }
        public string State { get; set; }
        public string ClienteId { get; set; }

        public Tarefa()
        {
            Description = null;
            State = "Nao alocado";
            ClienteId = null;
        }

        public Tarefa(string id, string desc, string state, string clientId )
        {
            Id = id;
            Description = desc;
            State = state;
            ClienteId = clientId;
        }

        public int FinishTask(string service) // Dá para fazer isto sem o parâmetro ao usar o id da tarefa
        {
            string filePath = @$"D:\Universidade\3ºAno\2º Semestre\Sistemas Distribuidos\SD_Trabalho_Pratico_1\Server\Data\{service}.csv";

            string[] lines = File.ReadAllLines(filePath);

            int lineIndex = Array.FindIndex(lines, line => line.StartsWith($"{Id},"));

            if (lineIndex >= 0)
            {
                string line = lines[lineIndex];

                string[] colunas = line.Split(',');

                colunas[2] = "Concluido";

                State = colunas[2];

                line = string.Join(',', colunas);
                lines[lineIndex] = line; 

                File.WriteAllLines(filePath, lines);

                return 0;
            }

            return -1;
        }

        public int AssignClient(Client client) // Atribuir uma tarefa a um cliente
        {
            if (client.Service.Any() == false) // É preciso alocar o cliente a um serviço
                return -1;

            string filePath = @$"D:\Universidade\3ºAno\2º Semestre\Sistemas Distribuidos\SD_Trabalho_Pratico_1\Server\Data\{client.Service}.csv";

            string[] lines = File.ReadAllLines(filePath);

            int lineIndex = Array.FindIndex(lines, line => line.Contains($",Nao alocado,"));

            if (lineIndex >= 0)
            {
                string line = lines[lineIndex];

                string[] colunas = line.Split(',');

                // Modificar linha
                colunas[2] = "Em curso";
                colunas[3] = client.Id;

                Id = colunas[0];
                Description = colunas[1];
                State = colunas[2];
                ClienteId = colunas[3];

                line = string.Join(',', colunas);
                lines[lineIndex] = line; // Substituir linha

                File.WriteAllLines(filePath, lines);


                return 0;
            }

            return -1; // Se não houverem tarefas disponíveis retorna -1
        }

        public void UpdateTask(string newId, string newDesc, string newState, string newClient)
        {
            Id = newId;
            Description = newDesc;
            State = newState;
            ClienteId = newClient;
        }
    }
}
