import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { PaginatedListDto, ParteDto, ProcessoDto } from '../models/responses';
import { AddParteDto } from '../models/requests';

@Injectable({
  providedIn: 'root'
})
export class PartesService {
  private api = inject(ApiService);

  getAll(processoId: string, pageNumber: number = 1, pageSize: number = 10): Observable<PaginatedListDto<ParteDto>> {
    return this.api.get<PaginatedListDto<ParteDto>>(`/processos/${processoId}/partes`, { pageNumber, pageSize });
  }

  addParte(processoId: string, dto: AddParteDto): Observable<ProcessoDto> {
    return this.api.post<ProcessoDto>(`/processos/${processoId}/partes`, dto);
  }

  removeParte(processoId: string, parteId: string): Observable<ProcessoDto> {
    return this.api.delete<ProcessoDto>(`/processos/${processoId}/partes/${parteId}`);
  }
}
