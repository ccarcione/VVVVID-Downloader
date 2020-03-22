using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VVVVID_Downloader.Hds
{
    public class HdsTool
    {
        public static string SanitizeFileName(string fileName) => string.Join("", fileName.Split(Path.GetInvalidFileNameChars()));

        public static string DecodeManifestLink(string h)
        {
            h = h.Replace("master.m3u8", "manifest.f4m").Replace("/i/", "/z/");
            // TODO: they might change this, find a smart way to retrieve it from vvvvid.js
            var g = "MNOPIJKL89+/4567UVWXQRSTEFGHABCDcdefYZabstuvopqr0123wxyzklmnghij";
            var m = h.Select(c => g.IndexOf(c)).ToArray();
            for (var i = m.Length * 2 - 1; i >= 0; i--)
                m[i % m.Length] ^= m[(i + 1) % m.Length];
            var sb = new StringBuilder(m.Length * 3 / 4);
            for (int i = 0; i < m.Length; i++)
                if (i % 4 != 0)
                    sb.Append((char)((m[i - 1] << (i % 4) * 2 & 255) + (m[i] >> (3 - i % 4) * 2)));
            if (m.Length % 4 == 1)
                sb.Append((char)(m.Last() << 2));
            //return sb.ToString();
            
            string manifestLink = sb.ToString();
            return manifestLink.Contains("akamaihd") ? manifestLink + "?hdcore=3.6.0" : $"http://wowzaondemand.top-ix.org/videomg/_definst_/mp4:{manifestLink}/manifest.f4m";
        }
    }

    //public class Json
    //{
    //    private readonly static JavaScriptSerializer _serializer;
    //    static Json()
    //    {
    //        _serializer = new JavaScriptSerializer();
    //    }

    //    private readonly Dictionary<string, object> _dictionary;
    //    public Json(string s)
    //    {
    //        _dictionary = _serializer.Deserialize<Dictionary<string, object>>(s);
    //    }

    //    public object this[string key] => Get(key);

    //    public object Get(string key) => _dictionary[key];

    //    public T Get<T>(string key) => (T)Get(key);

    //    public ArrayList GetList(string key) => Get<ArrayList>(key);

    //    public IEnumerable<T> GetList<T>(string key) => Get<ArrayList>(key).Cast<T>();

    //    public static string GetStringRE(string json, string key) => json.ReMatch($"\"{key}\":" + "(\")?(.*?)(?(1)\")[,}]", 2);
    //}
}
