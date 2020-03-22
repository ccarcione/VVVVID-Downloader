using System;
using System.Collections.Generic;
using System.Globalization;

namespace VVVVID_Downloader.ConsoleApp
{
    class Program
    {
        public static VVVID _vvvID;
        static void Main(string[] args)
        {
            _vvvID = new VVVID();
            GetAnime();
            int episodioNumero = SelectEpisode();
            _vvvID.Start(episodioNumero);
            Console.ReadKey();
        }
        static void GetAnime()
        {
            //filtra per lettera
            Console.Write("Inserisci l'iniziale dell'anime che vuoi guardare: ");
            char c = Console.ReadLine()[0];
            c = char.ToLower(c, CultureInfo.InvariantCulture);
            List<Anime> listAnime = _vvvID.AnimeFilter(c);
            for (int k = 0; k < listAnime.Count; k++)
                Console.WriteLine((k + 1) + "- " + listAnime[k].title);
            //seleziona numero elenco
            Console.Write("Seleziona l'anime che vuoi guardare: ");
            int animeNumero = int.Parse(Console.ReadLine()) - 1;
            var animeData = _vvvID.GetAnimeData(listAnime[animeNumero].show_id);
            int i = 0;
            if (animeData.data.Count > 1)
            {
                // stampa elenchi versione lingua (stagioni???)
                foreach (Anime a in animeData.data)
                    Console.WriteLine(a.number + "- " + a.name);
                Console.Write("Scegli: ");
                i = int.Parse(Console.ReadLine()) - 1;
            }
            _vvvID.Anime = animeData.data[i];
            _vvvID.Anime.title = listAnime[animeNumero].title;
        }
        static int SelectEpisode()
        {
            Anime anime = _vvvID.Anime;
            for (int i = 0; i < anime.episodes.Count; i++)
                if (anime.episodes[i].playable)
                    Console.WriteLine($"{anime.episodes[i].number}  <-\t {anime.title} {anime.episodes[i].number}");
            Console.WriteLine("-1  <-\tPer scaricarli tutti");
            Console.Write("Inserisci il numero dell'episodio che vuoi scaricare: ");
            int episodioNumero = int.Parse(Console.ReadLine()) - 1;
            return episodioNumero;
        }
    }
}
