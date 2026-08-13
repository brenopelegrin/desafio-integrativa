import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { CardModule } from 'primeng/card';
import { TimelineModule } from 'primeng/timeline';
import { ButtonModule } from 'primeng/button';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { DatePickerModule } from 'primeng/datepicker';
import { SelectButtonModule } from 'primeng/selectbutton';
import { TableModule } from 'primeng/table';
import { PaginatorModule } from 'primeng/paginator';
import { MessageService, ConfirmationService } from 'primeng/api';
import { TooltipModule } from 'primeng/tooltip';
import { SelectModule } from 'primeng/select';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

import { ProcessosService } from '../../../core/services/processos.service';
import { AndamentosService } from '../../../core/services/andamentos.service';
import { EntidadesLegaisService } from '../../../core/services/entidades-legais.service';
import { ProcessoDto, AndamentoDto } from '../../../core/models/responses';
import { StatusProcesso, TipoProcesso, TipoPolo, TipoEntidade } from '../../../core/models/enums';
import { documentoValidator } from '../../../core/validators/documento.validator';
import { StatusTagComponent } from '../../../shared/ui/status-tag/status-tag.component';
import { ParteFormComponent } from '../../../shared/components/parte-form/parte-form.component';

import { ProcessoHeaderComponent } from './components/processo-header/processo-header.component';
import { ProcessoDetailsCardComponent } from './components/processo-details-card/processo-details-card.component';
import { ProcessoPartesCardComponent } from './components/processo-partes-card/processo-partes-card.component';
import { ProcessoAndamentosCardComponent } from './components/processo-andamentos-card/processo-andamentos-card.component';

@Component({
  selector: 'app-processo-detail',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule, CardModule, TimelineModule,
    ButtonModule, DialogModule, InputTextModule, TextareaModule, DatePickerModule,
    SelectButtonModule, TableModule, PaginatorModule, TooltipModule, SelectModule,
    ConfirmDialogModule, RouterModule, ParteFormComponent, TagModule,
    ProcessoHeaderComponent, ProcessoDetailsCardComponent, ProcessoPartesCardComponent, ProcessoAndamentosCardComponent
  ],
  templateUrl: './processo-detail.component.html',
  styleUrl: './processo-detail.component.scss'
})
export class ProcessoDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private processosService = inject(ProcessosService);
  private andamentosService = inject(AndamentosService);
  private entidadesService = inject(EntidadesLegaisService);
  private fb = inject(FormBuilder);
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  processoId: string = '';
  processo: ProcessoDto | null = null;
  andamentos: AndamentoDto[] = [];

  showAndamentoDialog = false;
  andamentoForm: FormGroup;
  isSubmittingAndamento = false;

  firstAndamento = 0;
  rowsAndamento = 5;

  showViewAndamentoDialog = false;
  selectedAndamento: AndamentoDto | null = null;

  isSavingAssunto = false;

  // Modal para Add Partes
  showAddParteDialog = false;
  isSubmittingParte = false;
  parteForm: FormGroup;

  // Modal para editar Status
  showEditStatusDialog = false;
  isSubmittingStatus = false;
  statusForm: FormGroup;
  editStatusOptions = [
    { label: 'Ativo', value: StatusProcesso.Ativo },
    { label: 'Finalizado', value: StatusProcesso.Finalizado },
    { label: 'Arquivado', value: StatusProcesso.Arquivado }
  ];

  constructor() {
    this.andamentoForm = this.fb.group({
      dataAndamento: [new Date(), Validators.required],
      descricao: ['', Validators.required]
    });

    this.statusForm = this.fb.group({
      status: [null, Validators.required]
    });

    this.parteForm = this.fb.group({
      tipoPolo: [null, Validators.required],
      entidadeLegalId: [null],
      isExisting: [false],
      novaEntidadeLegal: this.fb.group({
        nome: ['', Validators.maxLength(200)],
        tipoEntidade: [TipoEntidade.PessoaFisica],
        numeroDocumento: ['']
      })
    });

    const novaEntidade = this.parteForm.get('novaEntidadeLegal') as FormGroup;
    const tipoEntidadeCtrl = novaEntidade.get('tipoEntidade') as AbstractControl;
    const docCtrl = novaEntidade.get('numeroDocumento') as AbstractControl;

    docCtrl.setValidators([Validators.required, documentoValidator(tipoEntidadeCtrl)]);

    tipoEntidadeCtrl.valueChanges.subscribe(() => {
      docCtrl.updateValueAndValidity();
    });
  }

  ngOnInit() {
    this.route.paramMap.subscribe(params => {
      this.processoId = params.get('id') || '';
      if (this.processoId) {
        this.loadProcesso();
      }
    });
  }

  loadProcesso() {
    this.processosService.getById(this.processoId).subscribe({
      next: (res) => {
        this.processo = res;
        // Sort andamentos descending for timeline/table
        this.andamentos = [...res.andamentos].sort((a, b) => new Date(b.data).getTime() - new Date(a.data).getTime());
      }
    });
  }

  handleSaveAssunto(novoAssunto: string) {
    if (this.processo) {
      this.isSavingAssunto = true;
      const updateDto = { assunto: novoAssunto };
      this.processosService.update(this.processoId, { ...this.processo, assunto: novoAssunto }).subscribe({
        next: () => {
          if (this.processo) this.processo.assunto = novoAssunto;
          this.isSavingAssunto = false;
          this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Assunto atualizado.' });
        },
        error: () => {
          this.isSavingAssunto = false;
        }
      });
    }
  }

  removeParte(parteId: string) {
    this.confirmationService.confirm({
      header: 'Confirmação',
      message: 'Tem certeza que deseja remover esta parte do processo?',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sim',
      rejectLabel: 'Não',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.processosService.removeParte(this.processoId, parteId).subscribe({
          next: (res) => {
            this.processo = res;
            this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Parte removida com sucesso.' });
          },
          error: (err) => {
            // o interceptor de erro vai mostrar a notificação de erro
          }
        });
      }
    });
  }

  handlePageAndamento(event: { first: number, rows: number }) {
    this.firstAndamento = event.first;
    this.rowsAndamento = event.rows;
  }

  viewAndamento(andamento: AndamentoDto) {
    this.selectedAndamento = andamento;
    this.showViewAndamentoDialog = true;
  }

  openAndamentoDialog() {
    const brtDateStr = new Date().toLocaleString("en-US", { timeZone: "America/Sao_Paulo" });
    this.andamentoForm.reset({ dataAndamento: new Date(brtDateStr) });
    this.showAndamentoDialog = true;
  }

  submitAndamento() {
    if (this.andamentoForm.invalid) return;

    this.isSubmittingAndamento = true;
    this.andamentosService.addAndamento(this.processoId, this.andamentoForm.value).subscribe({
      next: (res) => {
        this.showAndamentoDialog = false;
        this.isSubmittingAndamento = false;
        this.loadProcesso(); // atualiza pra pegar os novos andamentos
      },
      error: () => {
        this.isSubmittingAndamento = false;
      }
    });
  }

  // -- Lógica do modal de Add Parte --

  openAddParteDialog() {
    this.parteForm.reset({
      tipoPolo: null,
      entidadeLegalId: null,
      isExisting: false,
      novaEntidadeLegal: {
        nome: '',
        tipoEntidade: TipoEntidade.PessoaFisica,
        numeroDocumento: ''
      }
    });
    this.showAddParteDialog = true;
  }

  closeAddParteDialog() {
    this.showAddParteDialog = false;
    this.isSubmittingParte = false;
  }

  submitParte() {
    if (this.parteForm.invalid) {
      this.parteForm.markAllAsTouched();
      return;
    }

    this.isSubmittingParte = true;
    const formValue = this.parteForm.value;

    let payload: any = {
      tipoPolo: formValue.tipoPolo
    };

    if (formValue.isExisting) {
      payload.entidadeLegalId = formValue.entidadeLegalId;
    } else {
      payload.novaEntidadeLegal = formValue.novaEntidadeLegal;
    }

    this.processosService.addParte(this.processoId, payload).subscribe({
      next: (res) => {
        this.processo = res;
        this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Parte adicionada com sucesso.' });
        this.closeAddParteDialog();
      },
      error: () => {
        this.isSubmittingParte = false;
      }
    });
  }

  // --- Lógica do modal de Status ---

  openEditStatusDialog() {
    if (!this.processo) return;
    this.statusForm.reset({
      status: this.processo.status
    });
    this.showEditStatusDialog = true;
  }

  closeEditStatusDialog() {
    this.showEditStatusDialog = false;
    this.isSubmittingStatus = false;
  }

  submitStatus() {
    if (this.statusForm.invalid || !this.processo) return;

    this.isSubmittingStatus = true;
    const newStatus = this.statusForm.value.status;

    this.processosService.update(this.processo.id, {
      status: newStatus,
      assunto: this.processo.assunto
    }).subscribe({
      next: (res) => {
        this.processo = res;
        this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Status atualizado com sucesso.' });
        this.closeEditStatusDialog();
      },
      error: () => {
        this.isSubmittingStatus = false;
      }
    });
  }
}
