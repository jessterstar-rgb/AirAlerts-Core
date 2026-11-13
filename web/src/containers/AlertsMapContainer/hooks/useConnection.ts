import { useEffect, useState } from "react";

type AlertsState = Record<string, boolean>;

type UseConnectionReturnType = {
  alerts: AlertsState;
  isConnected: boolean;
};

export const useConnection = (): UseConnectionReturnType => {
  const [alerts, setAlerts] = useState<AlertsState>({});
  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    const conn: {
      ws?: WebSocket;
      timer?: ReturnType<typeof setTimeout>;
      stopped: boolean;
    } = { stopped: false };

    const connect = () => {
      const ws = new WebSocket("ws://localhost:5031/ws");
      conn.ws = ws;

      ws.onopen = () => setIsConnected(true);

      ws.onmessage = (e) => {
        const msg = JSON.parse(e.data);

        if (msg.type === "snapshot") {
          setAlerts(msg.data);
        } else if (msg.type === "change") {
          setAlerts((prev) => ({ ...prev, [msg.data.region]: msg.data.active }));
        }
      };

      ws.onclose = () => {
        setIsConnected(false);
        if (!conn.stopped) {
          conn.timer = setTimeout(connect, 2000);
        }
      };
    };

    connect();

    return () => {
      conn.stopped = true;
      clearTimeout(conn.timer);
      conn.ws?.close();
    };
  }, []);

  return { alerts, isConnected };
};
