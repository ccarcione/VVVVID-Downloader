import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { Anime } from 'src/app/models/anime';
import { Subscription } from 'rxjs';
import { AnimeService } from 'src/app/services/anime.service';
import { ShellData, ShellService } from 'src/app/services/shell.service';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';

@Component({
  selector: 'app-anime-list',
  templateUrl: './anime-list.component.html',
  styleUrls: ['./anime-list.component.css']
})
export class AnimeListComponent implements OnInit, OnDestroy, ShellData {
  sub: Subscription = new Subscription();
  animeList: Anime[];
  @ViewChild('title', { static: true }) title: ElementRef;
  isMobile: boolean = false;

  constructor(
    private ss: ShellService,
    private animeService: AnimeService,
    private breakpointObserver: BreakpointObserver) { }

  ngOnInit() {
    this.ss.register(this);
    this.sub.add(this.animeService.anime$
      .subscribe(a => {
        this.animeList = a;
      }));
    this.sub.add(this.breakpointObserver.observe(Breakpoints.Handset).subscribe(b =>  this.isMobile = b.matches));
    this.animeService.getAnimeList();
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
