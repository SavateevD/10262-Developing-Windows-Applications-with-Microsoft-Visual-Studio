using System.Collections.Generic;

namespace AdventureWorks.Model
{
    public interface IDataAccessService
    {
        IEnumerable<Product> GetProductList();

        IEnumerable<Product> GetProductList(string name);

        IEnumerable<Product> GetProductList(decimal price);

        IEnumerable<Product> GetProductList(int level);

        IEnumerable<WorkOrder> GetWorkOrders(int id);

        IEnumerable<WorkOrderRouting> GetWorkOrderRouting(int id);

        void ServiceDispose();
    }
}
