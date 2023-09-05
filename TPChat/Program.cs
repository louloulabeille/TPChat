// See https://aka.ms/new-console-template for more information
using TPChat;

Console.WriteLine("Hello, World!");

ServeurChat serveurChat = new ServeurChat();
await serveurChat.RunAsync();
