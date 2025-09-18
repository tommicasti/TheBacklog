import Header from "../components/Header";
import '../styles/pages/_notfound.scss';

function NotFound (){
    return (
        <>
            <Header />
            <section className="notfound-page">
                <h2>404: Page Not Found</h2>
                <p>Uh Oh, your page is in another castle</p>
            </section>
        </>
    );
}

export default NotFound;