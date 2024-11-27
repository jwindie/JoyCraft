using System.Collections.Generic;

namespace JoyCraft {
        public class ObjectPool<T> {
        private Stack<T> stack;

        public ObjectPool () {
            stack = new Stack<T> ();
        }

        public void Pool (T item) {
            stack.Push (item);
        }

        public T GetItem () {
            if (stack.Count > 0) {
                return stack.Pop ();
            }
            else return default;
        }
    }
}