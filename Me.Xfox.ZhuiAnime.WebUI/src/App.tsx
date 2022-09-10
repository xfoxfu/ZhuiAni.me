import {
  Flex,
  VStack,
  HStack,
  Heading,
  Button,
  Center,
  Tag,
  chakra,
  Stack,
  SimpleGrid,
  Image,
  Divider,
  Spacer,
} from "@chakra-ui/react";
import React from "react";
import Api from "./api";

const App: React.FunctionComponent = () => {
  const { data, error } = Api.anime.useAnimesList();

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
          </HStack>
          <SimpleGrid spacingX="3" spacingY="2" minChildWidth="18ch">
            {data?.map((a) => (
              <VStack px="3" py="2" borderWidth="1px" rounded="md" align="stretch" bg="white">
                <Image alt={a.title} />
                <Spacer />
                <Heading as="h3" size="sm">
                  {a.title}
                </Heading>
              </VStack>
            ))}
          </SimpleGrid>
        </Stack>
      </Stack>
    </Flex>
  );
};

export default App;
