import {
  Heading,
  Stack,
  Alert,
  AlertIcon,
  AlertDescription,
  Tag,
  Table,
  TableContainer,
  Tbody,
  Td,
  Tr,
  VStack,
  Text,
} from "@chakra-ui/react";
import React from "react";
import { useParams } from "react-router-dom";
import api from "../api";
import { ItemLinks } from "../components/anime/ItemLinks";

export const Item: React.FunctionComponent = () => {
  const params = useParams();
  const { data: anime, error: animeError } = api.api.useItemGet(Number.parseInt(params["animeId"] ?? "0", 10));
  const { data: episodes, error: episodesError } = api.api.useItemGetChildItems(
    Number.parseInt(params["animeId"] ?? "0", 10),
  );
  const error = animeError ?? episodesError;

  return (
    <>
      <Heading as="h1" size="xl" color="green.700">
        {anime?.title}
      </Heading>
      <Stack spacing="1">
        {error && (
          <Alert status="error">
            <AlertIcon />
            <AlertDescription>{error.message}</AlertDescription>
          </Alert>
        )}
        {anime && <ItemLinks id={anime?.id} />}
        <TableContainer width="fit-content">
          <Table>
            <Tbody>
              {episodes
                ?.sort((a, b) => a.title.localeCompare(b.title))
                ?.map((e) => (
                  <Tr key={e.id}>
                    <Td paddingY="1.5" paddingX="1">
                      <Tag variant="solid" colorScheme="teal" width="100%" justifyContent="center">
                        {e.annotations["https://bgm.tv/ep/:id/type"]} {e.annotations["https://bgm.tv/ep/:id/sort"]}
                      </Tag>
                    </Td>
                    <Td paddingY="1.5" paddingX="1">
                      <VStack spacing={1} alignItems="flex-start">
                        <Text>{e.title}</Text>
                        <ItemLinks id={e.id} />
                      </VStack>
                    </Td>
                  </Tr>
                ))}
            </Tbody>
          </Table>
        </TableContainer>
      </Stack>
    </>
  );
};
