import { createContext, useContext, useState, ReactNode } from "react";
import { Toast, ToastContainer } from "react-bootstrap";
import { useSse } from "../hooks/useSse";

type Notification = {
  id: number;
  title: string;
  message: string;
  type: "follow" | "comment";
};

type SseContextType = {
  notifications: Notification[];
};

const SseContext = createContext<SseContextType | undefined>(undefined);

export function SseProvider({ children }: { children: ReactNode }) {
  const [notifications, setNotifications] = useState<Notification[]>([]);

  const addNotification = (title: string, message: string, type: "follow" | "comment") => {
    const id = Date.now();
    setNotifications((prev) => [...prev, { id, title, message, type }]);

    setTimeout(() => {
      setNotifications((prev) => prev.filter((n) => n.id !== id));
    }, 5000);
  };

  const removeNotification = (id: number) => {
    setNotifications((prev) => prev.filter((n) => n.id !== id));
  };

  useSse({
    onNewFollow: (data) => {
      addNotification(
        "New Follower!",
        `${data.followerName} started following ${data.followingName}`,
        "follow"
      );
    },
    onNewComment: (data) => {
      addNotification(
        "New Comment!",
        `${data.userName}: "${data.text}"`,
        "comment"
      );
    },
  });

  return (
    <SseContext.Provider value={{ notifications }}>
      {children}
      <ToastContainer position="bottom-end" className="p-3">
        {notifications.map((notification) => (
          <Toast
            key={notification.id}
            onClose={() => removeNotification(notification.id)}
            bg={notification.type === "follow" ? "primary" : "success"}
          >
            <Toast.Header>
              <strong className="me-auto">{notification.title}</strong>
            </Toast.Header>
            <Toast.Body className="text-white">
              {notification.message}
            </Toast.Body>
          </Toast>
        ))}
      </ToastContainer>
    </SseContext.Provider>
  );
}

export const useSseContext = () => {
  const context = useContext(SseContext);
  if (!context) {
    throw new Error("useSseContext must be used within SseProvider");
  }
  return context;
};
