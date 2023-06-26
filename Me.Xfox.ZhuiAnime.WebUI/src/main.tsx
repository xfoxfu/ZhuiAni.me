import React from "react";
import { createRoot } from "react-dom/client";
import App from "./App";
import { ChakraProvider } from "@chakra-ui/react";
import { SWRConfig } from "swr";
import { fetcher } from "./api";
import { BrowserRouter } from "react-router-dom";

const container = document.getElementById("root");
// eslint-disable-next-line @typescript-eslint/no-non-null-assertion
const root = createRoot(container!);
root.render(
  <React.StrictMode>
    <ChakraProvider>
      <SWRConfig value={{ fetcher }}>
        <BrowserRouter>
          <App />
        </BrowserRouter>
      </SWRConfig>
    </ChakraProvider>
  </React.StrictMode>
);
