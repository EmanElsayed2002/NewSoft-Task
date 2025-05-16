import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../Services/auth.service';
import { StudentService } from '../../Services/student.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-my-courses',
  templateUrl: './my-courses.component.html',
  styleUrl: './my-courses.component.css',
})
export class MyCoursesComponent implements OnInit {
  courses: any[] = [];
  constructor(private auth: StudentService, private toaster: ToastrService) {}
  ngOnInit(): void {
    this.auth.getUserSubjects().subscribe({
      next: (res: any) => {
        this.courses = res.data;
        console.log(this.courses);
      },
    });
  }

  removeCourse(courseId: number): void {
    const courseToRemove = this.courses.find(
      (course) => course.id === courseId
    );
    this.courses = this.courses.filter((course) => course.id !== courseId);

    this.auth.removeCourse(courseId).subscribe({
      next: (res) => {
        this.toaster.success('Course Removed successfully🎉', 'Success');
      },
      error: () => {
        if (courseToRemove) {
          this.courses = [...this.courses, courseToRemove];
          this.courses.sort((a, b) => a.id - b.id);
        }
        this.toaster.error(
          'Failed to remove the course. Please try again!',
          'Error'
        );
      },
    });
  }
}
