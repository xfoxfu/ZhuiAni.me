import api from "../api";
import {
  Alert,
  AlertDescription,
  AlertIcon,
  Box,
  Button,
  GridItem,
  Heading,
  HStack,
  Icon,
  Image,
  SimpleGrid,
  Stack,
  Tooltip,
  VStack,
} from "@chakra-ui/react";
import React from "react";
import { IoImageSharp } from "react-icons/io5";
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
          <SimpleGrid spacingX="3" spacingY="2" alignItems="stretch" minChildWidth="24ch">
            {animes?.map((a) => (
              <GridItem key={a.id} px="3" py="2" borderWidth="1px" rounded="md" bg="white">
                <VStack height="100%" maxWidth="32ch" justifyContent="space-between">
                  {a.image_url ? (
                    <Image src={a.image_url} alt={a.title} width="auto" height="auto" />
                  ) : (
                    <Icon as={IoImageSharp} w="12" h="12" flexGrow="1" />
                  )}
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
