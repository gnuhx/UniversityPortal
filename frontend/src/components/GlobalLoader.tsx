import { useIsFetching, useIsMutating } from "@tanstack/react-query";

export function GlobalLoader() {
  const fetching = useIsFetching();
  const mutating = useIsMutating();
  const active = fetching + mutating > 0;

  return (
    <div
      style={{
        position: "fixed",
        top: 0,
        left: 0,
        right: 0,
        height: 3,
        zIndex: 9999,
        background: active
          ? "linear-gradient(90deg, #1677ff 0%, #52c41a 60%, #1677ff 100%)"
          : "transparent",
        backgroundSize: active ? "200% 100%" : "100% 100%",
        animation: active ? "globalLoaderSlide 1.4s linear infinite" : "none",
        transition: "opacity 0.2s",
        opacity: active ? 1 : 0,
      }}
    />
  );
}
