import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const storedUser = localStorage.getItem('techStoreUser');
  if (!storedUser) return next(request);

  try {
    const token = (JSON.parse(storedUser) as { token?: string }).token;
    if (!token || !request.url.startsWith('http://localhost:5050/api/')) return next(request);
    return next(request.clone({ setHeaders: { Authorization: `Bearer ${token}` } }));
  } catch {
    return next(request);
  }
};
