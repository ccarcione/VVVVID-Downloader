import { Injectable, OnDestroy, ElementRef } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ShellService {
  data: ShellData = {};

  register(component: OnDestroy & ShellData) {
    for (const p in component) {
      if (component.hasOwnProperty(p)) {
        this.data[p] = component[p];
      }
    }

    const od = component.ngOnDestroy.bind(component);
    component.ngOnDestroy = () => { od(); this.unregister(); };
  }

  unregister() { this.data = {}; console.log('distrutto componente'); }
}

export interface ShellData {
  title?: ElementRef;
  search?: ElementRef;
  actions?: ElementRef;
  menu?: ElementRef;
  nav?: ElementRef;
}
