import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { TextareaModule } from 'primeng/textarea';
import { ProcessoDto } from '../../../../../core/models/responses';

@Component({
  selector: 'app-processo-details-card',
  standalone: true,
  imports: [CommonModule, FormsModule, CardModule, ButtonModule, TooltipModule, TextareaModule, DatePipe],
  template: `
    <p-card styleClass="shadow-1 border-round h-full">
      <h3 class="text-xl font-medium m-0 mb-4 pb-3 border-bottom-1 surface-border">Detalhes do Processo</h3>
      <div class="grid grid-nogutter">
        <div class="col-12 mb-4">
          <span class="text-color-secondary block text-sm font-medium mb-1">Assunto</span>
          <div *ngIf="!isEditingAssunto" class="flex align-items-center gap-3">
            <p class="m-0 text-900 line-height-3 text-lg">{{ processo.assunto }}</p>
            <p-button icon="pi pi-pencil" [text]="true" [rounded]="true" (onClick)="toggleEditAssunto()" pTooltip="Editar Assunto"></p-button>
          </div>
          <div *ngIf="isEditingAssunto" class="flex align-items-start gap-2 mt-2">
            <textarea pInputTextarea [(ngModel)]="editAssuntoValue" rows="2" class="w-full" style="resize: none;"></textarea>
            <div class="flex flex-column gap-2">
              <p-button icon="pi pi-check" severity="primary" (onClick)="onSaveAssunto()" [loading]="isSavingAssunto" pTooltip="Salvar"></p-button>
              <p-button icon="pi pi-times" severity="secondary" [outlined]="true" (onClick)="cancelEditAssunto()" pTooltip="Cancelar"></p-button>
            </div>
          </div>
        </div>
        <div class="col-12 md:col-6 mb-3">
          <span class="text-color-secondary block text-sm font-medium mb-1">Data de Criação</span>
          <p class="m-0 text-900">{{ processo.dataCriacao | date:'dd/MM/yyyy HH:mm' }}</p>
        </div>
      </div>
    </p-card>
  `,
  styles: []
})
export class ProcessoDetailsCardComponent {
  @Input({ required: true }) processo!: ProcessoDto;
  @Input() isSavingAssunto = false;
  @Output() saveAssunto = new EventEmitter<string>();

  isEditingAssunto = false;
  editAssuntoValue = '';

  toggleEditAssunto() {
    this.editAssuntoValue = this.processo.assunto || '';
    this.isEditingAssunto = true;
  }

  cancelEditAssunto() {
    this.isEditingAssunto = false;
  }

  onSaveAssunto() {
    this.saveAssunto.emit(this.editAssuntoValue);
    this.isEditingAssunto = false;
  }
}
