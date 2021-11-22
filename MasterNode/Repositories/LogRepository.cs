using System.Collections.Concurrent;

namespace MasterNode.Repositories
{
    public static class LogRepository
    {
        public static readonly ConcurrentDictionary<int, string> Context = new ConcurrentDictionary<int, string>();
    }
}
