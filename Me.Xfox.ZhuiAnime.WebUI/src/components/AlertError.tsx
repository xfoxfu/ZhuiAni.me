import { Match, Switch } from "solid-js";
import { ApiError } from "../api";

export const AlertError = (props: { error: unknown }) => (
  <div class="alert alert-error shadow-lg">
    <Switch>
      <Match when={props.error instanceof ApiError}>
        <span>{(props.error as ApiError)?.message}</span>
      </Match>
      <Match when={props.error instanceof Error}>
        <span>{(props.error as Error)?.message}</span>
      </Match>
      <span>Unknown error occurred.</span>
    </Switch>
  </div>
);

export const alertError = (error: unknown) => <AlertError error={error} />;
