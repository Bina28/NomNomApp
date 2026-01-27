import { useEffect, useRef } from "react";
import { useAuth } from "../context/AuthContext";

type SseEvent = {
  type: string;
  data: unknown;
};

type SseCallbacks = {
  onNewFollow?: (data: { followerName: string; followingName: string }) => void;
  onNewComment?: (data: { userName: string; text: string; recipeId: number }) => void;
};

export function useSse(callbacks: SseCallbacks) {
  const { isLoggedIn } = useAuth();
  const eventSourceRef = useRef<EventSource | null>(null);

  useEffect(() => {
    if (!isLoggedIn) {
      return;
    }

    const apiUrl = import.meta.env.VITE_API_URL;
    const eventSource = new EventSource(`${apiUrl}/sse/stream`, {
      withCredentials: true,
    });

    eventSourceRef.current = eventSource;

    eventSource.addEventListener("new_follow", (event) => {
      const data = JSON.parse(event.data);
      callbacks.onNewFollow?.(data);
    });

    eventSource.addEventListener("new_comment", (event) => {
      const data = JSON.parse(event.data);
      callbacks.onNewComment?.(data);
    });

    eventSource.addEventListener("ping", () => {
      console.log("SSE ping received");
    });

    eventSource.onerror = (error) => {
      console.error("SSE error:", error);
    };

    return () => {
      eventSource.close();
      eventSourceRef.current = null;
    };
  }, [isLoggedIn]);

  return eventSourceRef;
}
