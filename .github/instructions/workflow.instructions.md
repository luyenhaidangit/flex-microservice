---
applyTo: '**'
---
Coding standards, domain knowledge, and preferences that AI should follow.

# System.API Workflow Guidelines

## Project Structure

### Core Components
- **Controllers**: API endpoints and business logic
- **Entities**: Domain models mapped to database
- **Repositories**: Data access layer
- **Extensions**: Service configuration and DI setup
- **Persistence**: Database context and configurations

### Design Patterns
1. Repository Pattern
- Base repository interface: `IRepositoryBase<TEntity, TKey, TContext>`
- Generic implementation: `RepositoryBase<TEntity, TKey, TContext>` 
- Entity-specific repositories inherit from base

2. Unit of Work Pattern
- Interface: `IUnitOfWork<TContext>`
- Implementation: `UnitOfWork<TContext>`
- Transaction management and SaveChanges

### Key Workflows

#### 1. Branch Management Flow
- Master data in `BranchMaster` table
- Request workflow:
  1. Create request header (`BranchRequestHeader`)
  2. Add request details (`BranchRequestData`) 
  3. Approve/Reject request
  4. Update master and audit log
  5. Cache invalidation if needed

#### 2. Configuration Management
- Key-Value pairs in `Config` table
- Redis caching for frequently accessed configs
- Auth mode configuration with validation

### Code Standards

1. Repository Layer
```csharp
public class EntityRepository : RepositoryBase<Entity, long, SystemDbContext>, IEntityRepository 
{
    public EntityRepository(SystemDbContext dbContext, IUnitOfWork<SystemDbContext> unitOfWork) 
        : base(dbContext, unitOfWork)
    {
    }
}
```

2. Controller Layer
```csharp
[Route("api/[controller]")]
[ApiController]
public class EntityController : ControllerBase
{
    private readonly IEntityRepository _repository;
    
    // Constructor injection
    // Query methods with [HttpGet]
    // Command methods with [HttpPost]
}
```

3. Entity Models
```csharp
[Table("TableName")]
public class Entity : EntityBase<long> 
{
    [Required]
    [Column("COLUMN_NAME", TypeName = "DATA_TYPE")]
    public string Property { get; set; }
}
```

### Best Practices

1. **Transaction Management**
```csharp
await using var tx = await _repository.BeginTransactionAsync();
try {
    // DB operations
    await tx.CommitAsync();
} catch {
    await tx.RollbackAsync();
    throw;
}
```

2. **Error Handling**
```csharp
return BadRequest(Result.Failure("Error message"));
return Ok(Result.Success(data: result));
```

3. **Caching Strategy**
- Use distributed cache (Redis)
- Cache invalidation on data changes
- Configurable cache duration

4. **Query Optimization**
- Use IQueryable for deferred execution
- Include related data explicitly
- Filter early in the query chain

### Deployment Considerations

1. **Database**
- Oracle database
- Migrations handled by EF Core
- Connection string in configuration

2. **Dependencies**
- AutoMapper for object mapping
- EntityFrameworkCore for data access
- Redis for distributed caching

3. **Configuration**
- AppSettings for environment-specific configs
- Feature flags for gradual rollouts
- Logging and monitoring setup
