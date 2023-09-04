import { ErrorProdResponse, HttpResponse } from "./api";
import { createStandaloneToast } from "@chakra-ui/react";

const { ToastContainer, toast } = createStandaloneToast();
export const ErrorToastContainer = ToastContainer;

export const promiseWithLog = (promise: Promise<unknown> | (() => Promise<unknown>)) => {
  (typeof promise === "function" ? promise() : promise).catch((err) => console.error(err));
};
export const promiseWithToast = (promise: Promise<unknown> | (() => Promise<unknown>)) => {
  (typeof promise === "function" ? promise() : promise).catch((err) => {
    console.error(err);
    toast({
      title: "An error occurred.",
      description:
        (err as ErrorProdResponse).message ??
        (err as HttpResponse<void, ErrorProdResponse>).error?.message ??
        (err as Error).message ??
        "Unknown error",
      status: "error",
      duration: 5000,
      isClosable: true,
    });
  });
};
