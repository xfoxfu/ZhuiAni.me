import { Suspense, ErrorBoundary, Show, For } from "solid-js";
import { alertError } from "./AlertError";
import api from "../api";

export const ItemLinks = (props: { itemId: number }) => {
  const [links] = api.itemLink.useItemLinks(() => ({ itemId: props.itemId }));

  return (
    <ul class="menu menu-horizontal bg-base-200">
      <Suspense fallback={<progress class="w-56" />}>
        <ErrorBoundary fallback={alertError}>
          <Show when={links()}>
            <For each={links()}>
              {(link) => (
                <li>
                  <a href={link.address}>{link.id}</a>
                </li>
              )}
            </For>
          </Show>
        </ErrorBoundary>
      </Suspense>
    </ul>
  );
};
