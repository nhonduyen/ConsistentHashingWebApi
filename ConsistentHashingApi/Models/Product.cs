﻿namespace ConsistentHashingApi.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; } = true;
    }
}
