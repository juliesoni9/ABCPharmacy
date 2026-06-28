import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SalesListComponent } from './components/sales-list/sales-list';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, SalesListComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {}
