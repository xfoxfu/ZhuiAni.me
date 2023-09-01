import React from "react";
import { createRoot } from "react-dom/client";
import App from "./App";
import { ChakraProvider } from "@chakra-ui/react";
import { SWRConfig } from "swr";
import { fetcher } from "./api";
import { BrowserRouter } from "react-router-dom";
import { defaultConfig } from "swr/_internal";

const container = document.getElementById("root");
// eslint-disable-next-line @typescript-eslint/no-non-null-assertion
const root = createRoot(container!);
root.render(
  <React.StrictMode>
    <ChakraProvider>
      <SWRConfig
        value={{
          fetcher,
          onErrorRetry(err, key, config, revalidate, revalidateOpts) {
            // eslint-disable-next-line @typescript-eslint/no-unsafe-member-access
            if (err.error_code === "INVALID_TOKEN") {
              return;
            }
            defaultConfig.onErrorRetry(err, key, config, revalidate, revalidateOpts);
          },
        }}
      >
        <BrowserRouter>
          <App />
        </BrowserRouter>
      </SWRConfig>
    </ChakraProvider>
  </React.StrictMode>,
);
