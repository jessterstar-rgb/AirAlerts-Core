import { AlertsMap } from "@/components/AlertsMap/AlertsMap";
import { useConnection } from "@/containers/AlertsMapContainer/hooks/useConnection";

export const AlertsMapContainer: React.FC = () => {
  const { isConnected, alerts } = useConnection();

  console.log({ isConnected, alerts });

  return <AlertsMap />;
};

export default AlertsMapContainer;
