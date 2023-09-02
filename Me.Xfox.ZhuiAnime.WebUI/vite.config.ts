import react from "@vitejs/plugin-react";
import { defineConfig, splitVendorChunkPlugin } from "vite";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react(), splitVendorChunkPlugin()],
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
