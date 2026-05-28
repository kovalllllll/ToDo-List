import { Component, inject, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { Project } from '../../../core/models/project.models';

@Component({
  selector: 'app-project-card',
  standalone: true,
  imports: [RouterLink, MatCardModule, MatButtonModule, MatIconModule, MatChipsModule],
  templateUrl: './project-card.component.html',
  styleUrl: './project-card.component.scss',
})
export class ProjectCardComponent {
  @Input({ required: true }) project!: Project;

  @Input() edit = (_project: Project) => {};
  @Input() remove = (_project: Project) => {};

  onEdit(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.edit(this.project);
  }

  onDelete(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    this.remove(this.project);
  }
}
