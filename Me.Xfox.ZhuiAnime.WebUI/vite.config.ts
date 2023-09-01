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
  build: {
    rollupOptions: {
      output: {
        manualChunks: (id) => {
          if (id.includes("/node_modules/.pnpm/")) {
            const stripped = id.split("/node_modules/.pnpm/")[1];
            if (stripped.startsWith("react")) return "react";
            if (stripped.startsWith("@chakra-ui+")) return "chakra";
            if (stripped.startsWith("@emotion+")) return "chakra";
            if (stripped.includes("framer-motion")) return "chakra";
            return "vendor";
          } else {
            console.log(id);
          }
        },
      },
    },
  },
});
