import { useParams } from "@solidjs/router";
import { Component, ErrorBoundary, For, Show, Suspense } from "solid-js";
import { alertError } from "../components/AlertError";
import api from "../api";
import { ItemLinks } from "../components/ItemLinks";

export const Item: Component = () => {
  const params = useParams();
  const itemId = Number.parseInt(params["itemId"] ?? "0");
  const [item] = api.item.useItem(() => ({ id: itemId }));
  const [childItems] = api.item.useItemItems(() => ({ id: itemId }));

  return (
    <>
      <Suspense fallback={<progress class="w-56" />}>
        <ErrorBoundary fallback={alertError}>
          <Show when={item()}>
            <h1 class="text-3xl font-bold">{item()?.title}</h1>
          </Show>
        </ErrorBoundary>
      </Suspense>
      <ItemLinks itemId={itemId} />
      <Suspense fallback={<progress class="w-56" />}>
        <ErrorBoundary fallback={alertError}>
          <Show when={childItems()}>
            <For each={childItems()}>
              {(item) => (
                <div class="card w-full bg-base-100 shadow-xl">
                  <div class="card-body">
                    <h2 class="card-title">{item.title}</h2>
                    <ItemLinks itemId={item.id} />
                  </div>
                </div>
              )}
            </For>
          </Show>
        </ErrorBoundary>
      </Suspense>
    </>
  );
};
export default Item;
