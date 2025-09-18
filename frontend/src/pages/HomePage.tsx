import Header from "../components/Header";
import PopularGames from "../components/PopularGames";
import '../styles/pages/_homepage.scss';

function HomePage() {
    return (
        <>
            <Header />
            <section className="welcome-section">
                <article>
                    <h2>Welcome to The Backlog</h2>
                    <p>Manage the list of games you have in your backlog. Add games and update the status of your current game.</p>
                </article>
                <PopularGames />
            </section>
        </>
    );
}

export default HomePage;