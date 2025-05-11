import { Component, OnInit } from '@angular/core';
import { StudentService } from '../../Services/student.service';
import { NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
import { AuthService } from '../../Services/auth.service';

@Component({
  selector: 'app-course-list',
  templateUrl: './course-list.component.html',
  styleUrls: ['./course-list.component.scss'],
  providers: [NgbPaginationConfig],
})
export class CourseListComponent implements OnInit {
  courses: any[] = [];
  page = 1;
  pageSize = 5;
  collectionSize = 0;
  isLoading = false;

  constructor(
    private studentService: StudentService,
    private paginationConfig: NgbPaginationConfig,
    private authService: AuthService,
    private router: Router
  ) {
    paginationConfig.boundaryLinks = true;
    paginationConfig.rotate = true;
  }

  ngOnInit(): void {
    this.loadCourses();
  }

  loadCourses(): void {
    this.isLoading = true;
    this.studentService
      .getPaginatedCourses(this.page, this.pageSize)
      .subscribe({
        next: (response) => {
          this.courses = response;
          this.collectionSize = this.courses.length;
          this.isLoading = false;
        },
        error: (err) => {
          console.error('Error loading courses:', err);
          this.isLoading = false;
        },
      });
  }

  onPageChange(page: number): void {
    this.page = page;
    this.loadCourses();
  }

  joinCourse(courseId: number): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login'], {
        queryParams: { returnUrl: this.router.url },
      });
      return;
    }

    this.studentService.joinCourse(courseId).subscribe({
      next: () => {
        alert('Successfully joined the course!');
        this.loadCourses();
      },
      error: (err) => {
        console.error('Error joining course:', err);
        alert('Failed to join the course. Please try again.');
      },
    });
  }
}
