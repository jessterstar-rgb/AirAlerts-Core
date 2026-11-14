"use client";

import { AlertsMap } from "@/components/AlertsMap/AlertsMap";
import { useConnection } from "@/containers/AlertsMapContainer/hooks/useConnection";

export const AlertsMapContainer: React.FC = () => {
    const { alerts } = useConnection();

    return alerts ? <AlertsMap regions={alerts} /> : null;
};

export default AlertsMapContainer;
