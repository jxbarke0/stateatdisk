import { Component, OnInit } from "@angular/core";
import { RegistrationService } from "../registration.service";
import { FormBuilder, Validators, AbstractControl } from "@angular/forms";

@Component({
  selector: "app-registration-form",
  templateUrl: "./registration-form.html",
  styles: []
})
export class RegistrationFormComponent implements OnInit {
  submitted = false;
  errorMsg = "";
  successData = "";
  user = "";

  constructor(
    private _registrationService: RegistrationService,
    private fb: FormBuilder
  ) {}
  // Registration Form from FormBuilder
  registrationForm = this.fb.group(
    {
      userName: ["", Validators.required],
      email: ["", [Validators.required, Validators.email]],
      password: ["", Validators.required],
      confirmPassword: ["", Validators.required]
    },
    { validator: this.PasswordValidator }
  );

  // Handles form validation errors
  validationErrors = false;

  // Checks to see whether Password and Confirm Password fields match
  PasswordValidator(
    control: AbstractControl
  ): { [key: string]: boolean } | null {
    const password = control.get("password");
    const confirmPassword = control.get("confirmPassword");
    if (password.pristine || confirmPassword.pristine) {
      return null;
    }
    return password &&
      confirmPassword &&
      password.value !== confirmPassword.value
      ? { misMatch: true }
      : null;
  }

  // Handle submit button on form
  onSubmit() {
    //console.log(this.registrationForm);
    this.submitted = true;
    this.user = this.registrationForm.value.userName;
    this._registrationService.register(this.registrationForm.value).subscribe(
      response => {
        if (response !== "success") {
          this.errorMsg = response;
        }
      },
      error => (this.errorMsg = error.statusText)
    );
  }

  // Resets form after it is submitted (Try Again button)
  resetForm() {
    this.submitted = false;
    this.errorMsg = "";
  }

  ngOnInit() {}
}
