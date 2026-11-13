"use client";

import dynamic from "next/dynamic";

const AlertsMap = dynamic(() => import("../components/AlertsMap/AlertsMap"), {
  ssr: false,
});

import { useEffect, useState } from "react";

type AlertsState = Record<string, boolean>;

export default function Home() {
  const [alerts, setAlerts] = useState<AlertsState>({});
  const [connected, setConnected] = useState(false);

  useEffect(() => {
    // змінні значення живуть як поля const-об'єкта
    const conn: {
      ws?: WebSocket;
      timer?: ReturnType<typeof setTimeout>;
      stopped: boolean;
    } = { stopped: false };

    const connect = () => {
      const ws = new WebSocket("ws://localhost:5031/ws");
      conn.ws = ws;

      ws.onopen = () => setConnected(true);

      ws.onmessage = (e) => {
        const msg = JSON.parse(e.data);

        if (msg.type === "snapshot") {
          setAlerts(msg.data);
        } else if (msg.type === "change") {
          setAlerts((prev) => ({ ...prev, [msg.data.region]: msg.data.active }));
        }
      };

      ws.onclose = () => {
        setConnected(false);
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

  return (
      <main>
        <h1>Тривоги</h1>
        <AlertsMap />
        <p>{connected ? "🟢 підключено" : "🔴 немає з'єднання, пробую знову..."}</p>

        {Object.entries(alerts).map(([region, active]) => (
            <div key={region}>
              {region}: {active ? "🔴 тривога" : "🟢 чисто"}
            </div>
        ))}
      </main>
  );
}