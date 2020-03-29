import { NgModule } from '@angular/core';
import {
  MatFormFieldModule,
  MatInputModule,
  MatButtonModule,
  MatCardModule,
  MatRadioModule,
  MatIconModule,
  MatDatepickerModule,
  MatNativeDateModule,
  MatSnackBarModule,
  MatDividerModule,
  MatToolbarModule,
  MatTooltipModule,
  MatTabsModule,
  MatRippleModule,
  MatCheckboxModule,
  MatProgressSpinnerModule,
  MatSelectModule,
  MatExpansionModule,
  MatSidenavModule,
  MatMenuModule,
  MatListModule,
  MatStepperModule,
  MatAutocompleteModule,
  MatTableModule,
  MatButtonToggleModule,
  MatSlideToggleModule,
  MatDialogModule,
  MatPaginatorModule
} from '@angular/material';

const modules = [
  MatFormFieldModule,
  MatInputModule,
  MatButtonModule,
  MatCardModule,
  MatRadioModule,
  MatIconModule,
  MatDatepickerModule,
  MatNativeDateModule,
  MatSnackBarModule,
  MatDividerModule,
  MatToolbarModule,
  MatTooltipModule,
  MatTabsModule,
  MatRippleModule,
  MatCheckboxModule,
  MatProgressSpinnerModule,
  MatSelectModule,
  MatExpansionModule,
  MatSidenavModule,
  MatMenuModule,
  MatListModule,
  MatStepperModule,
  MatAutocompleteModule,
  MatTableModule,
  MatPaginatorModule,
  MatButtonToggleModule,
  MatSlideToggleModule,
  MatDialogModule
];

@NgModule({
  imports: modules,
  exports: modules
})
export class MaterialModule { }
