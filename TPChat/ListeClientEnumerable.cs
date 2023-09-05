using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPChat
{
    internal class ListeClientEnumerable : IEnumerator<Client>
    {
        private List<Client> _clients;

        private int _index = 0;

        public ListeClientEnumerable(List<Client> clients)
        {
            _clients = clients;
        }

        public Client Current => _clients[_index];

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {
            
        }

        public bool MoveNext()
        {
            if (_index >= _clients.Count - 1) return false;
            _index = _index==0?0:_index++;
            return true;
        }

        public void Reset()
        {
            _index = 0;
        }
    }
}
