import { Component, Input, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { MessageService } from 'primeng/api';

import { EntidadesLegaisService } from '../../../core/services/entidades-legais.service';
import { TipoPolo, TipoEntidade } from '../../../core/models/enums';

@Component({
  selector: 'app-parte-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    SelectModule,
    InputTextModule,
    ButtonModule,
    TooltipModule
  ],
  templateUrl: './parte-form.component.html',
  styleUrls: ['./parte-form.component.scss']
})
export class ParteFormComponent {
  private entidadesService = inject(EntidadesLegaisService);
  private messageService = inject(MessageService);

  @Input({ required: true }) parteFormGroup!: FormGroup;
  @Input() showRemoveButton: boolean = false;

  tipoPoloOptions = [
    { label: 'Ativo', value: TipoPolo.Ativo },
    { label: 'Passivo', value: TipoPolo.Passivo }
  ];

  tipoEntidadeOptions = [
    { label: 'Pessoa Física (CPF)', value: TipoEntidade.PessoaFisica },
    { label: 'Pessoa Jurídica (CNPJ)', value: TipoEntidade.PessoaJuridica }
  ];

  buscarEntidadeLegal() {
    const docCtrl = this.parteFormGroup.get('novaEntidadeLegal.numeroDocumento');

    if (!docCtrl || docCtrl.invalid || !docCtrl.value) {
      return;
    }

    const doc = docCtrl.value.replace(/\D/g, '');

    this.entidadesService.getAll(1, 1, doc).subscribe({
      next: (res) => {
        if (res.items && res.items.length > 0) {
          const entidade = res.items[0];
          this.parteFormGroup.patchValue({
            entidadeLegalId: entidade.id,
            isExisting: true,
            novaEntidadeLegal: {
              nome: entidade.nome,
              tipoEntidade: entidade.tipoEntidade
            }
          });

          const nomeCtrl = this.parteFormGroup.get('novaEntidadeLegal.nome');
          nomeCtrl?.clearValidators();
          nomeCtrl?.updateValueAndValidity();

          this.messageService.add({ severity: 'success', summary: 'Sucesso', detail: 'Entidade Legal encontrada e vinculada.' });
        } else {
          this.parteFormGroup.patchValue({
            entidadeLegalId: null,
            isExisting: false,
            novaEntidadeLegal: {
              nome: ''
            }
          });

          const nomeCtrl = this.parteFormGroup.get('novaEntidadeLegal.nome');
          nomeCtrl?.setValidators([Validators.maxLength(200)]);
          nomeCtrl?.updateValueAndValidity();

          this.messageService.add({ severity: 'info', summary: 'Não encontrada', detail: 'Entidade Legal não encontrada. Você pode cadastrá-la agora preenchendo o nome.' });
        }
      }
    });
  }
}
