export interface BaseResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
  statusCode?: number;
}

export interface UserDto {
  id: string;
  userName?: string;
  email: string;
  isActive: boolean;
}

export enum TaskStatusEnum {
  None = 0,
  Pending = 1,
  InProgress = 2,
  Completed = 3,
  Archived = 4
}

export interface UserTaskDto {
  id: string;
  name: string;
  description?: string | null;
  userId: string;
  status: TaskStatusEnum;
  createdAt?: string | null;
  updatedAt?: string | null;
  deletedAt?: string | null;
}

export interface TaskListDto {
  userTasks: UserTaskDto[];
  totalCount: number;
}

export interface CreateTaskDto {
  name: string;
  description?: string | null;
  status: TaskStatusEnum;
}

export interface UpdateTaskDto extends CreateTaskDto {

}

export interface PaginationRequest {
  pageNumber: number;
  pageSize: number;
  searchValue?: string;
  sortField?: string;
  sortOrder: string;
  status?: TaskStatusEnum;
}
