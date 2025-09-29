import { useSelector, useDispatch } from "react-redux";
import { useNavigate, Link } from "react-router-dom";
import { useState, useContext } from "react";
import { clearToken } from "../redux/slicers/authTokenSlice";
import { MobileContext } from "../context/MobileContext";

export default function Header() {
    const isAuthenticated = useSelector((state: any) => state.authToken.isAuthenticated);
    const isMobile = useContext(MobileContext)
    const [searchQuery, setSearchQuery] = useState('');
    const [isMenuOpen, setIsMenuOpen] = useState(false);
    const [openLinkSection, setOpenLinkSection] = useState(false);
    const [openSearchSection, setOpenSearchSection] = useState(false);
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

    const headerMobile = () => {
        return (
            <>
                <header className="header">
                    <section className="header-title">
                        <Link className="title" to="/">The Backlog</Link>
                    </section>

                    <section className="header-menu">
                        <button className="menu-button" onClick={() => setIsMenuOpen(!isMenuOpen)}>
                            <span className="menu-icon">&#9776;</span>
                        </button>
                    </section>
                </header>
                {isMenuOpen && (
                    <div className="dropdown-menu">
                        <section className="link-section" onClick={() => setOpenLinkSection(!openLinkSection)}>
                            <span className="link-section-title">Links</span>
                            {openLinkSection &&
                                <nav className="link-section-nav">
                                    <Link to="/new-games" >New Games</Link>
                                </nav>
                            }
                        </section>
                        <section className="search-section">
                            <span className="search-section-title" onClick={() => setOpenSearchSection(!openSearchSection)}>Search</span>
                            {openSearchSection &&
                                <form onSubmit={handleSearch} className="search-form">
                                    <input
                                        type="text"
                                        placeholder="Search..."
                                        onChange={(e) => setSearchQuery(e.target.value)}
                                    />
                                </form>
                            }
                        </section>
                        <section className="user-section">
                            {isAuthenticated &&
                                <span className="user-section-logout" onClick={() => { dispatch(clearToken()); } }>Log out</span>
                            }
                            {!isAuthenticated &&
                                <Link className="user-section-signup" to="/signup" onClick={() => setIsMenuOpen(false)}>Sign up</Link>
                            }
                        </section>
                    </div>
                )}
            </>
        );
    }

    return (
        !isMobile ? headerDesktop() : headerMobile()
    );
}