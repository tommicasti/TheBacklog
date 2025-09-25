import React, { useState, useEffect, createContext } from "react";

export const MobileContext = createContext(false);

export const MobileProvider = ({ children }: { children: React.ReactNode }) => {
    const [isMobile, setIsMobile] = useState(false);

    useEffect(() => {
        const mediaQuery = window.matchMedia("(max-width: 768px)");

        const handleMediaQueryChange = (event: MediaQueryListEvent) => {
            setIsMobile(event.matches);
        };

        setIsMobile(mediaQuery.matches);
        mediaQuery.addEventListener("change", handleMediaQueryChange);

        return () => {
            mediaQuery.removeEventListener("change", handleMediaQueryChange);
        };
    }, []);

    return (
        <MobileContext.Provider value={ isMobile }>
            {children}
        </MobileContext.Provider>
    );
}