using Flex.Shared.SeedWork;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Linq.Dynamic.Core;
using AutoMapper;

namespace Flex.Infrastructure.EF
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }

        public static async Task<Flex.Shared.SeedWork.PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            PagingRequest request)
            where T : class
        {
            var totalItems = await query.CountAsync();

            // Apply default sorting if "CreatedDate" exists
            var createdDateProperty = typeof(T).GetProperty("CreatedDate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (createdDateProperty != null && string.IsNullOrWhiteSpace(request.OrderBy))
            {
                query = query.OrderByDescending(e => Microsoft.EntityFrameworkCore.EF.Property< DateTime>(e, "CreatedDate"));
            }
            else if (!string.IsNullOrWhiteSpace(request.OrderBy))
            {
                var orderByMappings = GetOrderByMappings(request);
                var mappedOrderBy = orderByMappings.TryGetValue(request.OrderBy.ToLower(), out var mappedValue)
                    ? mappedValue
                    : request.OrderBy;

                var sorting = $"{mappedOrderBy} {request.SortBy}".Trim();
                query = query.OrderBy(sorting);
            }

            if (!request.PageIndex.HasValue) request.PageIndex = 1;
            if (!request.PageSize.HasValue || request.PageSize.Value <= 0) request.PageSize = totalItems;

            var items = await query
                .Skip((request.PageIndex.Value - 1) * request.PageSize.Value)
                .Take(request.PageSize.Value)
                .ToListAsync();

            return Flex.Shared.SeedWork.PagedResult<T>.Create(
                request.PageIndex.Value,
                request.PageSize.Value,
                totalItems,
                items,
                request.OrderBy,
                request.SortBy
            );
        }

        public static Flex.Shared.SeedWork.PagedResult<TDestination> MapPagedResult<TSource, TDestination>(
            this Flex.Shared.SeedWork.PagedResult<TSource> source,
            IMapper mapper)
        {
            var mappedItems = mapper.Map<List<TDestination>>(source.Items);

            return Flex.Shared.SeedWork.PagedResult<TDestination>.Create(
                source.PageIndex,
                source.PageSize,
                source.TotalItems,
                mappedItems,
                source.OrderBy,
                source.SortBy
            );
        }

        //public static async Task<Flex.Shared.SeedWork.PagedResult<T>> ToPagedResultAsync<T>(
        //    this IQueryable<T> query,
        //    PagingRequest request)
        //    where T : EntityAuditBase<object>
        //{
        //    var totalItems = await query.CountAsync();

        //    // Lấy OrderByMappings từ request
        //    var orderByMappings = GetOrderByMappings(request);

        //    // Sắp xếp mặc định theo thuộc tính "CreatedDate" nếu có
        //    var createdDateProperty = typeof(T).GetProperty("CreatedDate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        //    if (createdDateProperty != null && string.IsNullOrWhiteSpace(request.OrderBy))
        //    {
        //        query = query.OrderByDescending(e => ((EntityAuditBase<object>)(object)e).CreatedDate);
        //    }
        //    else if (!string.IsNullOrWhiteSpace(request.OrderBy))
        //    {
        //        // Ánh xạ OrderByMappings
        //        var mappedOrderBy = orderByMappings.TryGetValue(request.OrderBy.ToLower(), out var mappedValue)
        //            ? mappedValue
        //            : request.OrderBy;

        //        var sorting = $"{mappedOrderBy} {request.SortBy}".Trim();
        //        query = query.OrderBy(sorting);
        //    }

        //    // Gán giá trị mặc định cho PageIndex và PageSize
        //    if (!request.PageIndex.HasValue) request.PageIndex = 1;
        //    if (!request.PageSize.HasValue || request.PageSize.Value <= 0) request.PageSize = totalItems;

        //    // Phân trang
        //    var items = await query
        //        .Skip((request.PageIndex.Value - 1) * request.PageSize.Value)
        //        .Take(request.PageSize.Value)
        //        .ToListAsync();

        //    // Trả về kết quả
        //    return Flex.Shared.SeedWork.PagedResult<T>.Create(
        //        request.PageIndex.Value,
        //        request.PageSize.Value,
        //        totalItems,
        //        items,
        //        request.OrderBy,
        //        request.SortBy
        //    );
        //}


        //public static async Task<Flex.Shared.SeedWork.PagedResult<T>> ToPageResultAsync<T, TKey>(this IQueryable<T>? query, PagingRequest request) where T : EntityAuditBase<TKey>
        //{
        //    var totalItems = await query.CountAsync();

        //    // Access OrderByMappings using reflection
        //    var orderByMappings = GetOrderByMappings(request);

        //    // Set default order if applicable
        //    if (string.IsNullOrWhiteSpace(request.OrderBy) && typeof(EntityBase<TKey>).IsAssignableFrom(typeof(T)))
        //    {
        //        query = query.OrderByDescending(e => ((EntityAuditBase<TKey>)(object)e).CreatedDate);
        //    }

        //    if (!string.IsNullOrWhiteSpace(request.OrderBy))
        //    {
        //        var mappedOrderBy = orderByMappings.TryGetValue(request.OrderBy.ToLower(), out var mappedValue)
        //                            ? mappedValue
        //                            : request.OrderBy;
        //        var sorting = $"{mappedOrderBy} {request.SortBy}".Trim();
        //        query = query.OrderBy(sorting);
        //    }

        //    if (string.IsNullOrWhiteSpace(request.OrderBy) || orderByMappings.Count == 0)
        //    {
        //        request.OrderBy = default;
        //        request.SortBy = default;
        //    }

        //    if (!request.PageIndex.HasValue) request.PageIndex = 1;
        //    if (!request.PageSize.HasValue) request.PageSize = totalItems;

        //    var items = await query.Skip((request.PageIndex.Value - 1) * request.PageSize.Value)
        //        .Take(request.PageSize.Value)
        //        .ToListAsync();

        //    return Flex.Shared.SeedWork.PagedResult<T>.Create(
        //        request.PageIndex.Value,
        //        request.PageSize.Value,
        //        totalItems,
        //        items,
        //        request.OrderBy,
        //        request.SortBy
        //    );
        //}

        private static Dictionary<string, string> GetOrderByMappings(PagingRequest request)
        {
            return request.GetType().GetProperty("OrderByMappings",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)
                ?.GetValue(request) as Dictionary<string, string> ?? new Dictionary<string, string>();
        }
    }
}
