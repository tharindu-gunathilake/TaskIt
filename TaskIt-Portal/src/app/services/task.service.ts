import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  BaseResponse,
  CreateTaskDto,
  PaginationRequest,
  TaskListDto,
  UpdateTaskDto,
  UserTaskDto
} from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class TaskService {
  constructor(private readonly http: HttpClient) {}

  getTasks(pagination: PaginationRequest, userId: string): Observable<BaseResponse<TaskListDto>> {
    let params = new HttpParams()
      .set('pageNumber', pagination.pageNumber.toString())
      .set('pageSize', pagination.pageSize.toString())
      .set('sortOrder', pagination.sortOrder)
      .set('userId', userId);

    if (pagination.searchValue) {
      params = params.set('searchValue', pagination.searchValue);
    }

    if (pagination.sortField) {
      params = params.set('sortField', pagination.sortField);
    }

    if (pagination.status !== undefined) {
      console.log('ps : ', pagination.status)
      params = params.set('status', pagination.status.toString());
    }

    return this.http.get<BaseResponse<TaskListDto>>(`${environment.apiBaseUrl}/Task`, {
      params
    });
  }

  getTask(id: string): Observable<BaseResponse<UserTaskDto>> {
    return this.http.get<BaseResponse<UserTaskDto>>(`${environment.apiBaseUrl}/Task/${id}`);
  }

  createTask(payload: CreateTaskDto): Observable<BaseResponse<UserTaskDto>> {
    return this.http.post<BaseResponse<UserTaskDto>>(`${environment.apiBaseUrl}/Task`, payload);
  }

  updateTask(id: string, payload: UpdateTaskDto): Observable<BaseResponse<UserTaskDto>> {
    return this.http.put<BaseResponse<UserTaskDto>>(`${environment.apiBaseUrl}/Task/${id}`, payload);
  }

  deleteTask(id: string): Observable<BaseResponse<boolean>> {
    return this.http.delete<BaseResponse<boolean>>(`${environment.apiBaseUrl}/Task/${id}`);
  }
}
