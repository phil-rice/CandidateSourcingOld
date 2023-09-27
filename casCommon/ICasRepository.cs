using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xingyi.cas.common
{
    public interface ICasRepository
    {
        Task<ContentItem> ContentItem(string nameSpace, string sha);

        Task Àdd(ContentItem item);

        public Task<IEnumerable<ContentItem>> ContentItems(string nameSpace, int count = 10);
    }

    public class CasRepository : ICasRepository
    {
        private readonly CasDbContext context;

        public CasRepository(CasDbContext context)
        {
            this.context = context;
        }

        public Task Àdd(ContentItem item)
        {
            context.ContentItems.Add(item);
            return context.SaveChangesAsync();
        }

        async public Task<ContentItem?> ContentItem(string nameSpace, string sha)
        {
            return await context.ContentItems.FindAsync(nameSpace, sha);
        }

        async public Task<IEnumerable<ContentItem>> ContentItems(string nameSpace, int count)
        {
            return context.ContentItems.Where(item => item.Namespace == nameSpace)
                                  .Take(count)
                                  .ToList();
        }
    }

    public class MockCasRepository : ICasRepository
    {
        private readonly List<ContentItem> _mockData;


        public MockCasRepository()
        {
            _mockData = new List<ContentItem>();

        }

        public Task<ContentItem> ContentItem(string nameSpace, string sha)
        {
            return Task.FromResult(_mockData.FirstOrDefault(ci => ci.Namespace == nameSpace && ci.SHA == sha));
        }

        public Task Àdd(ContentItem item)
        {
            _mockData.Add(item);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ContentItem>> ContentItems(string nameSpace, int count = 10)
        {
            var items = _mockData.Where(ci => ci.Namespace == nameSpace).Take(count);
            return Task.FromResult(items);
        }

    }

}
