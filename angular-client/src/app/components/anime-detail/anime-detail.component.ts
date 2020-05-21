import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component, Input, OnInit, ViewChild, ElementRef, OnDestroy } from '@angular/core';
import { ShellData, ShellService } from 'src/app/services/shell.service';
import { ActivatedRoute } from '@angular/router';
import { AnimeService } from 'src/app/services/anime.service';
import { Subscription, combineLatest } from 'rxjs';
import { Anime } from 'src/app/models/anime';
import { write } from 'fs';

@Component({
  selector: 'app-anime-detail',
  templateUrl: './anime-detail.component.html',
  styleUrls: ['./anime-detail.component.css']
})
export class AnimeDetailComponent implements OnInit, OnDestroy, ShellData {
  @ViewChild('title', { static: true }) title: ElementRef;
  sub: Subscription = new Subscription();
  anime: Anime[];
  isMobile: boolean = false;

  constructor(
    private ss: ShellService,
    private activateRouter: ActivatedRoute,
    private animeService: AnimeService,
    private breakpointObserver: BreakpointObserver
  ) { }

  ngOnInit() {
    this.ss.register(this);
    this.sub.add(this.activateRouter.paramMap.subscribe(async map => {
      const show_id = +map.get('show_id');
      await this.animeService.getAnimeById(show_id.toString())
        .then(x => this.anime = x);
    }));
    this.sub.add(this.breakpointObserver.observe(Breakpoints.Handset).subscribe(b => {
      this.isMobile = b.matches;
      console.log(this.isMobile);
    }));
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
