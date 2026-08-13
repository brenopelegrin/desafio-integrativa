import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TagModule } from 'primeng/tag';
import { StatusProcesso } from '../../../core/models/enums';

@Component({
  selector: 'app-status-tag',
  standalone: true,
  imports: [CommonModule, TagModule],
  templateUrl: './status-tag.component.html',
  styleUrls: ['./status-tag.component.scss']
})
export class StatusTagComponent {
  @Input({ required: true }) status!: StatusProcesso;

  getSeverity(status: StatusProcesso): 'success' | 'secondary' | 'info' | 'warn' | 'danger' | 'contrast' {
    switch (status) {
      case StatusProcesso.Ativo: return 'success';
      case StatusProcesso.Finalizado: return 'info';
      case StatusProcesso.Arquivado: return 'secondary';
      default: return 'warn';
    }
  }

  getStatusLabel(status: StatusProcesso): string {
    switch (status) {
      case StatusProcesso.Ativo: return 'Ativo';
      case StatusProcesso.Finalizado: return 'Finalizado';
      case StatusProcesso.Arquivado: return 'Arquivado';
      default: return 'Desconhecido';
    }
  }
}
