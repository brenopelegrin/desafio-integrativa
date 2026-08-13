import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { TipoEntidade } from '../models/enums';

export function documentoValidator(tipoEntidadeControl: AbstractControl): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value || !tipoEntidadeControl.value) {
      return null;
    }

    const tipo = tipoEntidadeControl.value;

    // formato do cpf:  xxx.xxx.xxx-xx
    const cpfRegex = /^\d{3}\.?\d{3}\.?\d{3}-?\d{2}$/;

    // formato do cnpj: xx.xxx.xxx/xxxx-xx
    const cnpjRegex = /^\d{2}\.?\d{3}\.?\d{3}\/?\d{4}-?\d{2}$/;

    if (tipo === TipoEntidade.PessoaFisica) {
      return cpfRegex.test(control.value) ? null : { invalidCpf: true };
    } else if (tipo === TipoEntidade.PessoaJuridica) {
      return cnpjRegex.test(control.value) ? null : { invalidCnpj: true };
    }

    return null;
  };
}
