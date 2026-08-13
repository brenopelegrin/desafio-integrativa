import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { Observable, Subject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { PaginatedListDto, ProcessoDto } from '../models/responses';
import { CreateProcessoDto, UpdateProcessoDto, AddParteDto } from '../models/requests';
import { StatusProcesso } from '../models/enums';

@Injectable({
  providedIn: 'root'
})
export class ProcessosService {
  private api = inject(ApiService);
  private readonly route = '/processos';
  
  private processoChangedSource = new Subject<void>();
  processoChanged$ = this.processoChangedSource.asObservable();

  getAll(pageNumber: number = 1, pageSize: number = 10, numeroDocumentoParte?: string, statusProcesso?: StatusProcesso, id?: string, numeroProcesso?: string): Observable<PaginatedListDto<ProcessoDto>> {
    return this.api.get<PaginatedListDto<ProcessoDto>>(this.route, { pageNumber, pageSize, numeroDocumentoParte, statusProcesso, id, numeroProcesso });
  }

  getById(id: string): Observable<ProcessoDto> {
    return this.api.get<ProcessoDto>(`${this.route}/${id}`);
  }

  create(dto: CreateProcessoDto): Observable<ProcessoDto> {
    return this.api.post<ProcessoDto>(this.route, dto).pipe(
      tap(() => this.processoChangedSource.next())
    );
  }

  update(id: string, dto: UpdateProcessoDto): Observable<ProcessoDto> {
    return this.api.patch<ProcessoDto>(`${this.route}/${id}`, dto).pipe(
      tap(() => this.processoChangedSource.next())
    );
  }

  delete(id: string): Observable<void> {
    return this.api.delete<void>(`${this.route}/${id}`).pipe(
      tap(() => this.processoChangedSource.next())
    );
  }

  removeParte(processoId: string, parteId: string): Observable<ProcessoDto> {
    return this.api.delete<ProcessoDto>(`${this.route}/${processoId}/partes/${parteId}`).pipe(
      tap(() => this.processoChangedSource.next())
    );
  }

  addParte(processoId: string, dto: AddParteDto): Observable<ProcessoDto> {
    return this.api.post<ProcessoDto>(`${this.route}/${processoId}/partes`, dto).pipe(
      tap(() => this.processoChangedSource.next())
    );
  }
}
