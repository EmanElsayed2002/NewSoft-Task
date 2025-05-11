import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class StudentService {
  private readonly apiUrl: string = 'https://localhost:7073/api';

  constructor(private http: HttpClient) {}

  getPaginatedCourses(
    page: number = 1,
    pageSize: number = 10
  ): Observable<any> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get(`${this.apiUrl}/instructor/get-all-subjects`, {
      params,
    });
  }

  joinCourse(courseId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/instructor/add-user-subject`, {
      subjectId: courseId,
      userId: this.getCurrentUserId(),
    });
  }

  private getCurrentUserId(): number {
    return 1;
  }
}
