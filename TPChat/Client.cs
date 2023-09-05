using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TPChat
{
    internal class Client
    {
        public Socket ClientSocket { get; set; }
        public Guid Id { get; set; }
        public string Nom { get; set; }

        public Client(Socket clientSocket, Guid id,string nom)
        {
            ClientSocket = clientSocket;
            Id = id;
            Nom = nom;
        }

    }
}
