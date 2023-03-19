import { Route, Routes } from "@solidjs/router";
import { Component, lazy } from "solid-js";

import { AnimesList } from "./pages/AnimesList";
const Anime = lazy(() => import("./pages/Anime"));

const App: Component = () => {
  return (
    <div class="flex">
      <div class="flex-1 min-h-screen px-6 py-4 space-y-4 bg-green-500 text-white font-semibold">
        <h1 class="text-green-50 text-5xl">
          <span class="text-green-100 font-light">Zhui</span>
          Ani<span class="font-light">.</span>me
        </h1>
        <h2 class="text-2xl">Animations</h2>
        <h2 class="text-2xl">Torrents</h2>
        <div class="divider" />
        <div class="space-x-3">
          <h2 class="inline">Login</h2>
          <h2 class="inline">Register</h2>
        </div>
      </div>
      <div class="flex-3 bg-gray-50 p-4 space-y-4">
        <Routes>
          <Route path="/" element={<AnimesList />} />
          <Route path="/animes">
            <Route path="/:animeId" element={<Anime />} />
          </Route>
        </Routes>
      </div>
    </div>
  );
};

export default App;
