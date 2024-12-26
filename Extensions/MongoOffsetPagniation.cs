using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JohnKnoop.MongoRepository;
using MongoDB.Driver;

namespace DragonAPI.Extensions
{

    public class PagnitionData<T>
    {
        public IReadOnlyList<T> Items { get; set; }
    }
    // public class ListOffsetPagniationFiltering
    // {
    //     public int Page { get; set; }
    //     public int Size { get; set; }
    // }
    public class ListCursorPagniationFiltering
    {
        public string After { get; set; }
        public string Before { get; set; }
        public int Size { get; set; }
    }
    public class OffsetPagniationData<T> : PagnitionData<T>
    {
        public long Count { get; set; }
        public long TotalPages { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public OffsetPagniationData() { }
        public OffsetPagniationData(IReadOnlyList<T> items, long count, int page, int pageSize)
        {
            Items = items;
            Page = page;
            Size = pageSize;
            Count = count;
            TotalPages = Count / pageSize;
            if (Count % pageSize > 0)
                TotalPages++;
        }
    }
    public class CursorPagniationData<T> : PagnitionData<T>
    {
        public string After { get; set; }
        public string Before { get; set; }
        public int Size { get; set; }
    }

    public static class MongoDBPagniationExtensions
    {
        public static async Task<OffsetPagniationData<TEntity>> QueryOffsetPagniation<TEntity>(
                this IRepository<TEntity> repo,
                FilterDefinition<TEntity> filter = null,
                SortDefinition<TEntity> sort = null,
                int? page = 1, int? pageSize = 20)
        {
            page = (page ?? 1) <= 0 ? 1 : page;
            pageSize = (pageSize ?? 20) <= 0 ? 20 : pageSize;
            var countFacet = AggregateFacet.Create
            (
                "count",
                PipelineDefinition<TEntity, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<TEntity>()
                })
            );

            var dataPipeline = new List<PipelineStageDefinition<TEntity, TEntity>>
            {
                PipelineStageDefinitionBuilder.Skip<TEntity>((int)((page - 1) * pageSize)),
                PipelineStageDefinitionBuilder.Limit<TEntity>((int)pageSize),
            };
            if (sort != null)
            {
                dataPipeline.Add(PipelineStageDefinitionBuilder.Sort(sort));
            }
            var dataFacet = AggregateFacet.Create("data", PipelineDefinition<TEntity, TEntity>.Create(dataPipeline));

            filter = filter ?? Builders<TEntity>.Filter.Empty;
            var aggregation = await repo.Aggregate()
                .Match(filter)
                .Facet(countFacet, dataFacet)
                .ToListAsync();

            var count = aggregation.First()
                .Facets.First(x => x.Name == "count")
                .Output<AggregateCountResult>()
                ?.FirstOrDefault()
                ?.Count ?? 0;

            var normalized = (int)(count / pageSize);
            var totalPages = count * 1.0 / pageSize > normalized ? normalized + 1 : normalized;

            var data = aggregation.First()
                .Facets.First(x => x.Name == "data")
                .Output<TEntity>();

            return new OffsetPagniationData<TEntity>
            {
                Items = data,
                Count = count,
                Page = (int)page,
                Size = (int)pageSize,
                TotalPages = totalPages,
            };
        }
    }
}