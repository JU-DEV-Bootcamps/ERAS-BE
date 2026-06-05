using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

using Eras.Application.Contracts.Persistence;
using Eras.Error.Critical;

using Microsoft.EntityFrameworkCore;

namespace Eras.Infrastructure.Persistence.PostgreSQL.Repositories
{
    [ExcludeFromCodeCoverage]
    public class BaseRepository<TDomain, TPersist> : IBaseRepository<TDomain>
        where TDomain : class
        where TPersist : class
    {
        protected readonly AppDbContext _context;
        private readonly Func<TPersist, TDomain> _toDomain;
        private readonly Func<TDomain, TPersist> _toPersistence;

        public BaseRepository(
            AppDbContext Context,
            Func<TPersist, TDomain> ToDomain,
            Func<TDomain, TPersist> ToPersistence)
        {
            _context = Context;
            _toDomain = ToDomain;
            _toPersistence = ToPersistence;
        }

        public async Task<TDomain> AddAsync(TDomain Entity)
        {
            try
            {

                var response = await _context.Set<TPersist>().AddAsync(_toPersistence(Entity));
                await _context.SaveChangesAsync();

                return _toDomain(response.Entity);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                throw new DatabaseCustomException(ex);
            }
        }

        public async Task AddBatchAsync(IEnumerable<TDomain> Entities)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                IEnumerable<TPersist> entitiesToPersist = Entities.Select(e => _toPersistence(e));
                _context.Set<TPersist>().AddRange(entitiesToPersist);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                await transaction.RollbackAsync();
                throw new DatabaseCustomException(ex);
            }
        }
        public async Task DeleteAsync(TDomain Entity)
        {
            _context.Set<TPersist>().Remove(_toPersistence(Entity));
            await _context.SaveChangesAsync();
        }

        public async Task<TDomain?> GetByIdAsync(int Id)
        {
            var persistenceEntity = await _context.Set<TPersist>().FindAsync(Id);


            return persistenceEntity != null
                ? _toDomain(persistenceEntity)
                : null;
        }
        public async Task<IEnumerable<TDomain>> GetAllAsync()
        {
            var persistenceEntities = await _context.Set<TPersist>().ToListAsync();
            return persistenceEntities.Select(Entity => _toDomain(Entity));
        }


        public async Task<IEnumerable<TDomain>> GetPagedAsync(int Page, int PageSize)
        {
            var persistenceEntity = await _context.Set<TPersist>()
                .Skip((Page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            
            return persistenceEntity.Select(Entity => _toDomain(Entity));
        }
        public async Task<int> CountAsync()
        {
            return await _context.Set<TPersist>().CountAsync();
        }

        public async Task<TDomain> UpdateAsync(TDomain Entity)
        {
            try
            {
                _context.Set<TPersist>().Update(_toPersistence(Entity));
                await _context.SaveChangesAsync();

                return Entity;
            }
            catch (Exception e) 
            {
                throw new DatabaseCustomException(e);
            }
        }


        public async Task<int> CountAsync(Expression<Func<TDomain, bool>> predicate)
        {
            var oldParam = predicate.Parameters[0];

            var newParam = Expression.Parameter(typeof(TPersist), oldParam.Name);

            var visitor = new ParameterReplacer(oldParam, newParam);
            var body = visitor.Visit(predicate.Body);

            var finalExpression = Expression.Lambda<Func<TPersist, bool>>(body, newParam);

            return await _context.Set<TPersist>().CountAsync(finalExpression);
        }

        internal class ParameterReplacer(ParameterExpression oldParam, ParameterExpression newParam) : ExpressionVisitor
        {
            protected override Expression VisitParameter(ParameterExpression node)
                => node == oldParam ? newParam : base.VisitParameter(node);

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Member.Name == "Audit")
                {
                    return base.Visit(node.Expression);
                }

                var memberName = node.Member.Name;
                var targetType = newParam.Type;
                var property = targetType.GetProperty(memberName);

                if (property != null)
                {
                    return Expression.Property(Visit(node.Expression), property);
                }

                return base.VisitMember(node);
            }
        }

        public async Task<int> CountByDateRangeAsync(DateTime start, DateTime end)
        {
            var entityType = _context.Model.FindEntityType(typeof(TPersist));
            var tableName = entityType.GetTableName();

            var sql = $"SELECT COUNT(*) FROM \"{tableName}\" WHERE created_at >= @p0 AND created_at <= @p1";

            using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;

            var p0 = command.CreateParameter();
            p0.ParameterName = "@p0";
            p0.Value = DateTime.SpecifyKind(start, DateTimeKind.Utc);
            command.Parameters.Add(p0);

            var p1 = command.CreateParameter();
            p1.ParameterName = "@p1";
            p1.Value = DateTime.SpecifyKind(end, DateTimeKind.Utc);
            command.Parameters.Add(p1);

            if (command.Connection.State != ConnectionState.Open)
                await command.Connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }
}
