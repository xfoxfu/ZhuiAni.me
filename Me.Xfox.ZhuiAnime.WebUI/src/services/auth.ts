import api from "../api";
import { getDefaultStore } from "jotai";
import { atomWithStorage, createJSONStorage } from "jotai/utils";
import { mutate } from "swr";

const sessionStore = getDefaultStore();
const defaultStorage = createJSONStorage<string | null>(() => localStorage);
export const accessTokenAtom = atomWithStorage<string | null>("access_token", null, defaultStorage, {
  unstable_getOnInit: true,
});
export const refreshTokenAtom = atomWithStorage<string | null>("refresh_token", null, defaultStorage, {
  unstable_getOnInit: true,
});

export const getAccessToken = () => sessionStore.get(accessTokenAtom);

export const login = async (username: string, password: string, captcha: string) => {
  const result = await api.sessionLogin({ grant_type: "password", username, password, captcha });
  sessionStore.set(accessTokenAtom, result.data.access_token);
  sessionStore.set(refreshTokenAtom, result.data.refresh_token);
  await mutate(() => true, undefined, true);
};
export const refresh = async () => {
  const result = await api.sessionLogin({
    grant_type: "refresh_token",
    refresh_token: sessionStore.get(refreshTokenAtom) ?? "",
  });
  sessionStore.set(accessTokenAtom, result.data.access_token);
  sessionStore.set(refreshTokenAtom, result.data.refresh_token);
  await mutate(() => true, undefined, true);
};
export const logout = async () => {
  await api.sessionLogout();
  sessionStore.set(accessTokenAtom, null);
  sessionStore.set(refreshTokenAtom, null);
  await mutate(() => true, undefined, true);
};
export const forceLogout = async () => {
  sessionStore.set(accessTokenAtom, null);
  sessionStore.set(refreshTokenAtom, null);
  await mutate(() => true, undefined, true);
};
