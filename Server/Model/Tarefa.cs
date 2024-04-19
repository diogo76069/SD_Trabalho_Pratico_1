using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Model
{
    internal class Tarefa
    {
        public string Id { get; set; }
        public string? Description { get; set; }
        public string State { get; set; }
        public Client Cliente { get; set; }

        public Tarefa()
        {
            Description = null;
            State = "Nao alocado";
            Cliente = null;
        }


        public void UpdateState (string newState)
        {
            if (newState != State)
            {
                State = newState;
            }
            return;
        }

        public void AssignClient(Client novoCliente)
        {
            if (Cliente == null) 
            { 
                Cliente = novoCliente;
            }
            return;
        }

        public void UpdateTask(string newId, string newDesc, string newState, Client newClient)
        {
            Id = newId;
            Description = newDesc;
            State = newState;
            Cliente = newClient;
        }
    }
}
