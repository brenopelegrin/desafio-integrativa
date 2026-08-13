import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;

  get<T>(url: string, params?: any): Observable<T> {
    const httpParams = this.buildParams(params);
    return this.http.get<T>(`${this.baseUrl}${url}`, { params: httpParams });
  }

  post<T>(url: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}${url}`, this.serializePayload(body));
  }

  patch<T>(url: string, body: any): Observable<T> {
    return this.http.patch<T>(`${this.baseUrl}${url}`, this.serializePayload(body));
  }

  delete<T>(url: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}${url}`);
  }

  private serializePayload(body: any): any {
    if (body === null || body === undefined) return body;

    if (body instanceof Date) {
      return body.toISOString();
    }

    if (Array.isArray(body)) {
      return body.map(item => this.serializePayload(item));
    }

    if (typeof body === 'object') {
      const serialized: any = {};
      for (const key of Object.keys(body)) {
        serialized[key] = this.serializePayload(body[key]);
      }
      return serialized;
    }

    return body;
  }

  private buildParams(params: any): HttpParams {
    let httpParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach(key => {
        if (params[key] !== null && params[key] !== undefined) {
          httpParams = httpParams.append(key, params[key]);
        }
      });
    }
    return httpParams;
  }
}
