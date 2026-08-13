import { Component, OnInit, OnDestroy, inject, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { Subscription } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { SelectModule } from 'primeng/select';
import { TooltipModule } from 'primeng/tooltip';
import { TagModule } from 'primeng/tag';

import { EntidadesLegaisService } from '../../../core/services/entidades-legais.service';
import { EntidadeLegalDto } from '../../../core/models/responses';
import { TipoEntidade } from '../../../core/models/enums';
import { documentoValidator } from '../../../core/validators/documento.validator';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-entidade-legal-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, TableModule, ButtonModule, InputTextModule, DialogModule, SelectModule, TooltipModule, TagModule],
  templateUrl: './entidade-legal-list.component.html',
  styleUrl: './entidade-legal-list.component.scss'
})
export class EntidadeLegalListComponent implements OnInit, OnDestroy {
  private entidadesService = inject(EntidadesLegaisService);
  private fb = inject(FormBuilder);
  private messageService = inject(MessageService);

  @ViewChild('dt') dt: any;
  private changeSub?: Subscription;

  editMode = false;
  currentEditId?: string;

  entidades: EntidadeLegalDto[] = [];
  totalRecords: number = 0;
  loading: boolean = true;
  filterDocumento: string = '';

  showDialog = false;
  isSubmitting = false;
  entidadeForm: FormGroup;

  tipoEntidadeOptions = [
    { label: 'Pessoa Física', value: TipoEntidade.PessoaFisica },
    { label: 'Pessoa Jurídica', value: TipoEntidade.PessoaJuridica }
  ];

  constructor() {
    this.entidadeForm = this.fb.group({
      nome: ['', [Validators.required, Validators.maxLength(200)]],
      tipoEntidade: [TipoEntidade.PessoaFisica, Validators.required],
      numeroDocumento: ['', Validators.required]
    });

    // Adiciona o validador de documento
    const tipoCtrl = this.entidadeForm.get('tipoEntidade')!;
    const docCtrl = this.entidadeForm.get('numeroDocumento')!;
    docCtrl.setValidators([Validators.required, documentoValidator(tipoCtrl)]);

    tipoCtrl.valueChanges.subscribe(() => {
      docCtrl.updateValueAndValidity();
    });
  }

  ngOnInit() {
    this.changeSub = this.entidadesService.entidadeChanged$.subscribe(() => {
      if (this.dt) {
        this.dt.reset();
      }
    });
  }

  ngOnDestroy() {
    this.changeSub?.unsubscribe();
  }

  loadEntidades(event: TableLazyLoadEvent) {
    this.loading = true;
    const pageNumber = event.first! / event.rows! + 1;
    const pageSize = event.rows!;

    this.entidadesService.getAll(pageNumber, pageSize, this.filterDocumento || undefined).subscribe({
      next: (res) => {
        this.entidades = res.items;
        this.totalRecords = res.totalCount;
        this.loading = false;
      },
      error: () => this.loading = false
    });
  }

  applyFilter(table: any) {
    table.reset();
  }

  openNew() {
    this.editMode = false;
    this.currentEditId = undefined;
    this.entidadeForm.reset();
    this.entidadeForm.patchValue({ tipoEntidade: TipoEntidade.PessoaFisica });
    this.showDialog = true;
  }

  editEntidade(entidade: EntidadeLegalDto) {
    this.editMode = true;
    this.currentEditId = entidade.id;
    this.entidadeForm.patchValue({
      tipoEntidade: entidade.tipoEntidade,
      numeroDocumento: entidade.numeroDocumento,
      nome: entidade.nome
    });

    // Deixa o input do documento desabilitado, pois o backend não permite editar
    this.entidadeForm.get('tipoEntidade')?.disable();
    this.entidadeForm.get('numeroDocumento')?.disable();

    this.showDialog = true;
  }

  save() {
    if (this.entidadeForm.invalid) {
      this.entidadeForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.entidadeForm.getRawValue();

    if (this.editMode && this.currentEditId) {
      // Update
      this.entidadesService.update(this.currentEditId, { nome: formValue.nome }).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Entidade legal atualizada com sucesso.' });
          this.closeDialog();
        },
        error: () => {
          this.isSubmitting = false;
        }
      });
    } else {
      // Create
      this.entidadesService.create(formValue).subscribe({
        next: () => {
          this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Entidade legal criada com sucesso.' });
          this.closeDialog();
        },
        error: () => {
          this.isSubmitting = false;
        }
      });
    }
  }

  closeDialog() {
    this.showDialog = false;
    this.isSubmitting = false;

    // Reseta os "disables" dos inputs
    this.entidadeForm.get('tipoEntidade')?.enable();
    this.entidadeForm.get('numeroDocumento')?.enable();
  }

  getTipoLabel(tipo: TipoEntidade): string {
    return tipo === TipoEntidade.PessoaFisica ? 'Pessoa Física' : 'Pessoa Jurídica';
  }
}
