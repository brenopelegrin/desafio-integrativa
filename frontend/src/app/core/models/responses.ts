import { StatusProcesso, TipoEntidade, TipoPolo, TipoProcesso } from './enums';

export interface EntidadeLegalDto {
  id: string;
  nome: string;
  tipoEntidade: TipoEntidade;
  numeroDocumento: string;
}

export interface AndamentoDto {
  id: string;
  data: string; // data no formato iso 8601 com timezone
  descricao: string;
}

export interface ParteDto {
  id: string;
  tipoPolo: TipoPolo;
  entidadeLegal: EntidadeLegalDto;
}

export interface ProcessoDto {
  id: string;
  numeroProcesso: string;
  tipoProcesso: TipoProcesso;
  assunto: string;
  dataCriacao: string; // data no formato iso 8601 com timezone
  status: StatusProcesso;
  partes: ParteDto[];
  andamentos: AndamentoDto[];
}

export interface PaginatedListDto<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}
