using DemExApp.Data;
using System.Collections.Generic;
using System.Linq;

namespace DemExApp.Models {
    public interface IOrdering {
        IOrderedEnumerable<T> Order<T>(IEnumerable<T> collection, string propertyName) where T : BaseEntity;
    }

    class AZOrder : IOrdering {
        public IOrderedEnumerable<T> Order<T>(IEnumerable<T> collection, string propertyName) where T : BaseEntity {
            return collection.OrderBy(e => e[propertyName]);
        }
    }

    class ZAOrder : IOrdering {
        public IOrderedEnumerable<T> Order<T>(IEnumerable<T> collection, string propertyName) where T : BaseEntity {
            return collection.OrderByDescending(e => e[propertyName]);
        }
    }
}
