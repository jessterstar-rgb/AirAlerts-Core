"use client";

import { geoMercator, geoPath } from "d3-geo";
import type { Feature, FeatureCollection } from "geojson";
import { useEffect, useMemo, useState } from "react";

import { ISO_TO_REGION_ID, MARKER_COORDS } from "@/lib/regionGeo";

import styles from "./AlertsMap.module.css";

type RegionState = {
    regionId: string;
    regionName: string;
    isActive: boolean;
};

type Props = { regions: RegionState[] };
type Shape = { regionId: string; d: string };
type Marker = { regionId: string; x: number; y: number };

const WIDTH = 900;
const HEIGHT = 600;
const GEOJSON_URL = "/ukraine.geojson";

// у яких полях файлу може лежати ISO-код (я не знаю напевно, тому кілька варіантів)
const ISO_KEYS = ["ISO", "iso", "ISO_3166-2", "iso_3166_2", "shapeISO", "code"];

const normalizeIso = (value: string) =>
    /^\d{2}$/.test(value) ? `UA-${value}` : value.toUpperCase();

const getIso = (feature: Feature) => {
    const props = feature.properties ?? {};
    const key = ISO_KEYS.find((k) => typeof props[k] === "string");
    return key ? normalizeIso(props[key] as string) : undefined;
};

export const AlertsMap = ({ regions }: Props) => {
    const [collection, setCollection] = useState<FeatureCollection | null>(null);

    useEffect(() => {
        const controller = new AbortController();

        fetch(GEOJSON_URL, { signal: controller.signal })
            .then((res) => res.json())
            .then(setCollection)
            .catch((err) => {
                if (err.name !== "AbortError") {
                    console.error("Не вдалося завантажити GeoJSON", err);
                }
            });

        return () => controller.abort();
    }, []);

    const { shapes, markers } = useMemo((): { shapes: Shape[]; markers: Marker[] } => {
        if (!collection) {
            return { shapes: [], markers: [] };
        }

        const projection = geoMercator().fitSize([WIDTH, HEIGHT], collection);
        const toPath = geoPath(projection);

        const shapes = collection.features.flatMap((feature) => {
            const iso = getIso(feature);
            const regionId = iso ? ISO_TO_REGION_ID[iso] : undefined;
            const d = toPath(feature);
            return regionId && d ? [{ regionId, d }] : [];
        });

        const markers = Object.entries(MARKER_COORDS).flatMap(([regionId, coords]) => {
            const hasShape = shapes.some((s) => s.regionId === regionId);
            const point = projection(coords);
            return hasShape || !point ? [] : [{ regionId, x: point[0], y: point[1] }];
        });

        return { shapes, markers };
    }, [collection]);

    useEffect(() => {
        if (collection && shapes.length === 0) {
            console.warn(
                "Жоден контур не зіставився з регіонами. Властивості першого елемента:",
                collection.features[0]?.properties,
            );
        }
    }, [collection, shapes]);

    const byId = useMemo(() => new Map(regions.map((r) => [r.regionId, r])), [regions]);

    const label = (regionId: string) => {
        const region = byId.get(regionId);
        return region
            ? `${region.regionName}: ${region.isActive ? "тривога" : "чисто"}`
            : regionId;
    };

    return (
        <svg
            viewBox={`0 0 ${WIDTH} ${HEIGHT}`}
            className={styles.map}
            role="img"
            aria-label="Мапа повітряних тривог України"
        >
            {shapes.map(({ regionId, d }, index) => (
                <path
                    key={`${regionId}-${index}`}
                    d={d}
                    className={byId.get(regionId)?.isActive ? styles.active : styles.calm}
                >
                    <title>{label(regionId)}</title>
                </path>
            ))}

            {markers.map(({ regionId, x, y }) => (
                <circle
                    key={regionId}
                    cx={x}
                    cy={y}
                    r={7}
                    className={byId.get(regionId)?.isActive ? styles.active : styles.calm}
                >
                    <title>{label(regionId)}</title>
                </circle>
            ))}
        </svg>
    );
};
