import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../Services/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css',
})
export class NavbarComponent implements OnInit {
  isLogined: boolean = false;
  private loginSubscription: Subscription | undefined;

  constructor(private auth: AuthService) {}

  ngOnInit(): void {
    this.loginSubscription = this.auth.isLoggedIn$.subscribe((isLoggedIn) => {
      console.log('isLogined:', isLoggedIn);
      this.isLogined = isLoggedIn;
    });
  }
  loguot() {
    this.auth.logout();
  }
  ngOnDestroy(): void {
    if (this.loginSubscription) {
      this.loginSubscription.unsubscribe();
    }
  }
}
