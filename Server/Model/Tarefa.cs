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

        public void UpdateState (string newState)
        {
            if (newState != State)
            {
                State = newState;
            }
            return;
        }

        public void AssignClient(string novoId)
        {
            if (ClienteId == null) 
            { 
                ClienteId = novoId;
            }
            return;
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
