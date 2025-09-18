import { useSelector, useDispatch } from "react-redux";
import { clearToken } from "../redux/slicers/authTokenSlice";

export default function Header() {
    const isAuthenticated = useSelector((state: any) => state.authToken.isAuthenticated);
    const dispatch = useDispatch();

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
                {isAuthenticated &&
                    <button type='button' className="standard-button" onClick={() => {
                                dispatch(clearToken());
                            }
                        }>
                        <span>log out</span>
                    </button>
                }
                {!isAuthenticated &&
                    <button type='button' className="standard-button" onClick={() => window.location.href = '/signup'}>
                        <span>sign up</span>
                    </button>
                }
            </section>
        </header>
    );
}