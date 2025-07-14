using System.Collections.Generic;

namespace DevCommon.Extended
{
    public sealed class CStack<T>
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

        // Push item or add it to stack list
        public void Push(T a_Item)
        {
            items.Add(a_Item);
        }

        // Return the last object in the list and remove it from stack list
        public T Pop()
        {
            if (items.Count > 0)
            {
                T temp = items[items.Count - 1];
                items.RemoveAt(items.Count - 1);
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

        // Returns the stack list
        public List<T> GetAllItems()
        {
            return items;
        }

        // Clear out all items from stack list
        public void Clear()
        {
            items.Clear();
        }
    }
}