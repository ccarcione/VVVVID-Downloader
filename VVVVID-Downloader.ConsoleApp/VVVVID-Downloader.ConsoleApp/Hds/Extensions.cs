using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VVVVID_Downloader.ConsoleApp.Hds
{
    public static class Extensions
    {
        public static TSource MaxBy<TSource>(this IEnumerable<TSource> source, Func<TSource, IComparable> selector)
        {
            TSource max = default(TSource);
            foreach (TSource i in source)
                if (selector(i).CompareTo(max) > 0)
                    max = i;
            return max;
        }

        public static bool IsInt(this string s)
        {
            int n;
            return int.TryParse(s, out n);
        }

        public static string[] Split(this string s, string separator) => s.Split(new[] { separator }, StringSplitOptions.None);

        public static string Unescape(this string s) => Regex.Unescape(s);

        public static string ReMatch(this string s, string re, int groupIdx = 1) => Regex.Match(s, re).Groups[groupIdx].Value;

        public static string[] ReMatchGroups(this string s, string re) => Regex.Match(s, re).Groups.Cast<Group>().Select(g => g.Value).ToArray();

        public static IEnumerable<string> ReMatches(this string s, string re, int groupIdx = 1) =>
            Regex.Matches(s, re).Cast<Match>().Select(m => m.Groups[groupIdx].Value);

        public static IEnumerable<string[]> ReMatchesGroups(this string s, string re) =>
            Regex.Matches(s, re).Cast<Match>().Select(m => m.Groups.Cast<Group>().Select(g => g.Value).ToArray());

        public static void DoNotAwait(this Task t) { }
    }
}
