using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using TourismMallMS.Services;

namespace TourismMallMS.Helper
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> ApplySort<T>(
            this IQueryable<T> source,
            string orderBy,
            Dictionary<string, PropertyMappingValue> mappingDictionary
        )
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (mappingDictionary == null)
            {
                throw new ArgumentNullException("mappingDictionary");
            }

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            var orderByString = string.Empty;

            var orderByAfterSplit = orderBy.Split(',');

            foreach (var order in orderByAfterSplit)
            {
                var trimmedOrder = order.Trim();

                var orderDescending = trimmedOrder.EndsWith(" desc");

                var indexOfFirstSpace = trimmedOrder.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1
                    ? trimmedOrder
                    : trimmedOrder.Remove(indexOfFirstSpace);

                if (!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"Key mapping for {propertyName} is missing");
                }

                var propertyMappingValue = mappingDictionary[propertyName];
                if (propertyMappingValue == null)
                {
                    throw new ArgumentNullException("propertyMappingValue");
                }

                foreach (var destinationProperty in
                    propertyMappingValue.DestinationProperties)
                {
                    orderByString = orderByString +
                        (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ")
                        + destinationProperty
                        + (orderDescending ? " descending" : " ascending");
                }
            }
            return source.OrderBy(orderByString);
        }
    }
}