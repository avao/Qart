using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Qart.Core.DataStore
{
    public class ExtendedDataStore : IDataStore
    {
        private readonly IDataStore _dataStore;
        private readonly Func<string, IDataStore, Stream> _streamFunc;

        public ExtendedDataStore(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public ExtendedDataStore(IDataStore dataStore, Func<string, IDataStore, Stream> streamFunc)
        {
            _dataStore = dataStore;
            _streamFunc = streamFunc;
        }

        public Stream GetReadStream(string itemId)
        {
            if (_dataStore.Contains(itemId))
            {
                return _dataStore.GetReadStream(itemId);
            }

            string itemRef = GetItemRef(itemId);
            if (_dataStore.Contains(itemRef))
            {
                string target = CombinePaths(Path.GetDirectoryName(itemId), "", _dataStore.GetContent(itemRef));
                return GetReadStream(target);
            }

            string func = GetItemFunc(itemId);
            if (_dataStore.Contains(func))
            {
                return _streamFunc(_dataStore.GetContent(func), this);
            }

            itemRef = GetRedirectedItemId(itemId);
            if (itemRef != null && Contains(itemRef))
            {
                return GetReadStream(itemRef);
            }

            return null;
        }

        public Stream GetWriteStream(string itemId)
        {
            return _dataStore.GetWriteStream(itemId);
        }

        public bool Contains(string itemId)
        {
            if (_dataStore.Contains(itemId) || _dataStore.Contains(GetItemRef(itemId)))
                return true;

            var redirectedId = GetRedirectedItemId(itemId);
            if (redirectedId != null && itemId != redirectedId)
            {
                return Contains(redirectedId);
            }

            string func = GetItemFunc(itemId);
            return _dataStore.Contains(func);
        }

        public IEnumerable<string> GetItemIds(string tag)
        {
            //TODO
            return _dataStore.GetItemIds(tag);
        }

        public IEnumerable<string> GetItemGroups(string group)
        {
            //TODO
            return _dataStore.GetItemGroups(group);
        }

        private string GetRedirectedItemId(string itemId)
        {
            //TODO redirection logic should be rewritten as it won't work with items in subfolders
            //first redirection point should be found and then itemId should be rewriten in accordance to redirection

            var groupId = Path.GetDirectoryName(itemId);
            var referenceItemId = Path.Combine(groupId, ".ref");
            if (_dataStore.Contains(referenceItemId))
            {
                var reference = _dataStore.GetContent(referenceItemId).Trim();
                return CombinePaths(groupId, reference, Path.GetFileName(itemId));
            }
            return null;
        }

        private string CombinePaths(string currentGroupId, string redirectedGroupId, string itemId)
        {
            var redirectedItemId = Path.Combine(redirectedGroupId, itemId);
            if (!Path.IsPathRooted(redirectedItemId))
            {
                redirectedItemId = Path.Combine(currentGroupId, redirectedGroupId, itemId);
            }

            return redirectedItemId;
        }

        private string GetItemRef(string name)
        {
            return name + ".ref";
        }

        private string GetItemFunc(string name)
        {
            return name + ".transform";
        }
    }
}
