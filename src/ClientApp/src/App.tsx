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

const anime = [
  ["名侦探柯南", "https://mikanani.me/images/Bangumi/201310/91d95f43.jpg"],
  ["海贼王", "https://mikanani.me/images/Bangumi/201310/0aa598c7.jpg"],
  ["火影忍者 博人传之火影次世代", "https://mikanani.me/images/Bangumi/201704/e46ad033.jpg"],
  ["宝可梦 旅途‎", "https://mikanani.me/images/Bangumi/201911/0b25e1cd.jpg"],
  ["数码兽大冒险：", "https://mikanani.me/images/Bangumi/202004/2e66e770.jpg"],
  ["王者天下 第三季", "https://mikanani.me/images/Bangumi/202004/5988c4af.jpg"],
  ["舞伎家的料理人", "https://mikanani.me/images/Bangumi/202102/d74196db.jpg"],
  ["Tropical-Rouge！光之美少女", "https://mikanani.me/images/Bangumi/202102/f3a3a47c.jpg"],
  ["致不灭的你", "https://mikanani.me/images/Bangumi/202104/98a02bea.jpg"],
  ["甜梦猫 MIX!", "https://mikanani.me/images/Bangumi/202104/8418c9a0.jpg"],
  ["指尖传出的真挚热情2-恋人是消防员-", "https://mikanani.me/images/Bangumi/202106/efe4ab77.jpg"],
  ["死神少爷与黑女仆", "https://mikanani.me/images/Bangumi/202107/cbc5d713.jpg"],
];
const subscriptions = [
  { n: "现实主义勇者的王国再建记", e: "04", g: "NC-Raws", r: "1080p", s: "WebDL", t: "MKV", l: "zh-Hans" },
  { n: "现实主义勇者的王国再建记", e: "04", g: "NC-Raws", r: "1080p", s: "WebDL", t: "MKV", l: "zh-Hans" },
  { n: "现实主义勇者的王国再建记", e: "04", g: "NC-Raws", r: "1080p", s: "WebDL", t: "MKV", l: "zh-Hans" },
  { n: "现实主义勇者的王国再建记", e: "04", g: "NC-Raws", r: "1080p", s: "WebDL", t: "MKV", l: "zh-Hans" },
];

const App: React.FunctionComponent = () => {
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
            subscriptions.map((s) => (
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
            {anime.map((a) => (
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
