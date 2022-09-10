/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export interface Anime {
  /** @format int32 */
  id: number;
  title: string;

  /** @format uri */
  bangumi_link: string;
  image_base64?: string | null;
}

export interface AnimeDetailed {
  /** @format int32 */
  id: number;
  title: string;

  /** @format uri */
  bangumi_link: string;
  image?: string | null;
}

export interface Episode {
  /** @format int32 */
  id: number;
  name: string;
  title: string;
}

export interface ErrorProdResponse {
  error_code: string;
  message: string;
}

export interface ImportRequest {
  /** @format int32 */
  id: number;
}

import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse, ResponseType } from "axios";

export type QueryParamsType = Record<string | number, any>;

export interface FullRequestParams extends Omit<AxiosRequestConfig, "data" | "params" | "url" | "responseType"> {
  /** set parameter to `true` for call `securityWorker` for this request */
  secure?: boolean;
  /** request path */
  path: string;
  /** content type of request body */
  type?: ContentType;
  /** query params */
  query?: QueryParamsType;
  /** format of response (i.e. response.json() -> format: "json") */
  format?: ResponseType;
  /** request body */
  body?: unknown;
}

export type RequestParams = Omit<FullRequestParams, "body" | "method" | "query" | "path">;

export interface ApiConfig<SecurityDataType = unknown> extends Omit<AxiosRequestConfig, "data" | "cancelToken"> {
  securityWorker?: (
    securityData: SecurityDataType | null
  ) => Promise<AxiosRequestConfig | void> | AxiosRequestConfig | void;
  secure?: boolean;
  format?: ResponseType;
}

export enum ContentType {
  Json = "application/json",
  FormData = "multipart/form-data",
  UrlEncoded = "application/x-www-form-urlencoded",
}

export class HttpClient<SecurityDataType = unknown> {
  public instance: AxiosInstance;
  private securityData: SecurityDataType | null = null;
  private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
  private secure?: boolean;
  private format?: ResponseType;

  constructor({ securityWorker, secure, format, ...axiosConfig }: ApiConfig<SecurityDataType> = {}) {
    this.instance = axios.create({ ...axiosConfig, baseURL: axiosConfig.baseURL || "" });
    this.secure = secure;
    this.format = format;
    this.securityWorker = securityWorker;
  }

  public setSecurityData = (data: SecurityDataType | null) => {
    this.securityData = data;
  };

  private mergeRequestParams(params1: AxiosRequestConfig, params2?: AxiosRequestConfig): AxiosRequestConfig {
    return {
      ...this.instance.defaults,
      ...params1,
      ...(params2 || {}),
      headers: Object.assign({}, this.instance.defaults.headers, (params1 || {}).headers, (params2 || {}).headers),
    };
  }

  private createFormData(input: Record<string, unknown>): FormData {
    return Object.keys(input || {}).reduce((formData, key) => {
      const property = input[key];
      if (Array.isArray(property)) {
        property.forEach((blob) => {
          formData.append(
            key,
            blob instanceof Blob ? blob : typeof blob === "object" && blob !== null ? JSON.stringify(blob) : `${blob}`
          );
        });
      } else {
        formData.append(
          key,
          property instanceof Blob
            ? property
            : typeof property === "object" && property !== null
            ? JSON.stringify(property)
            : `${property}`
        );
      }
      return formData;
    }, new FormData());
  }

  public request = async <T = any, _E = any>({
    secure,
    path,
    type,
    query,
    format,
    body,
    ...params
  }: FullRequestParams): Promise<AxiosResponse<T>> => {
    const secureParams =
      ((typeof secure === "boolean" ? secure : this.secure) &&
        this.securityWorker &&
        (await this.securityWorker(this.securityData))) ||
      {};
    const requestParams = this.mergeRequestParams(params, secureParams);
    const responseFormat = (format && this.format) || void 0;

    if (type === ContentType.FormData && body && body !== null && typeof body === "object") {
      if (!requestParams.headers) requestParams.headers = { Accept: "*/*" };

      body = this.createFormData(body as Record<string, unknown>);
    }

    return this.instance.request({
      ...requestParams,
      headers: {
        ...(type && type !== ContentType.FormData ? { "Content-Type": type } : {}),
        ...(requestParams.headers || {}),
      },
      params: query,
      responseType: responseFormat,
      data: body,
      url: path,
    });
  };
}

import useSWR, { mutate, MutatorOptions, SWRConfiguration } from "swr";

/**
 * @title ZhuiAni.me API
 * @version v1
 */
export class Api<SecurityDataType extends unknown> extends HttpClient<SecurityDataType> {
  anime = {
    /**
     * No description
     *
     * @tags Anime
     * @name AnimesList
     * @request GET:/api/animes
     */
    animesList: (query?: { include_image?: boolean }, params: RequestParams = {}) =>
      this.request<Anime[], ErrorProdResponse>({
        path: `/api/animes`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),
    /**
     * No description
     *
     * @tags Anime
     * @name AnimesList
     * @request GET:/api/animes
     */
    useAnimesList: (query?: { include_image?: boolean }, options?: SWRConfiguration, doFetch: boolean = true) =>
      useSWR<Anime[], ErrorProdResponse>(doFetch ? [`/api/animes`, query] : null, options),

    /**
     * No description
     *
     * @tags Anime
     * @name AnimesList
     * @request GET:/api/animes
     */
    mutateAnimesList: (
      query?: { include_image?: boolean },
      data?: Anime[] | Promise<Anime[]>,
      options?: MutatorOptions
    ) => mutate<Anime[]>([`/api/animes`, query], data, options),

    /**
     * No description
     *
     * @tags Anime
     * @name AnimesDetail
     * @request GET:/api/animes/{id}
     */
    animesDetail: (id: number, params: RequestParams = {}) =>
      this.request<AnimeDetailed, ErrorProdResponse>({
        path: `/api/animes/${id}`,
        method: "GET",
        format: "json",
        ...params,
      }),
    /**
     * No description
     *
     * @tags Anime
     * @name AnimesDetail
     * @request GET:/api/animes/{id}
     */
    useAnimesDetail: (id: number, options?: SWRConfiguration, doFetch: boolean = true) =>
      useSWR<AnimeDetailed, ErrorProdResponse>(doFetch ? `/api/animes/${id}` : null, options),

    /**
     * No description
     *
     * @tags Anime
     * @name AnimesDetail
     * @request GET:/api/animes/{id}
     */
    mutateAnimesDetail: (id: number, data?: AnimeDetailed | Promise<AnimeDetailed>, options?: MutatorOptions) =>
      mutate<AnimeDetailed>(`/api/animes/${id}`, data, options),

    /**
     * No description
     *
     * @tags Anime
     * @name AnimesEpisodesDetail
     * @request GET:/api/animes/{id}/episodes
     */
    animesEpisodesDetail: (id: number, params: RequestParams = {}) =>
      this.request<Episode[], ErrorProdResponse>({
        path: `/api/animes/${id}/episodes`,
        method: "GET",
        format: "json",
        ...params,
      }),
    /**
     * No description
     *
     * @tags Anime
     * @name AnimesEpisodesDetail
     * @request GET:/api/animes/{id}/episodes
     */
    useAnimesEpisodesDetail: (id: number, options?: SWRConfiguration, doFetch: boolean = true) =>
      useSWR<Episode[], ErrorProdResponse>(doFetch ? `/api/animes/${id}/episodes` : null, options),

    /**
     * No description
     *
     * @tags Anime
     * @name AnimesEpisodesDetail
     * @request GET:/api/animes/{id}/episodes
     */
    mutateAnimesEpisodesDetail: (id: number, data?: Episode[] | Promise<Episode[]>, options?: MutatorOptions) =>
      mutate<Episode[]>(`/api/animes/${id}/episodes`, data, options),

    /**
     * No description
     *
     * @tags Anime
     * @name AnimesImportBangumiCreate
     * @request POST:/api/animes/import_bangumi
     */
    animesImportBangumiCreate: (data: ImportRequest, params: RequestParams = {}) =>
      this.request<void, ErrorProdResponse>({
        path: `/api/animes/import_bangumi`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),
  };
}

const api = new Api();
export default api;

export const fetcher = async (path: string, query?: Record<string, unknown>) => {
  return await api
    .request({ path, query })
    .then((res) => res.data)
    .catch((err) => {
      throw err.response.data;
    });
};
