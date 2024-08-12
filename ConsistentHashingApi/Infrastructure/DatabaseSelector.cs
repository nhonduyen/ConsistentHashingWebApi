namespace ConsistentHashingApi.Infrastructure
{
    public class DatabaseSelector
    {
        private readonly ConsistentHash _consistentHash;

        public DatabaseSelector(List<string> databaseServers)
        {
            _consistentHash = new ConsistentHash(databaseServers);
        }

        public string GetDatabaseForTenant(string tenantId)
        {
            return _consistentHash.GetNode(tenantId);
        }
    }
}
