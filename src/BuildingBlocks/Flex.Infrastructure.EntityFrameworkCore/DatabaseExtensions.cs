using Flex.Infrastructure.EntityFrameworkCore.Configurations;
using Flex.Shared.SeedWork.Workflow;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Flex.Infrastructure.EntityFrameworkCore
{
    public static class DatabaseExtensions
    {
        public static string ToRequestTableName(this string baseTableName)
        {
            if (string.IsNullOrWhiteSpace(baseTableName))
                throw new ArgumentException("Base table name cannot be null or empty.", nameof(baseTableName));

            var singular = baseTableName.EndsWith("S", StringComparison.OrdinalIgnoreCase)
                ? baseTableName.Substring(0, baseTableName.Length - 1)
                : baseTableName;

            return singular + "_REQUESTS";
        }

        public static void ApplyApprovalRequests(this ModelBuilder modelBuilder, Assembly assembly)
        {
            var approvalEntities = assembly
                .GetTypes()
                .Where(t => !t.IsAbstract
                    && t.BaseType != null
                    && t.BaseType.IsGenericType
                    && t.BaseType.GetGenericTypeDefinition() == typeof(ApprovalEntityBase<>));

            foreach (var entity in approvalEntities)
            {
                var keyType = entity.BaseType!.GetGenericArguments().First();
                var requestType = typeof(RequestBase<>).MakeGenericType(keyType);

                // Lấy table name gốc
                var entityType = modelBuilder.Model.FindEntityType(entity);
                var baseTableName = entityType?.GetTableName() ?? entity.Name.ToUpper();

                // Sinh tên bảng request
                var requestTableName = baseTableName.ToRequestTableName();

                // Đăng ký entity Request + cấu hình
                modelBuilder.ApplyConfiguration(
                    (dynamic)Activator.CreateInstance(
                        typeof(RequestBaseConfiguration<,>).MakeGenericType(requestType, keyType)
                    )!
                );

                // Override tên bảng
                modelBuilder.Entity(requestType, b =>
                {
                    b.ToTable(requestTableName);
                });
            }
        }

        public static IQueryable<object> GetRequest<TEntity>(this DbSet<TEntity> dbSet, DbContext context) where TEntity : class
        {
            var entityType = typeof(TEntity);
            var requestTypeName = entityType.FullName!.Replace(entityType.Name, entityType.Name + "Request");

            var requestType = entityType.Assembly.GetType(requestTypeName);
            if (requestType == null)
                throw new InvalidOperationException($"Không tìm thấy request type: {requestTypeName}");

            var method = typeof(DbContext).GetMethod("Set", Type.EmptyTypes)!.MakeGenericMethod(requestType);
            var requestDbSet = method.Invoke(context, null) as IQueryable<object>;

            return requestDbSet!;
        }
    }
}
