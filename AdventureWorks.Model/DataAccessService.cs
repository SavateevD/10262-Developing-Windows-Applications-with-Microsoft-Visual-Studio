using System.Collections.Generic;
using System.Linq;

namespace AdventureWorks.Model
{
    public class DataAccessService : IDataAccessService
    {
        private AdventureWorksEntities entities = null;

        #region Constructor

        public DataAccessService(string connectionString)
            : this(connectionString, null)
        {
        }

        public DataAccessService(string connectionString, AdventureWorksEntities repository)
        {
            this.entities = repository ?? new AdventureWorksEntities(connectionString);
        }

        #endregion

        #region IDataAccessService Members

        public IEnumerable<Product> GetProductList()
        {
            var query = from p in entities.Products
                        select p;
            return query.ToList<Product>();
        }

        public IEnumerable<Product> GetProductList(string name)
        {
            var query = from p in entities.Products
                        where p.Name == name
                        select p;
            return query.ToList<Product>();
        }

        public IEnumerable<Product> GetProductList(decimal price)
        {
            var query = from p in entities.Products
                        where p.ListPrice <= price
                        select p;
            return query.ToList<Product>();
        }

        public IEnumerable<Product> GetProductList(int level)
        {
            var query = from p in entities.Products
                        where p.SafetyStockLevel <= level
                        select p;
            return query.ToList<Product>();
        }

        public IEnumerable<WorkOrder> GetWorkOrders(int id)
        {
            var query = from w in entities.WorkOrders
                        where w.ProductID == id
                        select w;
            return query.ToList<WorkOrder>();
        }

        public IEnumerable<WorkOrderRouting> GetWorkOrderRouting(int id)
        {
            var query = from r in entities.WorkOrderRoutings
                        where r.WorkOrderID == id
                        select r;
            return query.ToList<WorkOrderRouting>();
        }

        public void ServiceDispose()
        {
            this.entities.Dispose();
        }

        #endregion
    }
}


