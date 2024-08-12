using System.Security.Cryptography;
using System.Text;

namespace Infrastructure
{
    public class ConsistentHash
    {
        private readonly SortedDictionary<uint, string> _circle = new SortedDictionary<uint, string>();
        private readonly int _virtualNodes;

        public ConsistentHash(List<string> nodes, int virtualNodes = 100)
        {
            _virtualNodes = virtualNodes;
            foreach (var node in nodes)
            {
                AddNode(node);
            }
        }

        public void AddNode(string node)
        {
            for (int i = 0; i < _virtualNodes; i++)
            {
                _circle[Hash($"{node}:{i}")] = node;
            }
        }

        public string GetNode(string key)
        {
            if (_circle.Count == 0)
                return null;

            var hash = Hash(key);
            var entry = _circle.FirstOrDefault(x => x.Key >= hash);

            return entry.Equals(default(KeyValuePair<uint, string>)) ? _circle.First().Value : entry.Value;
        }

        private static uint Hash(string key)
        {
            using (var md5 = MD5.Create())
            {
                var keyBytes = Encoding.UTF8.GetBytes(key);
                var hashBytes = md5.ComputeHash(keyBytes);
                return BitConverter.ToUInt32(hashBytes, 0);
            }
        }
    }
}
