import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../Services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { IConfirmEmail } from '../../Models/ConfirmRequestDTO.model';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css'],
})
export class ConfirmEmailComponent implements OnInit {
  confirmationStatus: 'loading' | 'success' | 'error' = 'loading';
  errorMessage: string =
    "We couldn't verify your email address. The verification link may have expired or is invalid.";
  resendLoading: boolean = false;
  resendSuccess: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      const token = params['token'];
      const id = params['userId'];
      if (token) {
        this.verifyEmailWithToken(token, id);
      } else {
        this.confirmationStatus = 'error';
        this.errorMessage = 'No verification token found in the URL.';
      }
    });
  }

  verifyEmailWithToken(token: string, id: string): void {
    this.confirmationStatus = 'loading';
    const confirmData: IConfirmEmail = {
      UserId: id,
      Token: token,
    };
    this.authService.ConfirmationEmail(confirmData).subscribe({
      next: (response) => {
        this.confirmationStatus = 'success';
      },
      error: (error) => {
        this.confirmationStatus = 'error';
        if (error.status === 410) {
          this.errorMessage = 'This verification link has expired.';
        } else if (error.status === 404) {
          this.errorMessage =
            'This verification link is invalid or has already been used.';
        } else {
          this.errorMessage =
            'An error occurred while verifying your email. Please try again later.';
        }
      },
    });
  }

  navigateToLogin(): void {
    this.router.navigate(['/login']);
  }
}
