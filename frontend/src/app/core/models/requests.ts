import { TipoPolo, TipoEntidade, TipoProcesso, StatusProcesso } from './enums';

export interface CreateEntidadeLegalDto {
  nome: string;
  tipoEntidade: TipoEntidade;
  numeroDocumento: string;
}

export interface UpdateEntidadeLegalDto {
  nome: string;
}

export interface AddAndamentoDto {
  dataAndamento: string; // ISO 8601 string
  descricao: string;
}

export interface AddParteDto {
  tipoPolo: TipoPolo;
  entidadeLegalId?: string;
  novaEntidadeLegal?: CreateEntidadeLegalDto;
}

export interface AddParteProcessoDto {
  tipoPolo: TipoPolo;
  entidadeLegalId?: string;
  novaEntidadeLegal?: CreateEntidadeLegalDto;
}

export interface CreateProcessoDto {
  numeroProcesso: string;
  tipoProcesso: TipoProcesso;
  assunto: string;
  partes: AddParteProcessoDto[];
}

export interface UpdateProcessoDto {
  assunto: string;
  status: StatusProcesso;
}
