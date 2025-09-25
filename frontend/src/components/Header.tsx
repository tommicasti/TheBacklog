import { useSelector, useDispatch } from "react-redux";
import { useNavigate, Link } from "react-router-dom";
import { useState, useContext } from "react";
import { clearToken } from "../redux/slicers/authTokenSlice";
import { MobileContext } from "../context/MobileContext";

export default function Header() {
    const isAuthenticated = useSelector((state: any) => state.authToken.isAuthenticated);
    const isMobile = useContext(MobileContext)
    const [searchQuery, setSearchQuery] = useState('');
    const dispatch = useDispatch();
    const navigate = useNavigate();
    

    const handleSearch = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        if(searchQuery.trim() !== ''){
           navigate(`/search?gameName=${searchQuery.trim()}`);
        }
    }

    const headerDesktop = () => {
        return (
            <header className="header">
                <section className="header-title">
                    <Link className="title" to="/">The Backlog</Link>
                </section>

                <section className="header-nav">
                    <nav>
                        <Link to="/new-games">New Games</Link>
                    </nav>    
                </section>
                
                <section className="search-bar">
                    <form onSubmit={handleSearch} className="search-form">
                        <input
                            type="text"
                            placeholder="Search..."
                            onChange={(e) => setSearchQuery(e.target.value)}
                        />
                    </form>
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

    return (
        !isMobile ? headerDesktop() : <p>Mobile Header</p>
    );
}