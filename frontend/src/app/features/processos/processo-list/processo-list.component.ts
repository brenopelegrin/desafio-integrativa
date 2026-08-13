import { Component, OnInit, inject, ViewChild, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { TableModule, TableLazyLoadEvent, Table } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TooltipModule } from 'primeng/tooltip';
import { DialogModule } from 'primeng/dialog';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

import { ProcessosService } from '../../../core/services/processos.service';
import { ProcessoDto } from '../../../core/models/responses';
import { StatusProcesso, TipoProcesso } from '../../../core/models/enums';
import { StatusTagComponent } from '../../../shared/ui/status-tag/status-tag.component';

@Component({
  selector: 'app-processo-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, TableModule, ButtonModule, InputTextModule, SelectModule, TooltipModule, DialogModule, ConfirmDialogModule, StatusTagComponent],
  templateUrl: './processo-list.component.html',
  styleUrl: './processo-list.component.scss'
})
export class ProcessoListComponent implements OnInit {
  private processosService = inject(ProcessosService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);
  private destroyRef = inject(DestroyRef);

  @ViewChild('dt') dt!: Table;

  processos: ProcessoDto[] = [];
  totalRecords: number = 0;
  loading: boolean = true;
  StatusProcesso = StatusProcesso;

  // Filtros
  filterNumeroProcesso: string = '';
  filterDocumento: string = '';
  filterStatus: StatusProcesso | undefined;

  statusOptions = [
    { label: 'Todos', value: null },
    { label: 'Ativo', value: StatusProcesso.Ativo },
    { label: 'Finalizado', value: StatusProcesso.Finalizado },
    { label: 'Arquivado', value: StatusProcesso.Arquivado }
  ];

  editStatusOptions = [
    { label: 'Ativo', value: StatusProcesso.Ativo },
    { label: 'Finalizado', value: StatusProcesso.Finalizado },
    { label: 'Arquivado', value: StatusProcesso.Arquivado }
  ];

  showEditDialog = false;
  editForm: FormGroup;
  isSubmitting = false;
  selectedProcessoId = '';

  constructor() {
    this.editForm = this.fb.group({
      assunto: ['', Validators.required],
      status: [null, Validators.required]
    });
  }

  ngOnInit() {
    this.processosService.processoChanged$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        if (this.dt) {
          this.dt.reset();
        }
      });
  }

  loadProcessos(event: TableLazyLoadEvent) {
    this.loading = true;
    const pageNumber = event.first! / event.rows! + 1;
    const pageSize = event.rows!;

    this.processosService.getAll(
      pageNumber, pageSize,
      this.filterDocumento || undefined,
      this.filterStatus || undefined,
      undefined, // podemos pesquisar por ID tbm, mas está desabilitado por enquanto
      this.filterNumeroProcesso || undefined
    ).subscribe({
      next: (res) => {
        this.processos = res.items;
        this.totalRecords = res.totalCount;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  applyFilters(table: any) {
    table.reset();
  }

  goToNovoProcesso() {
    this.router.navigate(['/processos/novo']);
  }

  viewDetail(processo: ProcessoDto) {
    this.router.navigate(['/processos', processo.id]);
  }

  editProcesso(processo: ProcessoDto) {
    this.selectedProcessoId = processo.id;
    this.editForm.patchValue({
      assunto: processo.assunto,
      status: processo.status
    });
    this.showEditDialog = true;
  }

  saveEdit() {
    if (this.editForm.invalid) return;
    this.isSubmitting = true;
    this.processosService.update(this.selectedProcessoId, this.editForm.value).subscribe({
      next: () => {
        this.showEditDialog = false;
        this.isSubmitting = false;
        this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Processo atualizado com sucesso.' });
      },
      error: () => {
        this.isSubmitting = false;
      }
    });
  }

  deleteProcesso(processo: ProcessoDto) {
    if (processo.status === StatusProcesso.Ativo) {
      this.messageService.add({
        severity: 'warn',
        summary: 'Ação não permitida',
        detail: 'Apenas processos com status Arquivado ou Finalizado podem ser excluídos.'
      });
      return;
    }

    this.confirmationService.confirm({
      header: 'Confirmar exclusão',
      message: `Tem certeza que deseja excluir o processo ${processo.numeroProcesso}? Esta ação não pode ser desfeita.`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Excluir',
      rejectLabel: 'Cancelar',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.processosService.delete(processo.id).subscribe({
          next: () => {
            this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Processo excluído com sucesso.' });
          }
        });
      }
    });
  }

  getTipoProcessoLabel(tipo: TipoProcesso): string {
    return tipo === TipoProcesso.Judicial ? 'Judicial' : 'Administrativo';
  }
}
