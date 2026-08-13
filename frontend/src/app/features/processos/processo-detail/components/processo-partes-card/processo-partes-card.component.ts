import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ParteDto } from '../../../../../core/models/responses';
import { TipoPolo, TipoEntidade } from '../../../../../core/models/enums';

@Component({
  selector: 'app-processo-partes-card',
  standalone: true,
  imports: [CommonModule, CardModule, ButtonModule, TableModule, TagModule, TooltipModule],
  template: `
    <p-card styleClass="shadow-1 border-round">
      <div class="flex justify-content-between align-items-center mb-4 pb-3 border-bottom-1 surface-border">
        <h3 class="text-xl font-medium m-0">Partes Envolvidas</h3>
        <p-button label="Adicionar Parte" icon="pi pi-user-plus" size="small" (onClick)="addParte.emit()"></p-button>
      </div>
      
      <p-table [value]="partes" styleClass="p-datatable-sm p-datatable-gridlines">
        <ng-template pTemplate="header">
          <tr>
            <th style="width: 100px" class="text-center">Ações</th>
            <th>Polo</th>
            <th>Tipo da Entidade</th>
            <th>Nome</th>
            <th>Documento</th>
          </tr>
        </ng-template>
        <ng-template pTemplate="body" let-parte>
          <tr>
            <td class="text-center">
              <p-button icon="pi pi-trash" [rounded]="true" [text]="true" severity="danger" size="small" pTooltip="Remover" (onClick)="removeParte.emit(parte.id)"></p-button>
            </td>
            <td>
              <p-tag [value]="isPoloAtivo(parte.tipoPolo) ? 'Polo Ativo' : 'Polo Passivo'" [severity]="isPoloAtivo(parte.tipoPolo) ? 'info' : 'warn'"></p-tag>
            </td>
            <td>
              {{ getTipoEntidadeLabel(parte.entidadeLegal.tipoEntidade) }}
            </td>
            <td>
              {{ parte.entidadeLegal.nome }}
            </td>
            <td>
              <div class="flex align-items-center gap-2">
                <p-tag [value]="parte.entidadeLegal.tipoEntidade === 'PessoaFisica' ? 'CPF' : 'CNPJ'" severity="secondary"></p-tag>
                <span>{{ parte.entidadeLegal.numeroDocumento }}</span>
              </div>
            </td>
          </tr>
        </ng-template>
        <ng-template pTemplate="emptymessage">
          <tr>
            <td colspan="5" class="text-center p-4 text-color-secondary">Nenhuma parte envolvida registrada.</td>
          </tr>
        </ng-template>
      </p-table>
    </p-card>
  `,
  styles: []
})
export class ProcessoPartesCardComponent {
  @Input({ required: true }) partes!: ParteDto[];
  @Output() addParte = new EventEmitter<void>();
  @Output() removeParte = new EventEmitter<string>();

  isPoloAtivo(tipoPolo: TipoPolo | string): boolean {
    return tipoPolo === TipoPolo.Ativo || tipoPolo === 'Ativo';
  }

  getTipoEntidadeLabel(tipoEntidade: TipoEntidade | string): string {
    return tipoEntidade === TipoEntidade.PessoaFisica || tipoEntidade === 'PessoaFisica' 
      ? 'Pessoa Física' 
      : 'Pessoa Jurídica';
  }
}
