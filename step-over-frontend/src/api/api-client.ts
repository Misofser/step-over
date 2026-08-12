import { API_URL } from '../config'

export async function baseFetch(input: string, options: RequestInit = {}) {
  return fetch(`${API_URL}${input}`, {
    ...options,
    credentials: "include",
  });
}

async function retryRequest(fn: () => Promise<Response>) {
  const res = await fn();
  if (res.status !== 401) return res;

  const refreshRes = await baseFetch("/auth/refresh", {
    method: "POST",
  });

  if (!refreshRes.ok) throw new Error("Not authenticated");

  return fn();
}

export function authenticatedFetch(input: string, options: RequestInit = {}) {
  return retryRequest(() => baseFetch(input, options));
}
