using MongoDB.Driver;

namespace BTL_Mongodb.Models.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

     
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("categories");
        public IMongoCollection<Brand> Brands => _database.GetCollection<Brand>("brands");
        public IMongoCollection<Product> Products => _database.GetCollection<Product>("products");



        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
        public IMongoCollection<UserRole> UserRoles => _database.GetCollection<UserRole>("UserRoles");
        public IMongoCollection<Role> Roles => _database.GetCollection<Role>("Roles");
        public IMongoCollection<Permission> Permissions => _database.GetCollection<Permission>("Permissions");
        public IMongoCollection<RolePermission> RolePermissions => _database.GetCollection<RolePermission>("RolePermissions");
        public IMongoCollection<Order> Orders => _database.GetCollection<Order>("Orders");
        public IMongoCollection<Banner> Baners => _database.GetCollection<Banner>("baners");

    }
}
