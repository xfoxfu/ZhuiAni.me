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

type Anime = [string, string];
type Subscription = { n: string, e: string, g: string, r: string, s: string, t: string, l: string; };

const App: React.FunctionComponent = () => {
  const [subscriptions, setSubscriptions] = React.useState<Subscription[]>();
  const [anime, setAnime] = React.useState<Anime[]>();

  React.useEffect(() => {
    fetch("/api/subscriptions", { headers: { "Content-Type": "application/json" } }).then(res => res.json()).then(setSubscriptions);
    fetch("/api/anime", { headers: { "Content-Type": "application/json" } }).then(res => res.json()).then(setAnime);
  }, []);

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
        <Stack spacing="1">
          {
            // background-image:url()
            subscriptions?.map((s) => (
              <HStack px="3" py="2" borderWidth="1px" rounded="md" bg="white">
                <Image src="https://mikanani.me/images/Bangumi/202107/97dda452.jpg" alt={s.n} maxH="3em" />
                <Stack spacing="1">
                  <Heading as="h3" size="sm">
                    {s.n} - {s.e}
                  </Heading>
                  <HStack spacing="1.5">
                    <Tag>{s.g}</Tag>
                    <Tag>{s.r}</Tag>
                    <Tag>{s.s}</Tag>
                    <Tag>{s.t}</Tag>
                    <Tag>{s.l}</Tag>
                  </HStack>
                </Stack>
                <Spacer />
                <Center>
                  <Button colorScheme="green" variant="ghost">
                    Magnet
                  </Button>
                  <Button colorScheme="green" variant="ghost">
                    Torrent
                  </Button>
                </Center>
              </HStack>
            ))
          }
        </Stack>
        <Heading as="h1" size="xl" color="green.700">
          Animations
        </Heading>
        <Stack spacing="1">
          <HStack spacing="1.5">
            <Button size="sm">Today</Button>
            <Button size="sm">3 Days</Button>
          </HStack>
          <SimpleGrid spacingX="3" spacingY="2" minChildWidth="18ch">
            {anime?.map((a) => (
              <VStack px="3" py="2" borderWidth="1px" rounded="md" align="stretch" bg="white">
                <Image src={a[1]} alt={a[0]} />
                <Spacer />
                <Heading as="h3" size="sm">
                  {a[0]}
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
