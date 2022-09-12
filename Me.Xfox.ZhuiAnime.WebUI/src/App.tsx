import {
  Flex,
  VStack,
  HStack,
  Heading,
  Button,
  chakra,
  Stack,
  Image,
  Divider,
  Spacer,
  Wrap,
  WrapItem,
  Tooltip,
} from "@chakra-ui/react";
import React from "react";
import Api from "./api";
import { ImportBangumi } from "./components/anime/ImportBangumi";

const App: React.FunctionComponent = () => {
  const { data: animes, error } = Api.anime.useAnimesList({ include_image: true });
  return (
    <Flex>
      <Stack flex="1" bg="green.500" minH="100vh" paddingX="6" paddingY="4" color="white" spacing="4">
        <Heading color="green.50" as="h1" size="2xl">
          <chakra.span color="green.100" fontWeight="light">
            Zhui
          </chakra.span>
          Ani<chakra.span fontWeight="light">.</chakra.span>me
        </Heading>
        <Heading as="h2" size="lg">
          Animations
        </Heading>
        <Heading as="h2" size="lg">
          Torrents
        </Heading>
        <Divider />
        <Heading as="h2" size="md" display="inline">
          Login
        </Heading>
        <Heading as="h2" size="md" display="inline">
          Register
        </Heading>
      </Stack>
      <Stack flex="3" padding="4" bg="gray.50" spacing="4">
        <Heading as="h1" size="xl" color="green.700">
          Subscriptions
        </Heading>
        <Heading as="h1" size="xl" color="green.700">
          Animations
        </Heading>
        <Stack spacing="1">
          <HStack spacing="1.5">
            <Button size="sm">Today</Button>
            <Button size="sm">3 Days</Button>
            <ImportBangumi />
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
                  <Image src={`data:image;base64,${a.image_base64 ?? ""}`} alt={a.title} width="auto" height="auto" />
                  <Spacer />
                  <Tooltip label={a.title}>
                    <Heading as="h3" size="sm" noOfLines={1}>
                      {a.title}
                    </Heading>
                  </Tooltip>
                </VStack>
              </WrapItem>
            ))}
          </Wrap>
        </Stack>
      </Stack>
    </Flex>
  );
};

export default App;
