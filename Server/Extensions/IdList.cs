using Server.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Server.Extensions
{
    public class IdList<T> : List<T> where T : IIdentificator
    {
        private int lastId => base.Count > 0 ? this.Max(p => p.Id) : 0;
        public T Add(T item)
        {
            item.Id = lastId + 1;
            base.Add(item);
            return item;
        }
        public T GetForId(int id)
        {
            int left = 0;
            int right = Count - 1;
            int getMiddle() => (right - left) / 2;
            for(int mId = getMiddle(); mId > 0; mId = getMiddle())
            {
                if (base[mId + left].Id == id)
                    return base[mId + left];
                else if (right - left == 2 || right == left)
                {
                    if (base[right].Id == id)
                        return base[right];
                    if (base[left].Id == id)
                        return base[left];
                }
                else if (base[mId + left].Id > id)
                    right -= mId;
                else if (base[mId + left].Id < id)
                    left += mId;
            }
            return default;
        }
    }
}
