import api from "../api";
import {
  Heading,
  Stack,
  HStack,
  Button,
  VStack,
  Tooltip,
  Alert,
  AlertDescription,
  AlertIcon,
  GridItem,
  SimpleGrid,
  Box,
} from "@chakra-ui/react";
import React from "react";
import { Link } from "react-router-dom";

export const ItemsList: React.FunctionComponent = () => {
  const { data: animes, error } = api.useItemList();

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
        <Box>
          <SimpleGrid spacingX="3" spacingY="2" alignItems="stretch" minChildWidth="16">
            {animes?.map((a) => (
              <GridItem alignItems="stretch" key={a.id} px="3" py="2" borderWidth="1px" rounded="md" bg="white">
                <VStack align="stretch" maxWidth="32ch">
                  {/* <Image src={`data:image;base64,${a.image_base64 ?? ""}`} alt={a.title} width="auto" height="auto" /> */}
                  <Tooltip label={a.title}>
                    <Heading as="h3" size="sm" noOfLines={1}>
                      <Link to={`/animes/${a.id}`}>{a.title}</Link>
                    </Heading>
                  </Tooltip>
                </VStack>
              </GridItem>
            ))}
          </SimpleGrid>
        </Box>
      </Stack>
    </>
  );
};
