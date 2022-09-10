import React from "react";
import ReactDOM from "react-dom";
// import "./index.css";
import App from "./App";
import { ChakraProvider } from "@chakra-ui/react";
import { SWRConfig } from "swr";
import { fetcher } from "./api";

ReactDOM.render(
  <React.StrictMode>
    <ChakraProvider>
      <SWRConfig value={{ fetcher }}>
        <App />
      </SWRConfig>
    </ChakraProvider>
  </React.StrictMode>,
  document.getElementById("root")
);
