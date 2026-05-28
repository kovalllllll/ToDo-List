import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { provideMockStore, MockStore } from '@ngrx/store/testing';
import { firstValueFrom, isObservable } from 'rxjs';
import { authGuard } from './auth.guard';
import { selectIsAuthenticated } from '../../store/auth/auth.selectors';

describe('authGuard', () => {
  let store: MockStore;
  let router: Router;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideMockStore(),
        {
          provide: Router,
          useValue: {
            createUrlTree: jest.fn(() => '/auth/login'),
          },
        },
      ],
    });

    store = TestBed.inject(MockStore);
    router = TestBed.inject(Router);
  });

  it('should allow navigation when authenticated', async () => {
    store.overrideSelector(selectIsAuthenticated, true);

    await TestBed.runInInjectionContext(async () => {
      const result = authGuard({} as never, {} as never);

      if (isObservable(result)) {
        const value = await firstValueFrom(result);
        expect(value).toBe(true);
        return;
      }

      expect(result).toBe(true);
    });
  });

  it('should redirect when not authenticated', async () => {
    store.overrideSelector(selectIsAuthenticated, false);

    await TestBed.runInInjectionContext(async () => {
      const result = authGuard({} as never, {} as never);

      if (isObservable(result)) {
        await firstValueFrom(result);
      }

      expect(router.createUrlTree).toHaveBeenCalledWith(['/auth/login']);
    });
  });
});
