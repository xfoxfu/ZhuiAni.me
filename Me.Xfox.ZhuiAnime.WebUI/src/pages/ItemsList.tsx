import {
  Heading,
  Stack,
  HStack,
  Button,
  Wrap,
  WrapItem,
  VStack,
  Tooltip,
  Alert,
  AlertDescription,
  AlertIcon,
} from "@chakra-ui/react";
import React from "react";
import Api from "../api";
import { Link } from "react-router-dom";

export const ItemsList: React.FunctionComponent = () => {
  const { data: animes, error } = Api.api.useItemList();

  return (
    <>
      <Heading as="h1" size="xl" color="green.700">
        Animations
      </Heading>
      <Stack spacing="1">
        {error && (
          <Alert status="error">
            <AlertIcon />
            <AlertDescription>{error.message}</AlertDescription>
          </Alert>
        )}
        <HStack spacing="1.5">
          <Button size="sm">Today</Button>
          <Button size="sm">3 Days</Button>
          {/* <ImportBangumi /> */}
        </HStack>
        <Wrap spacingX="3" spacingY="2" alignItems="stretch">
          {animes?.map((a) => (
            <WrapItem alignItems="stretch" key={a.id}>
              <VStack
                minW="18ch"
                px="3"
                py="2"
                borderWidth="1px"
                rounded="md"
                align="stretch"
                bg="white"
                width="min-content"
              >
                {/* <Image src={`data:image;base64,${a.image_base64 ?? ""}`} alt={a.title} width="auto" height="auto" /> */}
                <Tooltip label={a.title}>
                  <Heading as="h3" size="sm" noOfLines={1}>
                    <Link to={`/animes/${a.id}`}>{a.title}</Link>
                  </Heading>
                </Tooltip>
              </VStack>
            </WrapItem>
          ))}
        </Wrap>
      </Stack>
    </>
  );
};
