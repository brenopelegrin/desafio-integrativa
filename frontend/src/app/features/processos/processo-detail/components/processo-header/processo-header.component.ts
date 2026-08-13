import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { RouterModule } from '@angular/router';
import { StatusTagComponent } from '../../../../../shared/ui/status-tag/status-tag.component';
import { ProcessoDto } from '../../../../../core/models/responses';

@Component({
  selector: 'app-processo-header',
  standalone: true,
  imports: [CommonModule, ButtonModule, TooltipModule, RouterModule, StatusTagComponent],
  template: `
    <div class="header flex flex-column md:flex-row justify-content-between align-items-start md:align-items-center mb-4 gap-3">
      <div class="flex align-items-center gap-3">
        <p-button icon="pi pi-arrow-left" label="Voltar" [text]="true" routerLink="/processos" styleClass="p-button-secondary"></p-button>
        <h2 class="m-0 text-900 font-semibold text-2xl">Processo {{ processo.numeroProcesso }}</h2>
        <div class="flex align-items-center gap-2">
          <app-status-tag [status]="processo.status"></app-status-tag>
          <p-button icon="pi pi-pencil" [text]="true" [rounded]="true" size="small" (onClick)="editStatus.emit()" pTooltip="Editar Status"></p-button>
        </div>
      </div>
      <div class="actions">
        <!-- Future actions like Archive, Edit -->
      </div>
    </div>
  `,
  styles: []
})
export class ProcessoHeaderComponent {
  @Input({ required: true }) processo!: ProcessoDto;
  @Output() editStatus = new EventEmitter<void>();
}
