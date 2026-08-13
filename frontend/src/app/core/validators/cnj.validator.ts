import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function cnjValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) {
      return null;
    }

    // formato do CNJ: NNNNNNN-DD.AAAA.J.TR.OOOO
    const cnjRegex = /^\d{7}-\d{2}\.\d{4}\.\d\.\d{2}\.\d{4}$/;

    const valid = cnjRegex.test(control.value);

    return valid ? null : { invalidCnj: true };
  };
}
