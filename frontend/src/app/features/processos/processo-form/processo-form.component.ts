import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray, AbstractControl } from '@angular/forms';
import { StepperModule } from 'primeng/stepper';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { DialogModule } from 'primeng/dialog';
import { CardModule } from 'primeng/card';
import { TooltipModule } from 'primeng/tooltip';

import { ProcessosService } from '../../../core/services/processos.service';
import { EntidadesLegaisService } from '../../../core/services/entidades-legais.service';
import { MessageService } from 'primeng/api';
import { cnjValidator } from '../../../core/validators/cnj.validator';
import { TipoProcesso, TipoPolo, TipoEntidade } from '../../../core/models/enums';
import { documentoValidator } from '../../../core/validators/documento.validator';
import { ParteFormComponent } from '../../../shared/components/parte-form/parte-form.component';

@Component({
  selector: 'app-processo-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, StepperModule, ButtonModule, InputTextModule, SelectModule, DialogModule, CardModule, TooltipModule, RouterModule, ParteFormComponent],
  templateUrl: './processo-form.component.html',
  styleUrl: './processo-form.component.scss'
})
export class ProcessoFormComponent {
  private fb = inject(FormBuilder);
  private processosService = inject(ProcessosService);
  private entidadesService = inject(EntidadesLegaisService);
  private router = inject(Router);
  private messageService = inject(MessageService);

  processoForm: FormGroup;

  tipoProcessoOptions = [
    { label: 'Judicial', value: TipoProcesso.Judicial },
    { label: 'Administrativo', value: TipoProcesso.Administrativo }
  ];

  isSubmitting = false;

  constructor() {
    this.processoForm = this.fb.group({
      info: this.fb.group({
        tipoProcesso: [null, Validators.required],
        numeroProcesso: ['', Validators.required],
        assunto: ['', [Validators.required, Validators.maxLength(500)]]
      }),
      partes: this.fb.array([])
    });

    // Atualiza os validators de acordo com o input de TipoProcesso
    // Juidicial -> CNJ
    // Administrativo -> sem validação
    this.processoForm.get('info.tipoProcesso')?.valueChanges.subscribe(tipo => {
      const numControl = this.processoForm.get('info.numeroProcesso');
      if (tipo === TipoProcesso.Judicial) {
        numControl?.setValidators([Validators.required, cnjValidator()]);
      } else {
        numControl?.setValidators([Validators.required]);
      }
      numControl?.updateValueAndValidity();
    });
  }

  get partesFormArray(): FormArray {
    return this.processoForm.get('partes') as FormArray;
  }

  getParteFormGroup(index: number): FormGroup {
    return this.partesFormArray.at(index) as FormGroup;
  }

  addParte() {
    const parteGroup = this.fb.group({
      tipoPolo: [null, Validators.required],
      entidadeLegalId: [null],
      isExisting: [false], // flag caso encontrarmos uma Entidade existente
      novaEntidadeLegal: this.fb.group({
        nome: ['', Validators.maxLength(200)],
        tipoEntidade: [TipoEntidade.PessoaFisica],
        numeroDocumento: ['']
      })
    });

    const novaEntidade = parteGroup.get('novaEntidadeLegal') as FormGroup;
    const tipoEntidadeCtrl = novaEntidade.get('tipoEntidade') as AbstractControl;
    const docCtrl = novaEntidade.get('numeroDocumento') as AbstractControl;

    // Setup do validator de CPF/CNPJ
    docCtrl.setValidators([Validators.required, documentoValidator(tipoEntidadeCtrl)]);

    // Quando o tipo de entidade muda, precisa atualizar o validator de documento
    tipoEntidadeCtrl.valueChanges.subscribe(() => {
      docCtrl.updateValueAndValidity();
    });

    this.partesFormArray.push(parteGroup);
  }

  removeParte(index: number) {
    this.partesFormArray.removeAt(index);
  }

  validatePartes(): boolean {
    const partes = this.partesFormArray.value as any[];
    const hasAtivo = partes.some(p => p.tipoPolo === TipoPolo.Ativo);
    const hasPassivo = partes.some(p => p.tipoPolo === TipoPolo.Passivo);
    return hasAtivo && hasPassivo;
  }

  submit() {
    if (this.processoForm.invalid || !this.validatePartes()) {
      this.processoForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const formValue = this.processoForm.value;

    const payload = {
      numeroProcesso: formValue.info.numeroProcesso,
      tipoProcesso: formValue.info.tipoProcesso,
      assunto: formValue.info.assunto,
      partes: (formValue.partes as any[]).map(p => {
        if (p.isExisting) {
          return {
            tipoPolo: p.tipoPolo,
            entidadeLegalId: p.entidadeLegalId
          };
        } else {
          return {
            tipoPolo: p.tipoPolo,
            novaEntidadeLegal: p.novaEntidadeLegal
          };
        }
      })
    };

    this.processosService.create(payload).subscribe({
      next: (res) => {
        this.router.navigate(['/processos', res.id]);
      },
      error: () => {
        this.isSubmitting = false;
      }
    });
  }
}
