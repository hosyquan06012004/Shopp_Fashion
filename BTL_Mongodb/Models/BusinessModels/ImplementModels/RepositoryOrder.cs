// RepositoryOrder.cs
using BTL_Mongodb.Models.Atributes;
using BTL_Mongodb.Models.Data;
using BTL_Mongodb.Models.ViewModel;
using MongoDB.Bson;
using MongoDB.Driver;
using X.PagedList;
using X.PagedList.Extensions;

namespace BTL_Mongodb.Models.BusinessModels.ImplementModels
{
    public class RepositoryOrder : IRepositoryOrder
    {
        private readonly IMongoCollection<Order> _orders;

        public RepositoryOrder(MongoDbContext context)
        {
            _orders = context.Orders;
        }

        public bool Delete(string id)
        {
            var results = _orders.DeleteOne(o => o.Id == id);
            return results.DeletedCount > 0;
        }

        public List<Order> GetAll()
        {
            return _orders.Find(_ => true).ToList();
        }

        public Order GetById(string id)
        {
            return _orders.Find(o => o.Id == id).FirstOrDefault();
        }

        public IPagedList<OrderViewModel> GetFull(int page, int pageSize)
        {
            var orders = _orders.Find(_ => true)
                .SortByDescending(o => o.OrderDate)
                .ToList();

            var data = orders.Select(ConvertToViewModel).ToList();
            return data.ToPagedList(page, pageSize);
        }

        public IPagedList<Order> GetPage(int page, int pageSize)
        {
            return _orders.AsQueryable().ToPagedList(page, pageSize);
        }

        public bool Insert(Order entity)
        {
            try
            {
                _orders.InsertOne(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IPagedList<OrderViewModel> Search(OrderSearchModel searchModel, int pageNumber, int pageSize)
        {
            var builder = Builders<Order>.Filter;
            var filter = builder.Empty;

            // Add filters based on search criteria
            if (!string.IsNullOrEmpty(searchModel.CustomerName))
            {
                filter &= builder.Or(
                    builder.Regex(o => o.FirstName, new BsonRegularExpression(searchModel.CustomerName, "i")),
                    builder.Regex(o => o.LastName, new BsonRegularExpression(searchModel.CustomerName, "i"))
                );
            }

            if (!string.IsNullOrEmpty(searchModel.Email))
            {
                filter &= builder.Regex(o => o.Email, new BsonRegularExpression(searchModel.Email, "i"));
            }

            if (!string.IsNullOrEmpty(searchModel.Phone))
            {
                filter &= builder.Regex(o => o.Phone, new BsonRegularExpression(searchModel.Phone, "i"));
            }

            if (searchModel.FromDate.HasValue)
            {
                filter &= builder.Gte(o => o.OrderDate, searchModel.FromDate.Value);
            }

            if (searchModel.ToDate.HasValue)
            {
                // Add one day to include the entire ToDate day
                var toDate = searchModel.ToDate.Value.AddDays(1);
                filter &= builder.Lt(o => o.OrderDate, toDate);
            }

            if (searchModel.MinAmount.HasValue)
            {
                filter &= builder.Gte(o => o.TotalAmount, searchModel.MinAmount.Value);
            }

            if (searchModel.MaxAmount.HasValue)
            {
                filter &= builder.Lte(o => o.TotalAmount, searchModel.MaxAmount.Value);
            }

            var orders = _orders.Find(filter)
                .SortByDescending(o => o.OrderDate)
                .ToList();

            var data = orders.Select(ConvertToViewModel).ToList();
            return data.ToPagedList(pageNumber, pageSize);
        }

        public bool Update(Order entity)
        {
            var result = _orders.ReplaceOne(o => o.Id == entity.Id, entity);
            return result.ModifiedCount > 0;
        }

        public OrderViewModel ConvertToViewModel(Order order)
        {
            return new OrderViewModel
            {
                Id = order.Id,
                UserId = order.UserId,
                FullName = $"{order.FirstName} {order.LastName}",
                Address = order.Address, // Thêm dòng này
                Phone = order.Phone,
                Email = order.Email,
                Status = order.Status, // Đảm bảo có dòng này
                TotalAmount = order.TotalAmount,
                OrderDate = order.OrderDate,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailViewModel
                {
                    ProductId = od.ProductId,
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    Price = od.Price,
                    Total = od.Total
                }).ToList()
            };
        }
    }
}