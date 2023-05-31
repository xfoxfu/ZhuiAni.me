import { Suspense, ErrorBoundary, Show, For, Switch, Match } from "solid-js";
import { alertError } from "./AlertError";
import api from "../api";
import { IconVideo } from "../assets/icons/Video";
import MIMEType from "whatwg-mimetype";
import { IconGlobe } from "../assets/icons/Globe";

export const ItemLinks = (props: { itemId: number }) => {
  const [links] = api.itemLink.useItemLinks(() => ({ itemId: props.itemId }));

  return (
    <ul class="menu menu-horizontal bg-base-200">
      <Suspense fallback={<progress class="w-56" />}>
        <ErrorBoundary fallback={alertError}>
          <Show when={links()}>
            <For each={links()}>
              {(link) => {
                const mime = new MIMEType(link.mime_type);
                return (
                  <li>
                    <a href={link.address} target="blank">
                      <Switch>
                        <Match when={mime?.essence === "text/html" && mime?.parameters?.get("kind") === "video"}>
                          <IconVideo />
                        </Match>
                        <Match when>
                          <IconGlobe />
                        </Match>
                      </Switch>
                    </a>
                  </li>
                );
              }}
            </For>
          </Show>
        </ErrorBoundary>
      </Suspense>
    </ul>
  );
};
