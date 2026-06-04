import { createContext, useContext } from "react";
import type { SseContextType } from "../lib/api";

export const SseContext = createContext<SseContextType | undefined>(undefined);

export const useSseContext = () => {
  const context = useContext(SseContext);
  if (!context) {
    throw new Error("useSseContext must be used within SseProvider");
  }
  return context;
};
