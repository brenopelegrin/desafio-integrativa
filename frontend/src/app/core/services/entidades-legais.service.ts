import { Injectable, inject } from '@angular/core';
import { ApiService } from './api.service';
import { Observable, Subject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { PaginatedListDto, EntidadeLegalDto } from '../models/responses';
import { CreateEntidadeLegalDto, UpdateEntidadeLegalDto } from '../models/requests';

@Injectable({
  providedIn: 'root'
})
export class EntidadesLegaisService {
  private api = inject(ApiService);
  private readonly route = '/entidades-legais';

  private entidadeChangedSource = new Subject<void>();
  entidadeChanged$ = this.entidadeChangedSource.asObservable();

  getAll(pageNumber: number = 1, pageSize: number = 10, numeroDocumento?: string): Observable<PaginatedListDto<EntidadeLegalDto>> {
    return this.api.get<PaginatedListDto<EntidadeLegalDto>>(this.route, { pageNumber, pageSize, numeroDocumento });
  }

  getById(id: string): Observable<EntidadeLegalDto> {
    return this.api.get<EntidadeLegalDto>(`${this.route}/${id}`);
  }

  create(dto: CreateEntidadeLegalDto): Observable<EntidadeLegalDto> {
    return this.api.post<EntidadeLegalDto>(this.route, dto).pipe(
      tap(() => this.entidadeChangedSource.next())
    );
  }

  update(id: string, dto: UpdateEntidadeLegalDto): Observable<EntidadeLegalDto> {
    return this.api.patch<EntidadeLegalDto>(`${this.route}/${id}`, dto).pipe(
      tap(() => this.entidadeChangedSource.next())
    );
  }
}
