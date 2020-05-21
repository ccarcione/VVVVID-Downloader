import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ShellComponent } from './components/shell/shell.component';
import { ShellContentDirective } from './components/shell/shell-content.directive';
import { ShellMenuDirective } from './components/shell/shell-menu.directive';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MaterialModule } from './material.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HomeComponent } from './components/home/home.component';
import { HttpClientModule } from '@angular/common/http';
import { MatInputModule, MatPaginatorModule, MatProgressSpinnerModule, MatSortModule, MatTableModule } from '@angular/material';
import { AnimeListComponent } from './components/anime-list/anime-list.component';
import { AboutComponent } from './components/about/about.component';
import { AnimeDetailComponent } from './components/anime-detail/anime-detail.component';


@NgModule({
  declarations: [
    AppComponent,
    ShellComponent,
    ShellContentDirective,
    ShellMenuDirective,
    HomeComponent,
    AnimeListComponent,
    AboutComponent,
    AnimeDetailComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MaterialModule,
    FormsModule,
    HttpClientModule,
    MatInputModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatProgressSpinnerModule,
    ReactiveFormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
