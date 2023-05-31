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
      <div class="flex flex-wrap gap-x-3 gap-y-2">
        <Suspense fallback={<progress class="w-56" />}>
          <ErrorBoundary fallback={alertError}>
            <Show when={childItems()}>
              <For each={childItems()}>
                {(item) => (
                  <div class="w-72 px-3 py-2 bg-base-100 shadow-xl space-y-1">
                    <h2 class="text-xl">{item.title}</h2>
                    <ItemLinks itemId={item.id} />
                  </div>
                )}
              </For>
            </Show>
          </ErrorBoundary>
        </Suspense>
      </div>
    </>
  );
};
export default Item;
