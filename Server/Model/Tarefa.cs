using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Model
{
    internal class Tarefa
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string State { get; set; }
        public Client Cliente { get; set; }

        // Fazer com que estes metodos retornem valores (talvez 1 ou 0)
        // Para verificar que foram corridas normalmente

        public void UpdateTarefa (string newState)
        {
            if (newState != State)
            {
                State = newState;
            }
            return;
        }

        public Tarefa()
        {
            Description = null;
            State = "UNASSIGNED";
            Cliente = null;
        }

        public void AssignClient(Client novoCliente)
        {
            if (Cliente == null) 
            { 
                Cliente = novoCliente;
            }
            return;
        }

        public int UpdateTask(string estado)
        {
            return 0;
        }
    }
}
