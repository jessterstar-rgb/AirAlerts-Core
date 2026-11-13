"use client";

import "leaflet/dist/leaflet.css";
import { MapContainer, TileLayer } from "react-leaflet";

export const AlertsMap: React.FC = () => (
  <MapContainer center={[48.5, 31.5]} zoom={6} style={{ height: "500px", width: "100%" }}>
    <TileLayer
      attribution="&copy; OpenStreetMap contributors"
      url="https://tile.openstreetmap.org/{z}/{x}/{y}.png"
    />
  </MapContainer>
);
