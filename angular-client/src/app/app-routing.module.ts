import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { AnimeListComponent } from './components/anime-list/anime-list.component';
import { AboutComponent } from './components/about/about.component';


const routes: Routes = [
  {
    path: '',
    component: AppComponent,
    children: [
      {
        path: '',
        component: HomeComponent,
        children: [
          {
            path: 'anime-list',
            component: AnimeListComponent
          },
          {
            path: 'about',
            component: AboutComponent
          }
        ]
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
