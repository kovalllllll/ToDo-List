import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { provideMockStore, MockStore } from '@ngrx/store/testing';
import { authInterceptor } from './auth.interceptor';
import { selectAuthToken } from '../../store/auth/auth.selectors';

describe('authInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let store: MockStore;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideMockStore({ selectors: [{ selector: selectAuthToken, value: 'test-token' }] }),
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
      ],
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
    store = TestBed.inject(MockStore);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should attach bearer token to protected requests', () => {
    http.get('/api/projects').subscribe();

    const req = httpMock.expectOne('/api/projects');
    expect(req.request.headers.get('Authorization')).toBe('Bearer test-token');
    req.flush([]);
  });

  it('should not attach bearer token to sign-in requests', () => {
    store.overrideSelector(selectAuthToken, 'test-token');
    store.refreshState();

    http.post('/api/users/sign-in', {}).subscribe();

    const req = httpMock.expectOne('/api/users/sign-in');
    expect(req.request.headers.has('Authorization')).toBe(false);
    req.flush({});
  });
});
