import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { PaginatedListDto, AndamentoDto, ProcessoDto } from '../models/responses';
import { AddAndamentoDto } from '../models/requests';

@Injectable({
  providedIn: 'root'
})
export class AndamentosService {
  private api = inject(ApiService);

  getAll(processoId: string, pageNumber: number = 1, pageSize: number = 10): Observable<PaginatedListDto<AndamentoDto>> {
    return this.api.get<PaginatedListDto<AndamentoDto>>(`/processos/${processoId}/andamentos`, { pageNumber, pageSize });
  }

  addAndamento(processoId: string, dto: AddAndamentoDto): Observable<ProcessoDto> {
    return this.api.post<ProcessoDto>(`/processos/${processoId}/andamentos`, dto);
  }
}
