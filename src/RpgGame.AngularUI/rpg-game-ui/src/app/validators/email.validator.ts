import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export class EmailValidators {
  /**
   * Enhanced email validator that requires proper domain format (string@string.domain)
   */
  static enhancedEmail(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null; // Don't validate empty values, let required validator handle it
      }

      const email = control.value.toString().trim();
      
      // Enhanced email regex that requires proper domain with TLD
      const enhancedEmailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
      
      if (!enhancedEmailRegex.test(email)) {
        return { 
          enhancedEmail: { 
            message: 'Email must include a valid domain (e.g., user@example.com)',
            actualValue: email 
          } 
        };
      }

      // Additional checks
      if (email.includes('..')) {
        return { 
          enhancedEmail: { 
            message: 'Email cannot contain consecutive dots',
            actualValue: email 
          } 
        };
      }

      if (email.startsWith('.') || email.endsWith('.')) {
        return { 
          enhancedEmail: { 
            message: 'Email cannot start or end with a dot',
            actualValue: email 
          } 
        };
      }

      return null;
    };
  }

  /**
   * Strict email validator with additional business rules
   */
  static strictEmail(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) {
        return null;
      }

      const email = control.value.toString().trim().toLowerCase();
      
      // Very strict email pattern
      const strictEmailRegex = /^[a-zA-Z0-9]([a-zA-Z0-9._-]*[a-zA-Z0-9])?@[a-zA-Z0-9]([a-zA-Z0-9.-]*[a-zA-Z0-9])?\.[a-zA-Z]{2,}$/;
      
      if (!strictEmailRegex.test(email)) {
        return { 
          strictEmail: { 
            message: 'Please enter a valid email address with proper domain',
            actualValue: email 
          } 
        };
      }

      // Block common invalid domains for demonstration
      const invalidDomains = ['test.test', 'example.example', 'invalid.invalid'];
      const domain = email.split('@')[1];
      
      if (invalidDomains.includes(domain)) {
        return { 
          strictEmail: { 
            message: 'Please use a real email domain',
            actualValue: email 
          } 
        };
      }

      return null;
    };
  }
}