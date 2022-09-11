import React from "react";
import { createRoot } from "react-dom/client";
import App from "./App";
import { ChakraProvider } from "@chakra-ui/react";
import { SWRConfig } from "swr";
import { fetcher } from "./api";

const container = document.getElementById("root");
// eslint-disable-next-line @typescript-eslint/no-non-null-assertion
const root = createRoot(container!);
root.render(
  <React.StrictMode>
    <ChakraProvider>
      <SWRConfig value={{ fetcher }}>
        <App />
      </SWRConfig>
    </ChakraProvider>
  </React.StrictMode>
);
