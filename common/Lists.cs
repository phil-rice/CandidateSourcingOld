using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace xingyi.common
{
    public static class Lists
    {
        public static ListDiffResult<T> GetListDifferences<T>(List<T> originalList, List<T> updatedList)
        {
            var addedItems = updatedList.Except(originalList).ToList();
            var removedItems = originalList.Except(updatedList).ToList();
            return new ListDiffResult<T>(addedItems, removedItems);
        }
    }

    public record ListDiffResult<T>(List<T> Added, List<T> Removed);


}
