using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TPChat
{
    internal class ServeurChat
    {
        private Socket? Socket;
        public IPEndPoint? IPEndPoint { get; private set; }
        private IPAddress IPAddress = IPAddress.Any;
        private int Port = 2345;

        private List<Client> ListeSocket { get; set; } = new List<Client>();

        public ServeurChat()
        {
            //Socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            //IdEndPoint = new IPEndPoint(IPAddress,Port);
        }
        public ServeurChat(int port)
        {
            Port = port;
            //Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IdEndPoint = new IPEndPoint(IPAddress, Port);
        }

        public ServeurChat(IPAddress iPAddress, int port)
        {
            IPAddress = iPAddress;
            Port = port;

            // initialisation du socket et de l'adresse des écoutes du serveur
            //Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IdEndPoint = new IPEndPoint(IPAddress, Port);
        }

        // lancement du serveur
        public async Task RunAsync()
        {
            using (Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                IPEndPoint = new IPEndPoint(IPAddress, Port);
                try
                {
                    Socket.Bind(IPEndPoint);
                    Socket.Listen();

                    while (true)
                    {
                        Socket? received = null;
                        try
                        {
                            received = await Socket.AcceptAsync();
                            //ClientAsync(received);
                            var thread = new Thread(ClientAsync);
                            thread.Start(received);

                            //Console.WriteLine("Attente.");
                        }
                        catch
                        { // gestion erreur avec le client
                            Console.WriteLine("Problème de connexion avec le client.");
                            if (received is not null && received.Connected)
                            {
                                received.Shutdown(SocketShutdown.Both);
                                received.Close();
                            }
                        }
                    }
                }
                catch
                {
                    Console.WriteLine($"Le serveur n'arrive pas à se connecter.");
                }
                finally
                {
                    if (Socket.Connected) Socket.Shutdown(SocketShutdown.Both);
                    Socket.Close();
                }
            }
        }

        private async Task<Client?> ClientInitAsync(object? socket)
        {
            string message;
            byte[] buffer = new byte[1024];
            Guid id;

            // cast object en type Socket
            if (socket is Socket soc)
            {
                int nbByte = await soc.ReceiveAsync(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, nbByte);

                id = Guid.Parse(message);

                buffer = new byte[1024];

                nbByte = await soc.ReceiveAsync(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, nbByte);

                Client cli = new Client(soc, id, message);

                ListeSocket.Add(cli);
                Console.WriteLine(message + " " + id.ToString() );
                return cli;
            }
            
            return null;
        }

        private async void ClientAsync(object? socket)
        {
            string message;
            var cli = await ClientInitAsync(socket);

            while (true)
            {
                byte[] buffer = new byte[1024];
                if (cli is null) { return ; }
                // gestion erreur pour le client pour empêcher de down le serveur
                try
                {
                    int nbByte = await cli.ClientSocket.ReceiveAsync(buffer);
                    message = Encoding.UTF8.GetString(buffer, 0, nbByte);
                    if (message == "[EOC]")
                    {
                        ListeSocket.Remove(cli);
                        return;
                    }
                    Console.WriteLine(message);
                    EnvoiClients(cli, message);
                    
                }
                catch
                {
                    Console.WriteLine($"{cli.Id} & {cli.Nom} problème de connexion avec le serveur.");
                    ListeSocket.Remove(cli);
                    return; // on tue le thread
                }
            }
        }

        /// <summary>
        /// Méthode qui re-envoi le message envoyé par un client à tous les autres clients
        /// </summary>
        /// <returns></returns>
        private void EnvoiClients(Client cli, string message)
        {
            foreach(Client client in ListeSocket)
            {
                byte[] buffer = new byte[1024];

                int nbBuffer;
                if (client.Id.Equals(cli.Id)) continue;

                try
                {
                    buffer = Encoding.UTF8.GetBytes(cli.Nom);
                    nbBuffer = client.ClientSocket.Send(buffer);

                    buffer = new byte[1024]; // initilise la table

                    Console.WriteLine(message + " " + cli.Nom + " " + nbBuffer);

                    buffer = Encoding.UTF8.GetBytes(message);
                    client.ClientSocket.Send(buffer);
                }
                catch
                {
                    ListeSocket.Remove(cli);
                }
                
            }
        }
    }
}
