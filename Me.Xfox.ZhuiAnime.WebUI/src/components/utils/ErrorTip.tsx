import { ErrorProdResponse } from "../../api";
import { Alert, AlertDescription, AlertIcon } from "@chakra-ui/react";
import React from "react";

export const ErrorTip = ({ error }: { error?: ErrorProdResponse }) =>
  error && (
    <Alert status="error">
      <AlertIcon />
      <AlertDescription>{error.message}</AlertDescription>
    </Alert>
  );
