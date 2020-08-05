using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Logic
{
    public static class ReadOnlyCollectionExtensions
    {
        public static ReadOnlyCollection <T> Add <T>(this ReadOnlyCollection <T> list, T value)
        {
            ReadOnlyCollection<T> result = null;

            if (list != null)
            {
                List<T> tmpList = new List<T> ();

                for (int i = 0; i < list.Count; i++)
                {
                    tmpList.Add (list [i]);
                }

                tmpList.Add (value);
                result = new ReadOnlyCollection<T> (tmpList);
            }
            else
            {
                DebugLogger.Log (ActionStatus.NullParam);
            }
                   
            return result;
        }

        public static ReadOnlyCollection<T> RemoveAt<T>(this ReadOnlyCollection<T> list, int index)
        {
            ReadOnlyCollection<T> result = null;

            if (list != null)
            {
                List<T> tmpList = new List<T> ();

                for (int i = 0; i < list.Count; i++)
                {
                    if (i != index)
                    {
                        tmpList.Add (list [i]);
                    }
                }

                result = new ReadOnlyCollection<T> (tmpList);
            }
            else
            {
                DebugLogger.Log (ActionStatus.NullParam);
            }

            return result;
        }
    }
}

