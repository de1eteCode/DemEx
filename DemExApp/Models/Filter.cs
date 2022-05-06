using DemExApp.Data;

namespace DemExApp.Models
{
    public class Filter
    {
        public Filter(string property)
        {
            Property = property;
        }

        public string Property { get; set; }

        public bool IsEqual<T>(T entity, string seacrh)
            where T : BaseEntity
        {
            if (entity is null || string.IsNullOrEmpty(seacrh))
            {
                return true;
            }

            return entity[Property]
                .ToString()
                .ToLower()
                .Contains(seacrh.ToLower());
        }
    }
}
