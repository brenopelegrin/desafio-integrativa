import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TimelineModule } from 'primeng/timeline';
import { PaginatorModule } from 'primeng/paginator';
import { SelectButtonModule } from 'primeng/selectbutton';
import { TooltipModule } from 'primeng/tooltip';
import { AndamentoDto } from '../../../../../core/models/responses';

@Component({
  selector: 'app-processo-andamentos-card',
  standalone: true,
  imports: [CommonModule, FormsModule, CardModule, ButtonModule, TableModule, TimelineModule, PaginatorModule, SelectButtonModule, TooltipModule, DatePipe],
  template: `
    <p-card styleClass="shadow-1 border-round flex flex-column h-full">
      <div class="flex justify-content-between align-items-center mb-4 pb-3 border-bottom-1 surface-border">
        <div class="flex align-items-center gap-4">
          <h3 class="text-xl font-medium m-0">Andamentos</h3>
          <p-selectButton [options]="andamentoViewOptions" [(ngModel)]="andamentoView" optionLabel="icon" optionValue="value">
            <ng-template let-item pTemplate="item">
              <i [class]="item.icon" [pTooltip]="item.tooltip" tooltipPosition="top"></i>
            </ng-template>
          </p-selectButton>
        </div>
        <p-button label="Novo" icon="pi pi-plus" (onClick)="addAndamento.emit()" size="small"></p-button>
      </div>

      <div class="flex-grow-1 flex flex-column">
        <!-- Timeline View -->
        <div *ngIf="andamentoView === 'timeline'" class="px-2 py-3 flex-grow-1 overflow-x-auto">
          <p-timeline [value]="paginatedAndamentos" layout="horizontal" align="top" styleClass="w-full custom-timeline">
            <ng-template pTemplate="content" let-event>
              <div class="timeline-event pt-3 pr-3" style="min-width: 250px">
                <span class="text-primary font-medium text-sm block mb-1">{{ event.data | date:'dd/MM/yyyy HH:mm' }}</span>
                <div class="flex flex-column gap-2 mt-2">
                  <p class="m-0 text-900 line-height-3" style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 250px;">{{ event.descricao }}</p>
                  <div class="text-left mt-2">
                    <p-button icon="pi pi-eye" [rounded]="true" [text]="true" size="small" label="Ver" (onClick)="viewAndamento.emit(event)"></p-button>
                  </div>
                </div>
              </div>
            </ng-template>
          </p-timeline>
        </div>

        <!-- Table View -->
        <div *ngIf="andamentoView === 'table'" class="flex-grow-1">
          <p-table [value]="paginatedAndamentos" styleClass="p-datatable-sm p-datatable-striped" responsiveLayout="scroll">
            <ng-template pTemplate="header">
              <tr>
                <th style="width: 50px" class="text-center">Ações</th>
                <th style="width: 120px">Data/Hora</th>
                <th>Descrição</th>
              </tr>
            </ng-template>
            <ng-template pTemplate="body" let-event>
              <tr>
                <td>
                  <p-button icon="pi pi-eye" [rounded]="true" [text]="true" size="small" (onClick)="viewAndamento.emit(event)" pTooltip="Ver Detalhes"></p-button>
                </td>
                <td class="font-medium text-900">{{ event.data | date:'dd/MM/yyyy HH:mm' }}</td>
                <td><p class="m-0 text-truncate-2" style="max-width: 100%;">{{ event.descricao }}</p></td>
              </tr>
            </ng-template>
          </p-table>
        </div>
        
        <div *ngIf="andamentos.length === 0" class="text-center p-5 text-color-secondary flex-grow-1 flex flex-column justify-content-center">
          <i class="pi pi-clock text-4xl mb-3 block"></i>
          Nenhum andamento registrado.
        </div>

        <div *ngIf="andamentos.length > 0" class="mt-auto border-top-1 surface-border pt-3">
          <p-paginator (onPageChange)="onPageAndamento($event)" [first]="firstAndamento" [rows]="rowsAndamento" [totalRecords]="andamentos.length" [rowsPerPageOptions]="[5, 10, 20]" [showCurrentPageReport]="true" currentPageReportTemplate="{first} a {last} de {totalRecords}"></p-paginator>
        </div>
      </div>
    </p-card>
  `,
  styles: [`
    .text-truncate-2 {
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      overflow: hidden;
    }
    :host ::ng-deep .custom-timeline .p-timeline-event-opposite {
      display: none;
    }
  `]
})
export class ProcessoAndamentosCardComponent {
  @Input({ required: true }) andamentos!: AndamentoDto[];
  @Input() firstAndamento = 0;
  @Input() rowsAndamento = 5;

  @Output() addAndamento = new EventEmitter<void>();
  @Output() viewAndamento = new EventEmitter<AndamentoDto>();
  @Output() pageChange = new EventEmitter<{first: number, rows: number}>();

  andamentoView: 'timeline' | 'table' = 'timeline';
  andamentoViewOptions = [
    { icon: 'pi pi-calendar', value: 'timeline', tooltip: 'Ver em Timeline' },
    { icon: 'pi pi-table', value: 'table', tooltip: 'Ver em Tabela' }
  ];

  get paginatedAndamentos(): AndamentoDto[] {
    return this.andamentos.slice(this.firstAndamento, this.firstAndamento + this.rowsAndamento);
  }

  onPageAndamento(event: any) {
    this.pageChange.emit({ first: event.first, rows: event.rows });
  }
}
