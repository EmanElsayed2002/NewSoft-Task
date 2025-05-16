import { Component, OnInit } from '@angular/core';
import { StudentService } from '../../Services/student.service';
import { NgbPaginationConfig } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';
import { AuthService } from '../../Services/auth.service';
import { ToastrService } from 'ngx-toastr';

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
    private router: Router,
    private toaster: ToastrService
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
          console.log(response, this.page, this.pageSize);
          this.courses = response.subjects;
          this.collectionSize = response.totalSubjects;
          this.isLoading = false;
        },
        error: (err) => {
          this.toaster.error(
            'Failed to join the course. Please try again!',
            'Error'
          );
          this.isLoading = false;
        },
      });
  }

  onPageChange(page: number): void {
    this.page = page;
    this.loadCourses();
  }

  joinCourse(courseId: number): void {
    console.log(courseId);
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login'], {
        queryParams: { returnUrl: this.router.url },
      });
      return;
    }

    this.studentService.joinCourse(courseId).subscribe({
      next: () => {
        this.toaster.success('Successfully joined the course!', 'Success 🎉');
        this.loadCourses();
      },
      error: (err) => {
        this.toaster.error(
          'Failed to join the course. Please try again!',
          'Error'
        );
      },
    });
  }
}
