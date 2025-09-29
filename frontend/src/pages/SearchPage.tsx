import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import Header from "../components/Header";
import { SEARCH_GAMES_API_URL } from "../util/constants";
import '../styles/pages/_searchpage.scss';

export default function SearchPage() {
    const [games, setGames] = useState([]);
    const [loading, setLoading] = useState(true);
    const [searchedGame, setSearchedGame] = useState('');
    const [searchParams] = useSearchParams();

    const fetchGames = async (query: string) => {
        try {
            const response = await fetch(SEARCH_GAMES_API_URL.replace('{gamename}', encodeURIComponent(query)).replace('{pagesize}', '20'));
            
            if(response.ok){
                const data = await response.json();
                setGames(data.results);
                setLoading(false);
            }else{
                console.error('Error fetching games:', response.statusText);
                setLoading(false);
            }
        } catch (err) {
            console.error('Error fetching games:', err);
            setLoading(false);
        }
    }

    useEffect(() => {
        const query = searchParams.get('gameName') || '';
        setSearchedGame(query);
        fetchGames(query);
    }, [searchParams]);

    const getGamesList = () => {
        if(games.length === 0){
            return <span className="no-games">Uh Oh, No games found...</span>
        } else {
            return (
                <div className="games-list">
                    {games.map((game: any) => (
                        <div key={game.id} className="game-card">
                            <div className="game-card-img-container">
                                <img src={game.background_image} alt={game.name} />
                            </div>
                            <div className="game-card-platforms"></div>
                            <div className="game-card-name">{game.name}</div>
                        </div>
                    ))}
                </div>
            );
        }
    }

    return (
        <>
            <Header />
            <section className="games-section">
                <h2>Search Results for {searchedGame}</h2>
                {!!loading ? <span className="loading-text">Loading Results</span> : getGamesList()}
            </section>
        </>
    );
}