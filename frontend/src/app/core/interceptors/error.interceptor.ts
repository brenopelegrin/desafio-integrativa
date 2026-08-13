import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MessageService } from 'primeng/api';
import { catchError, throwError } from 'rxjs';

// Extrai a mensagem de erro da resposta do backend
const extractErrorMessage = (error: HttpErrorResponse): string => {
  const errData = error.error;

  if (typeof errData === 'string') return errData;
  if (errData?.error && typeof errData.error === 'string') return errData.error;
  if (errData?.errors) return Object.values(errData.errors).flat().join('\n');

  return errData?.detail
    || errData?.title
    || errData?.message
    || error.message
    || 'Ocorreu um erro inesperado.';
};

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const messageService = inject(MessageService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const errorMsg = extractErrorMessage(error);

      // Mostra uma notificação para erros do backend, com a mensagem
      if ([400, 422, 500].includes(error.status)) {
        messageService.add({
          severity: 'error',
          summary: 'Erro',
          detail: errorMsg,
          life: 5000
        });
      }

      return throwError(() => error);
    })
  );
};
