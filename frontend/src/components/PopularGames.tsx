import React, { useEffect, useState } from "react";
import { POPULAR_GAMES_API_URL } from "../util/constants";
import '../styles/components/_populargames.scss';

export default function PopularGames() {
    const [games, setGames] = useState([]);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchPopularGames = async () => {
            try {
                const response = await fetch(POPULAR_GAMES_API_URL);
                if(response.ok){
                    const data = await response.json();
                    setGames(data);
                }
            }catch(err){
                setError(err instanceof Error ? err.message : String(err));
                console.error(err);
            }
        }

        fetchPopularGames();
    }, [])

    return (
        <article className="popular-games-section">
            <h2>Popular Games</h2>
            {!!error && <p className="error-message">Error: {error}</p>}
            {!!games && games.length > 0 && <>
                <ul className="popular-games-list">
                    {games.map((game: any) => (
                        <li key={game.id} className="popular-game-item">
                            {game.name}
                        </li>
                    ))}
                </ul>
            </>}
        </article>
    );
}