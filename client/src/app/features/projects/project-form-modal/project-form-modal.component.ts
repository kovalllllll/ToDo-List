import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Project } from '../../../core/models/project.models';

export interface ProjectFormDialogData {
  project?: Project;
}

const HEX_COLOR_PATTERN = /^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$/;

export const PROJECT_NAME_MAX_LENGTH = 100;
export const PROJECT_DESCRIPTION_MAX_LENGTH = 500;

@Component({
  selector: 'app-project-form-modal',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
  templateUrl: './project-form-modal.component.html',
  styleUrl: './project-form-modal.component.scss',
})
export class ProjectFormModalComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<ProjectFormModalComponent>);
  readonly data = inject<ProjectFormDialogData>(MAT_DIALOG_DATA);

  readonly isEdit = !!this.data.project;
  readonly nameMaxLength = PROJECT_NAME_MAX_LENGTH;
  readonly descriptionMaxLength = PROJECT_DESCRIPTION_MAX_LENGTH;

  readonly form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(PROJECT_NAME_MAX_LENGTH)]],
    description: ['', [Validators.maxLength(PROJECT_DESCRIPTION_MAX_LENGTH)]],
    color: ['#607d8b', [Validators.pattern(HEX_COLOR_PATTERN)]],
  });

  ngOnInit(): void {
    if (this.data.project) {
      this.form.patchValue({
        name: this.data.project.name,
        description: this.data.project.description ?? '',
        color: this.data.project.color ?? '#607d8b',
      });
    }
  }

  cancel(): void {
    this.dialogRef.close();
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.dialogRef.close({
      name: value.name.trim(),
      description: value.description.trim() || null,
      color: value.color || null,
    });
  }
}
