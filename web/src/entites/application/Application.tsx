import dynamic from "next/dynamic";

const AlertsMapContainer = dynamic(
  () => import("../../containers/AlertsMapContainer/AlertsMapContainer"),
  {
    ssr: false,
  },
);

export const Application = () => {
  return <AlertsMapContainer />;
};
