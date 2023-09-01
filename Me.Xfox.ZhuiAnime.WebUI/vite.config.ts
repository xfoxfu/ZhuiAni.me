import { defineConfig, splitVendorChunkPlugin } from "vite";
import react from "@vitejs/plugin-react";
import eslint from "vite-plugin-eslint";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react(), eslint(), splitVendorChunkPlugin()],
  server: {
    proxy: {
      "/api": {
        target: process.env.VITE_PROXY_API ?? "https://localhost:5001",
        secure: false,
        changeOrigin: true,
      },
    },
  },
});
