using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DevCommon.Extended
{
    public sealed class CQueue<T>
    {
        private List<T> items = new List<T>();

        // Returns count of items
        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        // Check if stack contains item
        public bool Contains(T a_Item)
        {
            return items.Contains(a_Item);
        }

        // Add it to queue list
        public void Enqueue(T a_Item)
        {
            items.Add(a_Item);
        }

        // Return the first object in the queue list and remove it from queue list
        public T Dequeue()
        {
            if (items.Count > 0)
            {
                T temp = items[0];
                items.RemoveAt(0);
                return temp;
            }
            else
                return default(T);
        }

        // Removes an item from the stack list
        public void Remove(T a_Item)
        {
            items.Remove(a_Item);
        }

        // Returns the queue list
        public List<T> GetAllItems()
        {
            return items;
        }

        // Return the first object in the queue list
        public T Peek()
        {
            if (items.Count > 0)
            {
                T temp = items[0];
                return temp;
            }
            else
                return default(T);
        }

        // Clear out all items from queue list
        public void Clear()
        {
            items.Clear();
        }

        // Check if the queue list has any items
        public bool Any()
        {
            return items.Any();
        }
    }
}