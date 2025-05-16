import { Component } from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { RegisterRequestDTO } from '../../Models/RegisterRequestDTO .model';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
  registrationForm: FormGroup;
  submitted = false;
  showConfirmationMessage = false;
  emailSentTo: string = '';
  resendLoading = false;
  resendSuccess = false;
  confirmationLoading = true;
  confirmationError = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private toaster: ToastrService
  ) {
    this.registrationForm = new FormGroup(
      {
        FullName: new FormControl('', [
          Validators.required,
          Validators.minLength(3),
        ]),
        Email: new FormControl('', [
          Validators.required,
          Validators.pattern(
            /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/
          ),
        ]),
        Address: new FormControl('', [
          Validators.required,
          Validators.minLength(5),
        ]),
        Password: new FormControl('', [
          Validators.required,
          Validators.pattern(
            /^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{8,}$/
          ),
        ]),
        ConfirmedPassword: new FormControl('', [Validators.required]),
        Phone: new FormControl('', [
          Validators.required,
          Validators.pattern(/^\+?[0-9]{8,15}$/),
        ]),
        Age: new FormControl('', [Validators.required, Validators.min(10)]),
        Role: new FormControl('', [Validators.required]),
      },
      { validators: this.passwordMatchValidator }
    );
  }

  passwordMatchValidator: ValidatorFn = (formGroup: AbstractControl) => {
    const password = formGroup.get('Password')?.value;
    const confirmedPassword = formGroup.get('ConfirmedPassword')?.value;

    if (password !== confirmedPassword) {
      formGroup.get('ConfirmedPassword')?.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  };

  OnSubmit() {
    this.submitted = true;

    if (this.registrationForm.invalid) {
      return;
    }

    const userData: RegisterRequestDTO = {
      fullName: this.registrationForm.controls['FullName'].value,
      address: this.registrationForm.controls['Address'].value,
      email: this.registrationForm.controls['Email'].value,
      phone: this.registrationForm.controls['Phone'].value,
      age: this.registrationForm.controls['Age'].value,
      password: this.registrationForm.controls['Password'].value,
      confirmedPassword:
        this.registrationForm.controls['ConfirmedPassword'].value,
      role: this.registrationForm.controls['Role'].value,
    };
    this.emailSentTo = this.registrationForm.value.Email;

    console.log(this.registrationForm.value);
    this.authService.Register(userData).subscribe(
      (response) => {
        console.log('Registration successful', response);
        this.registrationForm.reset();
        this.submitted = false;
        this.showConfirmationMessage = true;
        this.toaster.success(
          'Registration successful! Please check your email.',
          'Success 🎉'
        );
        this.router.navigate(['login']);
      },
      (error) => {
        this.toaster.error(
          error.error.message || 'Registration failed. Try again.',
          'Error ❌'
        );
        console.error('Registration failed', error.error.message);
      }
    );
  }

  onReset() {
    this.submitted = false;
    this.registrationForm.reset();
  }
}
