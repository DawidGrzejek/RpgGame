import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './components/layout/navbar/navbar.component';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, NavbarComponent],
  template: `
    <div class="app">
      <app-navbar></app-navbar>

      <main class="main-content">
        <router-outlet></router-outlet>
      </main>

      <footer class="footer">
        <div class="footer-content">
          <p>&copy; 2025 RPG Game | A Clean Architecture Implementation</p>
          <div class="social-links">
            <a href="https://github.com/DawidGrzejek/RpgGame" aria-label="GitHub">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24">
                <path d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z" />
              </svg>
            </a>
          </div>
        </div>
      </footer>
    </div>
  `,
  styles: [`
    .app {
      min-height: 100vh;
      display: flex;
      flex-direction: column;
    }

    .main-content {
      flex: 1;
      background: #f8fafc;
    }

    .footer {
      background: #1f2937;
      color: white;
      padding: 20px 0;
    }

    .footer-content {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 20px;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .footer-content p {
      margin: 0;
      font-size: 14px;
    }

    .social-links a {
      color: white;
      opacity: 0.7;
      transition: opacity 0.2s;
    }

    .social-links a:hover {
      opacity: 1;
    }

    .social-links svg {
      fill: currentColor;
    }

    @media (max-width: 768px) {
      .footer-content {
        flex-direction: column;
        gap: 12px;
        text-align: center;
      }
    }
  `]
})
export class AppComponent implements OnInit {
  title = 'rpg-game-ui';

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    // Auth service will automatically initialize from localStorage
    // and handle token refresh
  }
}
