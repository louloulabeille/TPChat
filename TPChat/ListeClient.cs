using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPChat
{
    internal class ListeClient : IEnumerable<Client>
    {
        private ListeClientEnumerable _clients;

        public ListeClient(List<Client> clients)
        {
            _clients = new ListeClientEnumerable(clients);
        }

        public IEnumerator<Client> GetEnumerator()
        {
            return _clients;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
