import { Component } from "solid-js";
import api from "../api";

export const Anime: Component = () => {
  // const params = useParams();
  // const { data: anime, error: animeError } = api.anime.useAnimesDetail(Number.parseInt(params["animeId"] ?? "0", 10));
  // const { data: episodes, error: episodesError } = api.anime.useAnimesEpisodesDetail(
  //   Number.parseInt(params["animeId"] ?? "0", 10)
  // );
  // const error = animeError ?? episodesError;

  return (
    <>
      {/* <Heading as="h1" size="xl" color="green.700">
        {anime?.title}
      </Heading>
      <Stack spacing="1">
        {error && (
          <Alert status="error">
            <AlertIcon />
            <AlertDescription>{error.message}</AlertDescription>
          </Alert>
        )}
        <TableContainer width="fit-content">
          <Table>
            <Tbody>
              {episodes
                ?.sort((a, b) => a.name.localeCompare(b.name))
                ?.map((e) => (
                  <Tr key={e.id}>
                    <Td paddingY="1.5" paddingX="1">
                      <Tag variant="solid" colorScheme="teal" width="100%" justifyContent="center">
                        {e.name}
                      </Tag>
                    </Td>
                    <Td paddingY="1.5" paddingX="1">
                      {e.title}
                    </Td>
                  </Tr>
                ))}
            </Tbody>
          </Table>
        </TableContainer>
      </Stack> */}
    </>
  );
};
export default Anime;
