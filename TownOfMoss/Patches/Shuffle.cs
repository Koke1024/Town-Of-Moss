using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs {
    public static class ListExtensions {
        /// <summary>
        ///     Shuffles the element order of the specified list.
        /// </summary>
        public static void Shuffle<T>(this List<T> list) {
            var count = list.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i) {
                var r = Random.Range(i, count);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }

        public static T TakeFirst<T>(this List<T> list) {
            var item = list[0];
            list.RemoveAt(0);
            return item;
        }
    }
}