using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TpClientChat
{
    internal class ClientChat
    {
        private Socket? Socket { get; set; }  = null;
        public IPEndPoint? IPEndPoint { get; private set; }
        private IPAddress IPAddress { get; set; }  = IPAddress.Loopback ;
        private int Port { get; set; }  = 2345;
        private Guid IdGuid { get; set; } = Guid.NewGuid();
        private string Speudo { get; set; }  = "";

        public ClientChat() { }
        public ClientChat(int port) { Port = port; }
        public ClientChat(IPAddress iPAddress, int port) {
            IPAddress = iPAddress;
            Port = port;
        }

        public async Task RunAsync()
        {
            using (Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                IPEndPoint = new IPEndPoint(IPAddress, Port);
                try
                {
                    NomRecup(); // récupère le speudo de l'utilisateur

                    await Socket.ConnectAsync(IPEndPoint); // connection
                    await Init();

                    Console.WriteLine("-- Chat Message --");

                    // cela marche aussi
                    /*Task task1 = Task.Factory.StartNew(() => EnvoiMessage());
                    Task task2 = Task.Factory.StartNew(() => ReceptionMessage());
                    Task.WaitAll(task1, task2);*/

                    /*Thread threadMessage = new (EnvoiMessage);
                    threadMessage.Start();*/
                    // on créée un thread et après on fait le programme principal derrière
                    Thread threadReception = new(ReceptionMessage);
                    threadReception.IsBackground = true; // si le thread du programme principal s'arrête il arrête aussi ce thread
                    threadReception.Start();

                    EnvoiMessage();

                }
                catch
                {
                    Console.WriteLine($"Erreur au niveau de la connexion du client vers le serveur. Arrêt de l'application.");
                }
                finally
                {
                    await StopAsync();
                }
            }
        }

        /// <summary>
        /// initialisation du client du Chat
        /// </summary>
        /// <returns></returns>
        private async Task Init()
        {
            // envoi du Guid et du speudo au serveur
            string message = IdGuid.ToString();
            byte[] buffer = Encoding.UTF8.GetBytes(message);

            if (Socket is null) throw new SocketException();

            try
            {
                await Socket.SendAsync(buffer);

                Thread.Sleep(5);

                message = Speudo;
                buffer = Encoding.UTF8.GetBytes(message);
                await Socket.SendAsync(buffer);
            }
            catch {
                await StopAsync();
                Console.WriteLine("Impossible de se connecter au serveur. Arrêt de l'application.");
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Récupère le nom de la personne qui se connecte
        /// </summary>
        /// <returns></returns>
        private void NomRecup ()
        {
            string? retour;
            Console.WriteLine("Entrer votre speudo.");
            do
            {
                retour = Console.ReadLine();
            } while (string.IsNullOrEmpty(retour));

            Speudo = retour;
        }

        private async void EnvoiMessage()
        {
            while (true)
            {
                string? message = Console.ReadLine();

                if (Socket is null) throw new SocketException();

                if (!string.IsNullOrEmpty(message))
                {
                    if (message == "q")
                    {
                        await StopAsync();
                        Environment.Exit(0);
                    }

                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    await Socket.SendAsync(buffer);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="SocketException"></exception>
        private async void ReceptionMessage()
        {
            if (Socket is null) throw new SocketException();

            while (true)
            {
                byte[] buffer = new byte[1024];
                string speudo;
                string message;
                try
                {
                    await Socket.ReceiveAsync(buffer);
                    speudo = Encoding.UTF8.GetString(buffer);

                    buffer = new byte[1024]; // initilise la table

                    await Socket.ReceiveAsync(buffer);
                    message = Encoding.UTF8.GetString(buffer);

                    Console.WriteLine($"{speudo}: {message}");
                }
                catch {
                    await StopAsync();
                    Console.WriteLine("Déconnexion du serveur, arrêt de l'application.");
                    Environment.Exit(-1);
                }
                
            }
        }

        /// <summary>
        /// Méthode qui arrête les procedures de connexions
        /// - arrêt du socket et envoi vers le serveur de l'arrêt de la connexion
        /// </summary>
        private async Task StopAsync()
        {
            if (Socket is not null && Socket.Connected)
            {
                await EnvoiArretAsync();
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
            }
        }

        private async Task EnvoiArretAsync()
        {
            const string arret = "[EOC]";   // le best serait faire une structure

            byte[] buffer = Encoding.UTF8.GetBytes(arret);
            if (Socket is null) throw new SocketException();
            await Socket.SendAsync(buffer);
        }
    }
}
