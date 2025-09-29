import React, { useContext } from "react";
import { MobileContext } from "../context/MobileContext";
import Header from "../components/Header";
import Login from "../components/Login";
import Register from "../components/Register";
import '../styles/pages/_signinandsignup.scss';

function SignInAndSignUp() {
    const isMobile = useContext(MobileContext)

    return (
        <>
            <Header />
            <section className={`login-register-page ${isMobile ? 'mobile' : ''}`}>
                <article className="signup-form">
                    <Register />
                </article>
                <article className="signin-form">
                    <Login />
                </article>
            </section>
        </>
    );
}

export default SignInAndSignUp;