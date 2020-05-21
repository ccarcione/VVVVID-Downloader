import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { BehaviorSubject } from 'rxjs';
import { Anime } from '../models/anime';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/x-www-form-urlencoded' })
}

const ApiEndpoint = environment.APIEndpoint;
const url = ApiEndpoint + 'api/SerieTV';

@Injectable({
  providedIn: 'root'
})
export class AnimeService {
  private animeSub = new BehaviorSubject<Anime[]>(null);
  anime$ = this.animeSub.asObservable();

  constructor(private http: HttpClient) { }

  async getAnimeList(): Promise<void> {
    const tt = (await this.http.get<Anime[]>('api/VVVVID/GetAllAnime').toPromise())
      .map(t => new Anime(t));
    this.animeSub.next(tt);
  }

  async getAnimeById(showId: string): Promise<Anime[]> {
    return (await this.http.get<Anime[]>('api/VVVVID/Anime/'.concat(showId)).toPromise())
      .map(t => new Anime(t));
  }
}
