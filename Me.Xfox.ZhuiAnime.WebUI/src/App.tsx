import { Flex, Heading, chakra, Stack, Divider } from "@chakra-ui/react";
import React from "react";
import { Routes, Route, Link } from "react-router-dom";
import { Item } from "./pages/Item";
import { ItemsList } from "./pages/ItemsList";
import { TorrentsList } from "./pages/Torrents";

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
          <Link to="/">Animations</Link>
        </Heading>
        <Heading as="h2" size="lg">
          <Link to="/torrents">Torrents</Link>
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
        <Routes>
          <Route path="/" element={<ItemsList />}></Route>
          <Route path="/animes">
            <Route path=":animeId" element={<Item />} />
          </Route>
          <Route path="/torrents" element={<TorrentsList />}></Route>
        </Routes>
      </Stack>
    </Flex>
  );
};

export default App;
