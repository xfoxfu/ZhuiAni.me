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

export interface AnimeDto {
  /** @format int32 */
  id: number;
  /** @format int32 */
  bangumi: number;
  target: string;
  regex: string;
  /** @format int32 */
  match_group_ep: number;
  enabled: boolean;
}

/** Category information. */
export interface CategoryDto {
  /**
   * id
   * @format int32
   */
  id: number;
  /** user-friendly name */
  title: string;
}

export interface CreateAnimeDto {
  /** @format int32 */
  bangumi: number;
  target: string;
  regex: string;
  /** @format int32 */
  match_group_ep: number;
}

export interface CreateItemDto {
  /** @format int32 */
  category_id: number;
  title: string;
  annotations: Record<string, string>;
  /** @format int32 */
  parent_item_id?: number | null;
}

/** Information for creating a link. */
export interface CreateLinkDto {
  /**
   * the url this link points to
   * @format uri
   */
  address: string;
  /** the MIME type of the target of this link */
  mime_type: string;
  /** extra information for this link */
  annotations: Record<string, string>;
  /**
   * id of parent link, if exists
   * @format int32
   */
  parent_link_id?: number | null;
}

export interface CreateOrUpdateCategoryDto {
  /** user-friendly name */
  title: string;
}

export interface ErrorProdResponse {
  error_code: string;
  message: string;
  [key: string]: any;
}

export interface ImportSubjectDto {
  /** @format int32 */
  id: number;
}

/** An item, like an anime, a manga, a episode in an anime, etc. */
export interface ItemDto {
  /**
   * id
   * @format int32
   */
  id: number;
  /**
   * the id of category this item belongs to
   * @format int32
   */
  category_id: number;
  /** original title of the item */
  title: string;
  /** additional information */
  annotations: Record<string, string>;
  /**
   * the id of the parent item, if this item belongs to a parent item
   * @format int32
   */
  parent_item_id?: number | null;
}

/** Link. */
export interface LinkDto {
  /**
   * id
   * @format int32
   */
  id: number;
  /**
   * id of the item this link belongs to
   * @format int32
   */
  item_id: number;
  /**
   * the url this link points to
   * @format uri
   */
  address: string;
  /** the MIME type of the target of this link */
  mime_type: string;
  /** extra information for this link */
  annotations: Record<string, string>;
  /**
   * id of parent link, if exists
   * @format int32
   */
  parent_link_id?: number | null;
}

export interface TorrentDto {
  /** @format int32 */
  id: number;
  origin_site: string;
  origin_id: string;
  title: string;
  /** @format date-time */
  published_at: string;
  link_torrent?: string | null;
  link_magnet?: string | null;
}

export interface UpdateAnimeDto {
  /** @format int32 */
  bangumi: number;
  target: string;
  regex: string;
  /** @format int32 */
  match_group_ep: number;
  enabled: boolean;
}

export interface UpdateItemDto {
  /** @format int32 */
  category_id?: number | null;
  title?: string | null;
  annotations?: Record<string, string>;
}

/** Information for updating a link. */
export interface UpdateLinkDto {
  /**
   * the url this link points to
   * @format uri
   */
  address: string;
  /** the MIME type of the target of this link */
  mime_type: string;
  /** extra information for this link */
  annotations: Record<string, string>;
}

export type QueryParamsType = Record<string | number, any>;
export type ResponseFormat = keyof Omit<Body, "body" | "bodyUsed">;

export interface FullRequestParams extends Omit<RequestInit, "body"> {
  /** set parameter to `true` for call `securityWorker` for this request */
  secure?: boolean;
  /** request path */
  path: string;
  /** content type of request body */
  type?: ContentType;
  /** query params */
  query?: QueryParamsType;
  /** format of response (i.e. response.json() -> format: "json") */
  format?: ResponseFormat;
  /** request body */
  body?: unknown;
  /** base url */
  baseUrl?: string;
  /** request cancellation token */
  cancelToken?: CancelToken;
}

export type RequestParams = Omit<FullRequestParams, "body" | "method" | "query" | "path">;

export interface ApiConfig<SecurityDataType = unknown> {
  baseUrl?: string;
  baseApiParams?: Omit<RequestParams, "baseUrl" | "cancelToken" | "signal">;
  securityWorker?: (securityData: SecurityDataType | null) => Promise<RequestParams | void> | RequestParams | void;
  customFetch?: typeof fetch;
}

export interface HttpResponse<D extends unknown, E extends unknown = unknown> extends Response {
  data: D;
  error: E;
}

type CancelToken = Symbol | string | number;

export enum ContentType {
  Json = "application/json",
  FormData = "multipart/form-data",
  UrlEncoded = "application/x-www-form-urlencoded",
  Text = "text/plain",
}

export class HttpClient<SecurityDataType = unknown> {
  public baseUrl: string = "";
  private securityData: SecurityDataType | null = null;
  private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
  private abortControllers = new Map<CancelToken, AbortController>();
  private customFetch = (...fetchParams: Parameters<typeof fetch>) => fetch(...fetchParams);

  private baseApiParams: RequestParams = {
    credentials: "same-origin",
    headers: {},
    redirect: "follow",
    referrerPolicy: "no-referrer",
  };

  constructor(apiConfig: ApiConfig<SecurityDataType> = {}) {
    Object.assign(this, apiConfig);
  }

  public setSecurityData = (data: SecurityDataType | null) => {
    this.securityData = data;
  };

  protected encodeQueryParam(key: string, value: any) {
    const encodedKey = encodeURIComponent(key);
    return `${encodedKey}=${encodeURIComponent(typeof value === "number" ? value : `${value}`)}`;
  }

  protected addQueryParam(query: QueryParamsType, key: string) {
    return this.encodeQueryParam(key, query[key]);
  }

  protected addArrayQueryParam(query: QueryParamsType, key: string) {
    const value = query[key];
    return value.map((v: any) => this.encodeQueryParam(key, v)).join("&");
  }

  protected toQueryString(rawQuery?: QueryParamsType): string {
    const query = rawQuery || {};
    const keys = Object.keys(query).filter((key) => "undefined" !== typeof query[key]);
    return keys
      .map((key) => (Array.isArray(query[key]) ? this.addArrayQueryParam(query, key) : this.addQueryParam(query, key)))
      .join("&");
  }

  protected addQueryParams(rawQuery?: QueryParamsType): string {
    const queryString = this.toQueryString(rawQuery);
    return queryString ? `?${queryString}` : "";
  }

  private contentFormatters: Record<ContentType, (input: any) => any> = {
    [ContentType.Json]: (input: any) =>
      input !== null && (typeof input === "object" || typeof input === "string") ? JSON.stringify(input) : input,
    [ContentType.Text]: (input: any) => (input !== null && typeof input !== "string" ? JSON.stringify(input) : input),
    [ContentType.FormData]: (input: any) =>
      Object.keys(input || {}).reduce((formData, key) => {
        const property = input[key];
        formData.append(
          key,
          property instanceof Blob
            ? property
            : typeof property === "object" && property !== null
            ? JSON.stringify(property)
            : `${property}`,
        );
        return formData;
      }, new FormData()),
    [ContentType.UrlEncoded]: (input: any) => this.toQueryString(input),
  };

  protected mergeRequestParams(params1: RequestParams, params2?: RequestParams): RequestParams {
    return {
      ...this.baseApiParams,
      ...params1,
      ...(params2 || {}),
      headers: {
        ...(this.baseApiParams.headers || {}),
        ...(params1.headers || {}),
        ...((params2 && params2.headers) || {}),
      },
    };
  }

  protected createAbortSignal = (cancelToken: CancelToken): AbortSignal | undefined => {
    if (this.abortControllers.has(cancelToken)) {
      const abortController = this.abortControllers.get(cancelToken);
      if (abortController) {
        return abortController.signal;
      }
      return void 0;
    }

    const abortController = new AbortController();
    this.abortControllers.set(cancelToken, abortController);
    return abortController.signal;
  };

  public abortRequest = (cancelToken: CancelToken) => {
    const abortController = this.abortControllers.get(cancelToken);

    if (abortController) {
      abortController.abort();
      this.abortControllers.delete(cancelToken);
    }
  };

  public request = async <T = any, E = any>({
    body,
    secure,
    path,
    type,
    query,
    format,
    baseUrl,
    cancelToken,
    ...params
  }: FullRequestParams): Promise<HttpResponse<T, E>> => {
    const secureParams =
      ((typeof secure === "boolean" ? secure : this.baseApiParams.secure) &&
        this.securityWorker &&
        (await this.securityWorker(this.securityData))) ||
      {};
    const requestParams = this.mergeRequestParams(params, secureParams);
    const queryString = query && this.toQueryString(query);
    const payloadFormatter = this.contentFormatters[type || ContentType.Json];
    const responseFormat = format || requestParams.format;

    return this.customFetch(`${baseUrl || this.baseUrl || ""}${path}${queryString ? `?${queryString}` : ""}`, {
      ...requestParams,
      headers: {
        ...(requestParams.headers || {}),
        ...(type && type !== ContentType.FormData ? { "Content-Type": type } : {}),
      },
      signal: cancelToken ? this.createAbortSignal(cancelToken) : requestParams.signal,
      body: typeof body === "undefined" || body === null ? null : payloadFormatter(body),
    }).then(async (response) => {
      const r = response as HttpResponse<T, E>;
      r.data = null as unknown as T;
      r.error = null as unknown as E;

      const data = !responseFormat
        ? r
        : await response[responseFormat]()
            .then((data) => {
              if (r.ok) {
                r.data = data;
              } else {
                r.error = data;
              }
              return r;
            })
            .catch((e) => {
              r.error = e;
              return r;
            });

      if (cancelToken) {
        this.abortControllers.delete(cancelToken);
      }

      if (!response.ok) throw data;
      return data;
    });
  };
}

export class ApiError extends Error {}

import useSWR, { mutate, MutatorOptions, SWRConfiguration } from "swr";

/**
 * @title ZhuiAni.me API
 * @version v1
 */
export class Api<SecurityDataType extends unknown> extends HttpClient<SecurityDataType> {
  bangumi = {
    /**
     * No description
     *
     * @tags Bangumi
     * @name PostModulesBangumiImportSubject
     * @request POST:/api/modules/bangumi/import_subject
     */
    postModulesBangumiImportSubject: (data: ImportSubjectDto, params: RequestParams = {}) =>
      this.request<ItemDto, ErrorProdResponse>({
        path: `/api/modules/bangumi/import_subject`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),
  };
  category = {
    /**
     * No description
     *
     * @tags Category
     * @name GetCategories
     * @summary Get all categories.
     * @request GET:/api/categories
     */
    getCategories: (
      query?: {
        someBool?: boolean;
      },
      params: RequestParams = {},
    ) =>
      this.request<CategoryDto[], ErrorProdResponse>({
        path: `/api/categories`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),
    /**
     * No description
     *
     * @tags Category
     * @name GetCategories
     * @summary Get all categories.
     * @request GET:/api/categories
     */
    useGetCategories: (
      query?: {
        someBool?: boolean;
      },
      options?: SWRConfiguration,
      doFetch: boolean = true,
    ) => useSWR<CategoryDto[], ErrorProdResponse>(doFetch ? [`/api/categories`, query] : null, options),

    /**
     * No description
     *
     * @tags Category
     * @name GetCategories
     * @summary Get all categories.
     * @request GET:/api/categories
     */
    mutateGetCategories: (
      query?: {
        someBool?: boolean;
      },
      data?: CategoryDto[] | Promise<CategoryDto[]>,
      options?: MutatorOptions,
    ) => mutate<CategoryDto[]>([`/api/categories`, query], data, options),

    /**
     * No description
     *
     * @tags Category
     * @name PostCategories
     * @summary Create a new category.
     * @request POST:/api/categories
     */
    postCategories: (data: CreateOrUpdateCategoryDto, params: RequestParams = {}) =>
      this.request<CategoryDto, ErrorProdResponse>({
        path: `/api/categories`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Category
     * @name GetCategory
     * @summary Get a category.
     * @request GET:/api/categories/{id}
     */
    getCategory: (id: number, params: RequestParams = {}) =>
      this.request<CategoryDto, ErrorProdResponse>({
        path: `/api/categories/${id}`,
        method: "GET",
        format: "json",
        ...params,
      }),
    /**
     * No description
     *
     * @tags Category
     * @name GetCategory
     * @summary Get a category.
     * @request GET:/api/categories/{id}
     */
    useGetCategory: (id: number, options?: SWRConfiguration, doFetch: boolean = true) =>
      useSWR<CategoryDto, ErrorProdResponse>(doFetch ? `/api/categories/${id}` : null, options),

    /**
     * No description
     *
     * @tags Category
     * @name GetCategory
     * @summary Get a category.
     * @request GET:/api/categories/{id}
     */
    mutateGetCategory: (id: number, data?: CategoryDto | Promise<CategoryDto>, options?: MutatorOptions) =>
      mutate<CategoryDto>(`/api/categories/${id}`, data, options),

    /**
     * No description
     *
     * @tags Category
     * @name PatchCategory
     * @summary Update a category.
     * @request PATCH:/api/categories/{id}
     */
    patchCategory: (id: number, data: CreateOrUpdateCategoryDto, params: RequestParams = {}) =>
      this.request<CategoryDto, ErrorProdResponse>({
        path: `/api/categories/${id}`,
        method: "PATCH",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Category
     * @name DeleteCategory
     * @summary Delete a category.
     * @request DELETE:/api/categories/{id}
     */
    deleteCategory: (id: number, params: RequestParams = {}) =>
      this.request<CategoryDto, ErrorProdResponse>({
        path: `/api/categories/${id}`,
        method: "DELETE",
        format: "json",
        ...params,
      }),

    /**
     * @description This API will only return those are top-level, i.e. do not have a parent item. The result will be ordered by id descendingly.
     *
     * @tags Category
     * @name GetCategoryItems
     * @summary Get a category's items.
     * @request GET:/api/categories/{id}/items
     */
    getCategoryItems: (id: number, params: RequestParams = {}) =>
      this.request<ItemDto[], ErrorProdResponse>({
        path: `/api/categories/${id}/items`,
        method: "GET",
        format: "json",
        ...params,
      }),
    /**
     * @description This API will only return those are top-level, i.e. do not have a parent item. The result will be ordered by id descendingly.
     *
     * @tags Category
     * @name GetCategoryItems
     * @summary Get a category's items.
     * @request GET:/api/categories/{id}/items
     */
    useGetCategoryItems: (id: number, options?: SWRConfiguration, doFetch: boolean = true) =>
      useSWR<ItemDto[], ErrorProdResponse>(doFetch ? `/api/categories/${id}/items` : null, options),

    /**
     * @description This API will only return those are top-level, i.e. do not have a parent item. The result will be ordered by id descendingly.
     *
     * @tags Category
     * @name GetCategoryItems
     * @summary Get a category's items.
     * @request GET:/api/categories/{id}/items
     */
    mutateGetCategoryItems: (id: number, data?: ItemDto[] | Promise<ItemDto[]>, options?: MutatorOptions) =>
      mutate<ItemDto[]>(`/api/categories/${id}/items`, data, options),
  };
  item = {
    /**
     * @description This API will only return those are top-level, i.e. do not have a parent item. The result will be ordered by id descendingly.
     *
     * @tags Item
     * @name GetItems
     * @summary Get all items.
     * @request GET:/api/items
     */
    getItems: (params: RequestParams = {}) =>
      this.request<ItemDto[], ErrorProdResponse>({
        path: `/api/items`,
        method: "GET",
        format: "json",
        ...params,
      }),
    /**
     * @description This API will only return those are top-level, i.e. do not have a parent item. The result will be ordered by id descendingly.
     *
     * @tags Item
     * @name GetItems
     * @summary Get all items.
     * @request GET:/api/items
     */
    useGetItems: (options?: SWRConfiguration, doFetch: boolean = true) =>
      useSWR<ItemDto[], ErrorProdResponse>(doFetch ? `/api/items` : null, options),

    /**
     * @description This API will only return those are top-level, i.e. do not have a parent item. The result will be ordered by id descendingly.
     *
     * @tags Item
     * @name GetItems
     * @summary Get all items.
     * @request GET:/api/items
     */
    mutateGetItems: (data?: ItemDto[] | Promise<ItemDto[]>, options?: MutatorOptions) =>
      mutate<ItemDto[]>(`/api/items`, data, options),

    /**
     * No description
     *
     * @tags Item
     * @name PostItems
     * @summary Create a new item.
     * @request POST:/api/items
     */
    postItems: (data: CreateItemDto, params: RequestParams = {}) =>
      this.request<ItemDto, ErrorProdResponse>({
        path: `/api/items`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Item
     * @name GetItem
     * @summary Get a item.
     * @request GET:/api/items/{id}
     */
    getItem: (id: number, params: RequestParams = {}) =>
      this.request<ItemDto, ErrorProdResponse>({
        path: `/api/items/${id}`,
        method: "GET",
        format: "json",
        ...params,
      }),
    /**
     * No description
     *
     * @tags Item
     * @name GetItem
     * @summary Get a item.
     * @request GET:/api/items/{id}
     */
    useGetItem: (id: number, options?: SWRConfiguration, doFetch: boolean = true) =>
      useSWR<ItemDto, ErrorProdResponse>(doFetch ? `/api/items/${id}` : null, options),

    /**
     * No description
     *
     * @tags Item
     * @name GetItem
     * @summary Get a item.
     * @request GET:/api/items/{id}
     */
    mutateGetItem: (id: number, data?: ItemDto | Promise<ItemDto>, options?: MutatorOptions) =>
      mutate<ItemDto>(`/api/items/${id}`, data, options),

    /**
     * No description
     *
     * @tags Item
     * @name PatchItem
     * @summary Update a item.
     * @request PATCH:/api/items/{id}
     */
    patchItem: (id: number, data: UpdateItemDto, params: RequestParams = {}) =>
      this.request<ItemDto, ErrorProdResponse>({
        path: `/api/items/${id}`,
        method: "PATCH",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Item
     * @name DeleteItem
     * @summary Delete a item.
     * @request DELETE:/api/items/{id}
     */
    deleteItem: (id: number, params: RequestParams = {}) =>
      this.request<ItemDto, ErrorProdResponse>({
        path: `/api/items/${id}`,
        method: "DELETE",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Item
     * @name GetItemItems
     * @summary Get a item's child items.
     * @request GET:/api/items/{id}/items
     */
    getItemItems: (id: number, params: RequestParams = {}) =>
      this.request<ItemDto[], ErrorProdResponse>({
        path: `/api/items/${id}/items`,
        method: "GET",
        format: "json",
        ...params,
      }),
    /**
     * No description
     *
     * @tags Item
     * @name GetItemItems
     * @summary Get a item's child items.
     * @request GET:/api/items/{id}/items
     */
    useGetItemItems: (id: number, options?: SWRConfiguration, doFetch: boolean = true) =>
      useSWR<ItemDto[], ErrorProdResponse>(doFetch ? `/api/items/${id}/items` : null, options),

    /**
     * No description
     *
     * @tags Item
     * @name GetItemItems
     * @summary Get a item's child items.
     * @request GET:/api/items/{id}/items
     */
    mutateGetItemItems: (id: number, data?: ItemDto[] | Promise<ItemDto[]>, options?: MutatorOptions) =>
      mutate<ItemDto[]>(`/api/items/${id}/items`, data, options),
  };
  itemLink = {
    /**
     * No description
     *
     * @tags ItemLink
     * @name GetItemLinks
     * @summary Get all links.
     * @request GET:/api/items/{item_id}/links
     */
    getItemLinks: (itemId: number, params: RequestParams = {}) =>
      this.request<LinkDto[], ErrorProdResponse>({
        path: `/api/items/${itemId}/links`,
        method: "GET",
        format: "json",
        ...params,
      }),
    /**
     * No description
     *
     * @tags ItemLink
     * @name GetItemLinks
     * @summary Get all links.
     * @request GET:/api/items/{item_id}/links
     */
    useGetItemLinks: (itemId: number, options?: SWRConfiguration, doFetch: boolean = true) =>
      useSWR<LinkDto[], ErrorProdResponse>(doFetch ? `/api/items/${itemId}/links` : null, options),

    /**
     * No description
     *
     * @tags ItemLink
     * @name GetItemLinks
     * @summary Get all links.
     * @request GET:/api/items/{item_id}/links
     */
    mutateGetItemLinks: (itemId: number, data?: LinkDto[] | Promise<LinkDto[]>, options?: MutatorOptions) =>
      mutate<LinkDto[]>(`/api/items/${itemId}/links`, data, options),

    /**
     * No description
     *
     * @tags ItemLink
     * @name PostItemLinks
     * @summary Create a new link.
     * @request POST:/api/items/{item_id}/links
     */
    postItemLinks: (itemId: number, data: CreateLinkDto, params: RequestParams = {}) =>
      this.request<LinkDto, ErrorProdResponse>({
        path: `/api/items/${itemId}/links`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags ItemLink
     * @name GetItemLink
     * @summary Get a link.
     * @request GET:/api/items/{item_id}/links/{id}
     */
    getItemLink: (itemId: number, id: number, params: RequestParams = {}) =>
      this.request<LinkDto, ErrorProdResponse>({
        path: `/api/items/${itemId}/links/${id}`,
        method: "GET",
        format: "json",
        ...params,
      }),
    /**
     * No description
     *
     * @tags ItemLink
     * @name GetItemLink
     * @summary Get a link.
     * @request GET:/api/items/{item_id}/links/{id}
     */
    useGetItemLink: (itemId: number, id: number, options?: SWRConfiguration, doFetch: boolean = true) =>
      useSWR<LinkDto, ErrorProdResponse>(doFetch ? `/api/items/${itemId}/links/${id}` : null, options),

    /**
     * No description
     *
     * @tags ItemLink
     * @name GetItemLink
     * @summary Get a link.
     * @request GET:/api/items/{item_id}/links/{id}
     */
    mutateGetItemLink: (itemId: number, id: number, data?: LinkDto | Promise<LinkDto>, options?: MutatorOptions) =>
      mutate<LinkDto>(`/api/items/${itemId}/links/${id}`, data, options),

    /**
     * No description
     *
     * @tags ItemLink
     * @name PatchItemLink
     * @summary Update a link.
     * @request PATCH:/api/items/{item_id}/links/{id}
     */
    patchItemLink: (itemId: number, id: number, data: UpdateLinkDto, params: RequestParams = {}) =>
      this.request<LinkDto, ErrorProdResponse>({
        path: `/api/items/${itemId}/links/${id}`,
        method: "PATCH",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags ItemLink
     * @name DeleteItemLink
     * @summary Delete a link.
     * @request DELETE:/api/items/{item_id}/links/{id}
     */
    deleteItemLink: (itemId: number, id: number, params: RequestParams = {}) =>
      this.request<LinkDto, ErrorProdResponse>({
        path: `/api/items/${itemId}/links/${id}`,
        method: "DELETE",
        format: "json",
        ...params,
      }),
  };
  pikPak = {
    /**
     * No description
     *
     * @tags PikPak
     * @name GetModulesPikpakJobs
     * @request GET:/api/modules/pikpak/jobs
     */
    getModulesPikpakJobs: (params: RequestParams = {}) =>
      this.request<AnimeDto[], ErrorProdResponse>({
        path: `/api/modules/pikpak/jobs`,
        method: "GET",
        format: "json",
        ...params,
      }),
    /**
     * No description
     *
     * @tags PikPak
     * @name GetModulesPikpakJobs
     * @request GET:/api/modules/pikpak/jobs
     */
    useGetModulesPikpakJobs: (options?: SWRConfiguration, doFetch: boolean = true) =>
      useSWR<AnimeDto[], ErrorProdResponse>(doFetch ? `/api/modules/pikpak/jobs` : null, options),

    /**
     * No description
     *
     * @tags PikPak
     * @name GetModulesPikpakJobs
     * @request GET:/api/modules/pikpak/jobs
     */
    mutateGetModulesPikpakJobs: (data?: AnimeDto[] | Promise<AnimeDto[]>, options?: MutatorOptions) =>
      mutate<AnimeDto[]>(`/api/modules/pikpak/jobs`, data, options),

    /**
     * No description
     *
     * @tags PikPak
     * @name PostModulesPikpakJobs
     * @request POST:/api/modules/pikpak/jobs
     */
    postModulesPikpakJobs: (data: CreateAnimeDto, params: RequestParams = {}) =>
      this.request<AnimeDto, ErrorProdResponse>({
        path: `/api/modules/pikpak/jobs`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags PikPak
     * @name GetModulesPikpakJob
     * @request GET:/api/modules/pikpak/jobs/{id}
     */
    getModulesPikpakJob: (id: number, params: RequestParams = {}) =>
      this.request<AnimeDto, ErrorProdResponse>({
        path: `/api/modules/pikpak/jobs/${id}`,
        method: "GET",
        format: "json",
        ...params,
      }),
    /**
     * No description
     *
     * @tags PikPak
     * @name GetModulesPikpakJob
     * @request GET:/api/modules/pikpak/jobs/{id}
     */
    useGetModulesPikpakJob: (id: number, options?: SWRConfiguration, doFetch: boolean = true) =>
      useSWR<AnimeDto, ErrorProdResponse>(doFetch ? `/api/modules/pikpak/jobs/${id}` : null, options),

    /**
     * No description
     *
     * @tags PikPak
     * @name GetModulesPikpakJob
     * @request GET:/api/modules/pikpak/jobs/{id}
     */
    mutateGetModulesPikpakJob: (id: number, data?: AnimeDto | Promise<AnimeDto>, options?: MutatorOptions) =>
      mutate<AnimeDto>(`/api/modules/pikpak/jobs/${id}`, data, options),

    /**
     * No description
     *
     * @tags PikPak
     * @name PostModulesPikpakJob
     * @request POST:/api/modules/pikpak/jobs/{id}
     */
    postModulesPikpakJob: (id: number, data: UpdateAnimeDto, params: RequestParams = {}) =>
      this.request<AnimeDto, ErrorProdResponse>({
        path: `/api/modules/pikpak/jobs/${id}`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags PikPak
     * @name DeleteModulesPikpakJob
     * @request DELETE:/api/modules/pikpak/jobs/{id}
     */
    deleteModulesPikpakJob: (id: number, params: RequestParams = {}) =>
      this.request<void, ErrorProdResponse>({
        path: `/api/modules/pikpak/jobs/${id}`,
        method: "DELETE",
        ...params,
      }),
  };
  torrent = {
    /**
     * No description
     *
     * @tags Torrent
     * @name GetModulesTorrentDirectoryTorrents
     * @request GET:/api/modules/torrent_directory/torrents
     */
    getModulesTorrentDirectoryTorrents: (
      query?: {
        query?: string;
        /** @format int32 */
        count?: number;
        /** @format date-time */
        until?: string;
      },
      params: RequestParams = {},
    ) =>
      this.request<TorrentDto[], ErrorProdResponse>({
        path: `/api/modules/torrent_directory/torrents`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),
    /**
     * No description
     *
     * @tags Torrent
     * @name GetModulesTorrentDirectoryTorrents
     * @request GET:/api/modules/torrent_directory/torrents
     */
    useGetModulesTorrentDirectoryTorrents: (
      query?: {
        query?: string;
        /** @format int32 */
        count?: number;
        /** @format date-time */
        until?: string;
      },
      options?: SWRConfiguration,
      doFetch: boolean = true,
    ) =>
      useSWR<TorrentDto[], ErrorProdResponse>(
        doFetch ? [`/api/modules/torrent_directory/torrents`, query] : null,
        options,
      ),

    /**
     * No description
     *
     * @tags Torrent
     * @name GetModulesTorrentDirectoryTorrents
     * @request GET:/api/modules/torrent_directory/torrents
     */
    mutateGetModulesTorrentDirectoryTorrents: (
      query?: {
        query?: string;
        /** @format int32 */
        count?: number;
        /** @format date-time */
        until?: string;
      },
      data?: TorrentDto[] | Promise<TorrentDto[]>,
      options?: MutatorOptions,
    ) => mutate<TorrentDto[]>([`/api/modules/torrent_directory/torrents`, query], data, options),

    /**
     * No description
     *
     * @tags Torrent
     * @name PostModulesTorrentDirectoryProviderFetch
     * @request POST:/api/modules/torrent_directory/providers/{name}/fetch/{page}
     */
    postModulesTorrentDirectoryProviderFetch: (name: string, page: number, params: RequestParams = {}) =>
      this.request<void, ErrorProdResponse>({
        path: `/api/modules/torrent_directory/providers/${name}/fetch/${page}`,
        method: "POST",
        ...params,
      }),
  };
}

const api = new Api();
export default api;

export const fetcher = async (arg: string | [string, Record<string, unknown>?]) => {
  const { path, query } = typeof arg === "string" ? { path: arg, query: undefined } : { path: arg[0], query: arg[1] };
  return await api
    .request({ path, query })
    .then((res) => res.json())
    .catch(async (err) => {
      if (err.json) throw await err.json();
      throw err;
    });
};
