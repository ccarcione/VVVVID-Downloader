import { Component, ContentChild, ElementRef } from '@angular/core';
import { Breakpoints, BreakpointObserver } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ShellMenuDirective } from './shell-menu.directive';
import { ShellService } from '../../services/shell.service';

@Component({
  selector: 'dp-shell',
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.css']
})
export class ShellComponent {
  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches)
    );

  @ContentChild(ShellMenuDirective, { static: true}) fallbackMenu;
  get hasMenu(): boolean { return this.menu || this.fallbackMenu; }
  get menu(): ElementRef { return this.shellService.data.menu; }
  get title(): ElementRef { return this.shellService.data.title; }
  get actions(): ElementRef { return this.shellService.data.actions; }
  get search(): ElementRef { return this.shellService.data.search; }
  get nav(): ElementRef { return this.shellService.data.nav; }

  constructor(private breakpointObserver: BreakpointObserver, private shellService: ShellService) { }
}
