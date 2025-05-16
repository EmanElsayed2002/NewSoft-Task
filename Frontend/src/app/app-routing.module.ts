import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterComponent } from './components/register/register.component';
import { ConfirmEmailComponent } from './components/confirm-email/confirm-email.component';
import { CourseListComponent } from './components/course-list/course-list.component';
import { LoginComponent } from './components/login/login.component';
import { MyCoursesComponent } from './components/my-courses/my-courses.component';
import { authGuard } from './Gaurds/guard.guard';

const routes: Routes = [
  { path: '', component: CourseListComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'login', component: LoginComponent },
  {
    path: 'confirm-email',
    component: ConfirmEmailComponent,
  },
  {
    path: 'my-courses',
    component: MyCoursesComponent,
    canActivate: [authGuard],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
