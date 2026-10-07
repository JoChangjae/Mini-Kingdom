using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MiniKingdom.Core
{
    /// <summary>
    /// A thread-safe, static generic event bus.
    /// </summary>
    public static class EventBus
    {
        private static readonly ConcurrentDictionary<Type, object> _subscribers = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Subscribes to an event of type T.
        /// </summary>
        public static void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            _subscribers.AddOrUpdate(type, 
                _ => new List<Action<T>> { handler },
                (_, existingList) => 
                {
                    var list = (List<Action<T>>)existingList;
                    lock(list)
                    {
                        list.Add(handler);
                    }
                    return list;
                });
        }

        /// <summary>
        /// Unsubscribes from an event of type T.
        /// </summary>
        public static void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var existingList))
            {
                var list = (List<Action<T>>)existingList;
                lock(list)
                {
                    list.Remove(handler);
                }
            }
        }

        /// <summary>
        /// Publishes an event of type T to all subscribers.
        /// </summary>
        public static void Publish<T>(T eventMessage)
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var existingList))
            {
                var list = (List<Action<T>>)existingList;
                Action<T>[] handlers;
                lock(list)
                {
                    handlers = list.ToArray();
                }
                
                foreach (var handler in handlers)
                {
                    handler?.Invoke(eventMessage);
                }
            }
        }
    }
}
