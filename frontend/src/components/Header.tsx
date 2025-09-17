export default function Header() {
    return (
        <header className="header">
            <section className="header-title">
                <a className="title" href="/">The Backlog</a>
            </section>

            <section className="header-nav">
                <nav>
                    <a href="/new-games">New Games</a>
                </nav>    
            </section>

            <section className="header-user">
                <button type='button' className="standard-button" onClick={() => window.location.href = '/signup'}>
                    <span>sign up</span>
                </button>
            </section>
        </header>
    );
}