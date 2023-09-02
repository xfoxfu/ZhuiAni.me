import api from "../api";
import { mutate } from "swr";

let ACCESS_TOKEN: string | null = sessionStorage.getItem("access_token");
let REFRESH_TOKEN: string | null = localStorage.getItem("refresh_token");

export const getAccessToken = () => ACCESS_TOKEN;
export const setAccessToken = (token: string | null) => {
  if (token !== null) sessionStorage.setItem("access_token", token);
  else sessionStorage.removeItem("access_token");
  ACCESS_TOKEN = token;
};
export const getRefreshToken = () => REFRESH_TOKEN;
export const setRefreshToken = (token: string | null) => {
  if (token !== null) localStorage.setItem("refresh_token", token);
  else localStorage.removeItem("refresh_token");
  REFRESH_TOKEN = token;
};

export const login = async (username: string, password: string, captcha: string) => {
  const result = await api.sessionLogin({ grant_type: "password", username, password, captcha });
  setAccessToken(result.data.access_token);
  setRefreshToken(result.data.refresh_token);
  await mutate(() => true, undefined, true);
};
export const refresh = async () => {
  const result = await api.sessionLogin({ grant_type: "refresh_token", refresh_token: getRefreshToken() ?? "" });
  setAccessToken(result.data.access_token);
  setRefreshToken(result.data.refresh_token);
  await mutate(() => true, undefined, true);
};
export const logout = async () => {
  await api.sessionLogout();
  setAccessToken(null);
  setRefreshToken(null);
  await mutate(() => true, undefined, true);
};
export const forceLogout = async () => {
  setAccessToken(null);
  setRefreshToken(null);
  await mutate(() => true, undefined, true);
};
