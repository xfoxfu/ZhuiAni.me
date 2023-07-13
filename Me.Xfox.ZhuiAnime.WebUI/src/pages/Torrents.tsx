import {
  Heading,
  Stack,
  Alert,
  AlertDescription,
  AlertIcon,
  HStack,
  Button,
  Input,
  InputGroup,
  InputLeftElement,
  Tooltip,
} from "@chakra-ui/react";
import React, { useState } from "react";
import Api, { ApiError, TorrentDto } from "../api";
import { CopyIcon, LinkIcon, SearchIcon } from "@chakra-ui/icons";
import { useCopyToClipboard, useDebounce } from "usehooks-ts";
import useSWRInfinite from "swr/infinite";
export const TorrentsList: React.FunctionComponent = () => {
  const [rawQuery, setQuery] = useState("");
  const query = useDebounce(rawQuery, 300);
  // TODO: generate API hooks for this pattern
  const {
    data: torrents,
    error,
    size,
    setSize,
  } = useSWRInfinite<TorrentDto[], ApiError>(
    (pageIndex, previousPageData: TorrentDto[]): [string, string, string | undefined] | null => {
      if (previousPageData?.length === 0) return null;

      if (pageIndex === 0) return ["/api/modules/torrent_directory/torrents", query, undefined];

      return [
        "/api/modules/torrent_directory/torrents",
        query,
        previousPageData
          ?.map((d) => new Date(d.published_at))
          .sort((a, b) => a.getTime() - b.getTime())[0]
          ?.toISOString() ?? "",
      ];
    },
    {
      fetcher: ([, query, until]: [string, string, string | undefined]): Promise<TorrentDto[]> =>
        Api.torrent.getModulesTorrentDirectoryTorrents({ query: query ?? "", until, count: 1 }).then((d) => d.data),
    }
  );
  const [, copy] = useCopyToClipboard();

  return (
    <>
      <Heading as="h1" size="xl" color="green.700">
        Torrents
      </Heading>
      <Stack spacing="1">
        {error && (
          <Alert status="error">
            <AlertIcon />
            <AlertDescription>{error.message}</AlertDescription>
          </Alert>
        )}
        <InputGroup>
          <InputLeftElement pointerEvents="none">
            <SearchIcon color="gray.300" />
          </InputLeftElement>
          <Input placeholder="Query (regexp)" onChange={(e) => setQuery(e.target.value)} />
        </InputGroup>
        {torrents?.flat()?.map((a) => (
          <HStack key={a.id} px="3" py="2" borderWidth="1px" rounded="md" bg="white" gap="2">
            {a.link_torrent && (
              <a href={a.link_torrent} target="_blank" rel="noreferrer">
                <Tooltip label={a.link_torrent} placement="top">
                  <Button variant="solid" colorScheme="teal" size="xs">
                    <LinkIcon />
                  </Button>
                </Tooltip>
              </a>
            )}
            {a.link_magnet && (
              <Tooltip label={a.link_magnet} placement="top">
                <Button
                  variant="solid"
                  colorScheme="teal"
                  size="xs"
                  // eslint-disable-next-line @typescript-eslint/no-misused-promises
                  onClick={() => copy(a.link_magnet ?? "")}
                >
                  <CopyIcon />
                </Button>
              </Tooltip>
            )}
            <Heading as="h3" size="sm" noOfLines={1}>
              {a.title}
            </Heading>
          </HStack>
        ))}
        {/* eslint-disable-next-line @typescript-eslint/no-misused-promises */}
        <Button variant="solid" colorScheme="teal" size="xs" onClick={() => setSize(size + 1)}>
          Load More
        </Button>
      </Stack>
    </>
  );
};
