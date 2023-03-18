import { defineConfig, splitVendorChunkPlugin } from "vite";
import solid from "vite-plugin-solid";
import eslint from "vite-plugin-eslint";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [solid(), eslint(), splitVendorChunkPlugin()],
  server: {
    proxy: {
      "/api": {
        target: "https://localhost:5001",
        secure: false,
      },
    },
  },
});
