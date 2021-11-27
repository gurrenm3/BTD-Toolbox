using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace BTDToolbox.Extensions
{
    public static class ItemCollectionExtensions
    {
        public static void AddRange(this ItemCollection itemCollection, IEnumerable<object> collecton)
        {
            for (int i = 0; i < collecton.Count(); i++)
                itemCollection.Add(collecton.ElementAt(i));
        }

        public static void AddRange(this ItemCollection itemCollection, ItemCollection collectionToAdd)
        {
            for (int i = 0; i < collectionToAdd.Count; i++)
            {
                var item = collectionToAdd.GetItemAt(i);
                collectionToAdd.RemoveAt(i);
                itemCollection.Add(item);
            }
        }
    }
}
