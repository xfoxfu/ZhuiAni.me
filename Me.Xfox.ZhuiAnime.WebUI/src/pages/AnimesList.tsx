import Api from "../api";
import {
  Component,
  ErrorBoundary,
  For,
  Show,
  Suspense,
  createEffect,
  createSignal,
  resetErrorBoundaries,
} from "solid-js";
import { alertError } from "../components/AlertError";

export const AnimesList: Component = () => {
  const [categoryId, setCategoryId] = createSignal(0);
  const [categories] = Api.category.useCategories(() => ({}));
  const [items] = Api.category.useCategoryItems(() => ({ id: categoryId() }));

  createEffect(() => {
    categoryId();
    resetErrorBoundaries();
  });

  return (
    <>
      <Suspense fallback={<progress class="w-56" />}>
        <ErrorBoundary fallback={alertError}>
          <Show when={categories()}>
            <div class="flex gap-x-3">
              <For each={categories()}>
                {(category) => (
                  <button
                    classList={{ "btn btn-primary": true, "btn-outline": categoryId() !== category.id }}
                    onClick={() => setCategoryId(category.id)}
                  >
                    {category.title}
                  </button>
                )}
              </For>
            </div>
          </Show>
        </ErrorBoundary>
      </Suspense>
      <Show when={categoryId() !== 0}>
        <Suspense fallback={<progress class="progress w-56" />}>
          <ErrorBoundary fallback={alertError}>
            <Show when={items()}>
              <div class="flex gap-x-3 gap-y-2 align-items-stretch">
                <For each={items()}>
                  {(item) => (
                    <div class="card shadow-md">
                      <div class="card-body">
                        <a href="">
                          <h3 class="card-title">{item.title}</h3>
                        </a>
                      </div>
                    </div>
                  )}
                </For>
              </div>
            </Show>
          </ErrorBoundary>
        </Suspense>
      </Show>
    </>
  );
};
